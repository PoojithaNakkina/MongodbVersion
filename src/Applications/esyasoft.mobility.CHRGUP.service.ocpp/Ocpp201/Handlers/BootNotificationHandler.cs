using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using esyasoft.mobility.CHRGUP.service.ocpp.WebSockets;
using System.Data;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;


using esyasoft.mobility.CHRGUP.service.core.Helpers;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201.Handlers
{
    
    public static class BootNotificationHandler
    {
        public static async Task Handle(
            string chargePointId,
            string messageId,
            WebSocket socket)
        {
            var state = ChargerStateStore.Get(chargePointId);
            state.IsFaulted = false;
            HeartbeatStore.Update(chargePointId);
            ChargerProtocolStore.Set(chargePointId, OcppProtocol.V201);
            ChargerProtocolStore.MarkBooted(chargePointId);

            //
            await RabbitMqEventPublisher.PublishAsync(
            "event.charger.connected",
            new
            {
            ChargerId = chargePointId,
            Protocol = "2.0.1",
                //Timestamp = DateTime.Now
                Timestamp = DbTime.From(DateTime.Now)
            });
            //

            var response = new object[]
            {
                3,
                messageId,
                new
                {
                    status = "Accepted",
                    currentTime = DateTime.Now,
                    interval = 5
                }
            };



            var json = JsonSerializer.Serialize(response);
            var bytes = Encoding.UTF8.GetBytes(json);

            await socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );

            
        }
    }

}
