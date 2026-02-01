using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201.Handlers
{
    public static class HeartbeatHandler
    {
        public static async Task Handle(
            string messageId, string chargePointId, WebSocket socket)
        {
            HeartbeatStore.Update(chargePointId);
            var response = new object[]
            {
                3,
                messageId,
                new { currentTime = DateTime.Now }
            };

            await socket.SendAsync(
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response)),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
