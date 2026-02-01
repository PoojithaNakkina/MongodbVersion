using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using esyasoft.mobility.CHRGUP.service.ocpp.WebSockets;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.WebSockets
{
    public enum LockedOcppProtocol
    {
        Unknown = 0,
        Ocpp16 = 16,
        Ocpp201 = 201
    }
    public class ChargerConnection
    {
        private readonly WebSocket _socket;
        private readonly string _chargePointId;
        private readonly string _tenantId;
        private readonly int _evseId;
        private readonly MongoDbContext _db;
        public LockedOcppProtocol LockedProtocol { get; private set; } = LockedOcppProtocol.Unknown;

        


        public ChargerConnection(string chargePointId, string tenantId, WebSocket socket , int evseId, MongoDbContext db)
        {
            _chargePointId = chargePointId;
            _tenantId = tenantId;
            _socket = socket;
            _evseId = evseId;
            _db = db;

            ChargerConnectionManager.RegisterConnection(chargePointId, this);
        }
        public void LockProtocol(LockedOcppProtocol protocol)
        {
            if (LockedProtocol == LockedOcppProtocol.Unknown)
            {
                LockedProtocol = protocol;
                return;
            }

            if (LockedProtocol != protocol)
            {
                throw new InvalidOperationException(
                    $"Protocol already locked to {LockedProtocol} but received {protocol}");
            }
        }

        public async Task ListenAsync()
        {
            ChargerConnectionManager.Add(_chargePointId, _socket);
            bool gracefulClose = false;

            try
            {
                var buffer = new byte[8192];


                while (_socket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result;
                    try
                    {
                        result = await _socket.ReceiveAsync(buffer, CancellationToken.None);
                    }
                    catch (WebSocketException ex)
                    {
                        Console.WriteLine(
                            $"WebSocket abruptly closed for charger {_chargePointId}: {ex.Message}"
                        );
                        break;
                    }
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        gracefulClose = true;
                        break;
                    }
                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var doc = JsonDocument.Parse(json);
                    var action = doc.RootElement[2].GetString();
                    var payload = doc.RootElement.GetArrayLength() > 3 ? doc.RootElement[3] : default;


                    if (action == "BootNotification")
                    {

                        LockedOcppProtocol detectedProtocol;


                        if (payload.TryGetProperty("chargingStation", out _))
                        {
                            detectedProtocol = LockedOcppProtocol.Ocpp201;
                        }
                        else
                        {
                            detectedProtocol = LockedOcppProtocol.Ocpp16;
                        }

                        if (detectedProtocol == LockedOcppProtocol.Ocpp201)
                        {
                            ChargerProtocolStore.Set(_chargePointId, OcppProtocol.V201);
                        }
                        else
                        {
                            ChargerProtocolStore.Set(_chargePointId, OcppProtocol.V16);
                        }

                        try
                        {
                            LockProtocol(detectedProtocol);
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine($"Protocol lock violation for {_chargePointId}: {ex.Message}");

                            await _socket.CloseAsync(
                                WebSocketCloseStatus.PolicyViolation,
                                "Protocol mismatch",
                                CancellationToken.None
                            );
                            return;
                        }
                    }
                    else
                    {
                        var expected = LockedProtocol;
                        var actual = ChargerProtocolStore.Get(_chargePointId) == OcppProtocol.V201
                            ? LockedOcppProtocol.Ocpp201
                            : LockedOcppProtocol.Ocpp16;

                        if (expected != LockedOcppProtocol.Unknown && expected != actual)
                        {
                            Console.WriteLine($"Protocol mismatch for {_chargePointId}");
                            await _socket.CloseAsync(WebSocketCloseStatus.PolicyViolation,
                                "Protocol mismatch", CancellationToken.None);
                            return;
                        }
                    }
                    
                    await ProtocolRouter.RouteAsync(
                        json,
                        _chargePointId,
                        _tenantId,
                        //payload,
                        _socket,
                        _db
                     );
                }
            }

            finally
            {
                ChargerProtocolStore.Remove(_chargePointId);
                //CanonicalSessionStore.Remove(_chargePointId, _evseId);
                HeartbeatStore.Remove(_chargePointId);
                ChargerConnectionManager.Remove(_chargePointId);
                if (!gracefulClose)
                {
                    Console.WriteLine($"Charger {_chargePointId} disconnected unexpectedly");

                    var state = ChargerStateStore.Get(_chargePointId);
                    await RabbitMqEventPublisher.PublishAsync(
                        "event.charger.faulted",
                        new
                        {
                            ChargerId = _chargePointId,
                            FaultCode = "PowerLoss",
                            Timestamp = DbTime.From(DateTime.Now)
                        }
                    );
                    
                    //var session = CanonicalSessionStore.GetOrCreate(_chargePointId, ChargerProtocolStore.Get(_chargePointId), _evseId);
                    //session.Active = false;
                }

                try
                {
                    if (_socket.State != WebSocketState.Closed)
                    {
                        await _socket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Connection closed",
                            CancellationToken.None);
                    }
                }
                catch { }
            }

        }
    }
}
