using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16
{
    public class Ocpp16Message
    {
        public static async Task SendAuthorizeResult(
    WebSocket socket,
    string messageId,
    bool accepted)
        {
            var json = JsonSerializer.Serialize(new object[]
            {
        3,
        messageId,
        new
        {
            idTagInfo = new
            {
                status = accepted ? "Accepted" : "Rejected"
            }
        }
            });

            await socket.SendAsync(
                Encoding.UTF8.GetBytes(json),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
