using System.Net.WebSockets;
using System.Text;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201
{
    public class Ocpp201Message
    {
        public static async Task SendAuthorizeResult(WebSocket socket,string messageId,bool accepted)
        {
            var json = OcppMessage.CreateCallResult(messageId, new
            {
                idTokenInfo = new
                {
                    status = accepted ? "Accepted" : "Rejected"
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
