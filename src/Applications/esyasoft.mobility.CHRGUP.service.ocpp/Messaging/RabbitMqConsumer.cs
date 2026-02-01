using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201;
using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using esyasoft.mobility.CHRGUP.service.ocpp.WebSockets;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Messaging
{
    public class RabbitMqConsumer
    {
        private readonly IChannel _channel;

        public RabbitMqConsumer(IChannel channel)
        {
            _channel = channel;
        }

        public void Start()
        {
            const string exchangeName = "charging_commands_ex";
            const string queueName = "charging.commands";

            _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Direct,
                durable: true
            );

            _channel.QueueDeclareAsync(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false
            );

            _channel.QueueBindAsync(
        queue: queueName,
        exchange: exchangeName,
        routingKey: "event.authorization.result"
        );

            //_channel.QueueBindAsync(
            //queue: queueName,
            //exchange: exchangeName,
            //routingKey: "vin.authorization.result"
            //);

            //_channel.QueueBindAsync(
            //queue: queueName,
            //exchange: exchangeName,
            //routingKey: "rfid.authorization.result"
            //);

            //_channel.QueueBindAsync(
            //    queue: queueName,
            //    exchange: exchangeName,
            //    routingKey: "authorization.result"
            //);



            _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: "command.start"
            );

            _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: "command.stop"
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += HandleMessage;

            _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: true,
                consumer: consumer
            );
        }

        private async Task HandleMessage(object sender, BasicDeliverEventArgs ea)
        {
            //var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            //var root = JsonDocument.Parse(json).RootElement;
            string json;
            JsonElement root;
            try
            {
                json = Encoding.UTF8.GetString(ea.Body.ToArray());
                root = JsonDocument.Parse(json).RootElement;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid RabbitMQ message: {ex.Message}");
                return;
            }

            var routingKey = ea.RoutingKey;

            var evseId = root.TryGetProperty("EvseId", out var e) ? e.GetInt32() : 1;


            //if (routingKey == "vin.authorization.result")
            //{
            //    var messageId = root.GetProperty("MessageId").GetString();
            //    var accepted = root.GetProperty("Accepted").GetBoolean();


            //    if (!AuthRequestStore.TryTake(messageId!, out var auth))
            //        return;

            //    var chargerIdvin = auth.ChargerId;
            //    var protocolvin = ChargerProtocolStore.Get(chargerIdvin);

            //    var socketvin = ChargerConnectionManager.GetSocket(chargerIdvin);
            //    if (socketvin == null || socketvin.State != WebSocketState.Open)
            //        return;

            //    if (protocolvin == OcppProtocol.V201)
            //    {
            //        await Ocpp201Message.SendAuthorizeResult(socketvin, messageId!, accepted);
            //    }
            //    else
            //    {
            //        await Ocpp16Message.SendAuthorizeResult(socketvin, messageId!, accepted);
            //    }
            //    Console.WriteLine($"VIN AUTH RESULT: {messageId} is {(accepted ? "ACCEPTED" : "REJECTED")}");
            //    return;
            //}
            //if (routingKey == "rfid.authorization.result")
            //{
            //    var messageId = root.GetProperty("MessageId").GetString();
            //    var accepted = root.GetProperty("Accepted").GetBoolean();

            //    if (!AuthRequestStore.TryTake(messageId!, out var auth))
            //        return;

            //    var chargerIdRFID = auth.ChargerId;
            //    var protocolRFID = ChargerProtocolStore.Get(chargerIdRFID);
            //    var socketRFID = ChargerConnectionManager.GetSocket(chargerIdRFID);

            //    if (protocolRFID == OcppProtocol.V16)
            //    {
            //        await Ocpp16Message.SendAuthorizeResult(socketRFID, messageId!, accepted);
            //    }

            //    Console.WriteLine($"RFID AUTH RESULT: {messageId} is {(accepted ? "ACCEPTED" : "REJECTED")}");
            //    return;
            //}

            if (routingKey == "event.authorization.result")
            {
                var messageId = root.GetProperty("MessageId").GetString();
                var accepted = root.GetProperty("Accepted").GetBoolean();

                if (!AuthRequestStore.TryTake(messageId!, out var auth))
                    return;

                var chargerId_ = auth.ChargerId;
                var protocol_ = ChargerProtocolStore.Get(chargerId_);
                var socket_ = ChargerConnectionManager.GetSocket(chargerId_);

                if (socket_ == null || socket_.State != WebSocketState.Open)
                    return;

                if (protocol_ == OcppProtocol.V201)
                    await Ocpp201Message.SendAuthorizeResult(socket_, messageId!, accepted);
                else
                    await Ocpp16Message.SendAuthorizeResult(socket_, messageId!, accepted);

                Console.WriteLine($"AUTH RESULT ({routingKey}): {messageId} is {(accepted ? "ACCEPTED" : "REJECTED")}");
                return;
            }




            var chargerId = root.GetProperty("ChargerId").GetString();
            var sessionId = root.GetProperty("SessionId").GetString();
            var userId = root.TryGetProperty("DriverId", out var u)
                ? u.GetString()
                : null;

            if (!ChargerProtocolStore.HasBooted(chargerId))
            {
                Console.WriteLine($"Command sent to non-booted charger {chargerId}");
                return;
            }


            if (chargerId == null || sessionId == null)
                return;

            var socket = ChargerConnectionManager.GetSocket(chargerId);
            if (socket == null || socket.State != WebSocketState.Open)
                return;

            var protocol = ChargerProtocolStore.Get(chargerId);


            //
            if (routingKey == "command.start")
            {
                //var canonical = CanonicalSessionStore.GetOrCreate(chargerId, protocol, evseId);
                //canonical.SessionId = sessionId;
                //canonical.UserId = userId;

                await SendStart(protocol, socket, chargerId, sessionId, userId, evseId);
            }
                //await SendStart(protocol, socket, chargerId, sessionId, userId);

            else if (routingKey == "command.stop")
            {
                //var canonical = CanonicalSessionStore.GetOrCreate(chargerId, protocol);
                //await SendStop(protocol, socket, chargerId, canonical.SessionId);

                //var session = CanonicalSessionStore.GetOrCreate(chargerId, protocol, evseId);
                //if (!session.Active || session.SessionId == null)
                //{
                //    Console.WriteLine("No active session to stop");
                //    return;
                //}

                await SendStop(protocol, socket, chargerId, sessionId, evseId);

            }
            //

            //switch (routingKey)
            //{
            //    case "command.start":
            //        await HandleStartCommand(socket, chargerId, sessionId, userId);
            //        break;

            //    case "command.stop":
            //        await HandleStopCommand(socket, chargerId, sessionId);
            //        break;

            //    default:
            //        Console.WriteLine($"Unknown routing key: {routingKey}");
            //        break;
            //}
        }

        private async Task SendStart(OcppProtocol protocol, WebSocket socket, string chargerId, string sessionId, string? userId, int evseId)
        {
            Console.WriteLine($"[START {protocol}] Session={sessionId}");

            if (protocol == OcppProtocol.V201)
            {

                var state = ChargerStateStore.Get(chargerId);
                if (state.IsFaulted)
                {
                    Console.WriteLine("Cannot start: charger is faulted");
                    return;
                }

                //var session = CanonicalSessionStore.GetOrCreate(chargerId, protocol, evseId);
                //if (session.Active)
                //{
                //    Console.WriteLine("Cannot start: session already active");
                //    return;
                //}

                await SendOcppCommand(socket, "RequestStartTransaction", new
                {
                    sessionId,
                    userId
                });
            }
            else
            {
                var state = ChargerStateStore.Get(chargerId);
                if (state.IsFaulted)
                {
                    Console.WriteLine("Cannot start: charger is faulted");
                    return;
                }

                //var session = CanonicalSessionStore.GetOrCreate(chargerId, protocol, evseId);
                //if (session.Active)
                //{
                //    Console.WriteLine("Cannot start: session already active");
                //    return;
                //}

                await SendOcppCommand(socket, "RemoteStartTransaction", new
                {
                    idTag = userId,
                    connectorId = 1
                });
            }
        }

        private async Task SendStop(OcppProtocol protocol, WebSocket socket, string chargerId, string sessionId, int evseId)
        {
            Console.WriteLine($"[STOP {protocol}] Session={sessionId}");

            if (protocol == OcppProtocol.V201)
            {
                //var session = CanonicalSessionStore.GetOrCreate(chargerId, protocol, evseId);
                //if (!session.Active || session.SessionId == null)
                //{
                //    Console.WriteLine("No active session to stop");
                //    return;
                //}

                await SendOcppCommand(socket, "RequestStopTransaction", new
                {
                    sessionId
                });
            }
            else
            {
                //var session = CanonicalSessionStore.GetOrCreate(chargerId, protocol, evseId);
                //if (!session.Active || session.SessionId == null)
                //{
                //    Console.WriteLine("No active session to stop");
                //    return;
                //}


                await SendOcppCommand(socket, "RemoteStopTransaction", new
                {
                    transactionId = sessionId
                });
            }
        }




        //private async Task HandleStartCommand(
        //    WebSocket socket,
        //    string chargerId,
        //    string sessionId,
        //    string? userId)
        //{
        //    Console.WriteLine($"[START] Session={sessionId}");

        //    var state = ChargerStateStore.Get(chargerId);
        //    state.ActiveSessionId = sessionId;

        //    await SendOcppCommand(
        //        socket,
        //        "RequestStartTransaction",
        //        new
        //        {
        //            sessionId,
        //            userId
        //        });
        //}


        //private async Task HandleStopCommand(
        //    WebSocket socket,
        //    string chargerId,
        //    string sessionId)
        //{
        //    Console.WriteLine($"[STOP] Session={sessionId}");

        //    var state = ChargerStateStore.Get(chargerId);
        //    state.ActiveSessionId = null;

        //    await SendOcppCommand(
        //        socket,
        //        "RequestStopTransaction",
        //        new
        //        {
        //            sessionId
        //        });
        //}

        private static async Task SendOcppCommand(
            WebSocket socket,
            string action,
            object payload)
        {
            var message = new object[]
            {
                2,
                Guid.NewGuid().ToString(),
                action,
                payload
            };

            var bytes = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(message));

            await socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
    


