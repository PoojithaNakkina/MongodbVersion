using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16.Handlers
{
    public class HeartbeatHandler
    {
        public static async Task Handle(
            string chargerId,
            string messageId,
            WebSocket socket)
        {
            HeartbeatStore.Update(chargerId);

            var response = new object[]
            {
                3,
                messageId,
                new
                {
                    currentTime = DateTime.Now
                }
            };

            await socket.SendAsync(
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response)),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
