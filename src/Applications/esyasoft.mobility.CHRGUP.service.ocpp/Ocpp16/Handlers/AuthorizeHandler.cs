using System.Text.Json;
using System.Net.WebSockets;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using System.Text;


namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16.Handlers
{
    public class AuthorizeHandler
    {
        public static async Task Handle(
            JsonElement payload,
            string chargerId,
            string messageId,
            WebSocket socket)
        {
            var rfid = payload.GetProperty("idTag").GetString();
            var evseId = payload.GetProperty("evseId").GetInt32();
            //
            AuthRequestStore.Register(messageId, chargerId, 1, socket);
            //

            //VinAuthorizationStore.Register(messageId, socket);
            //
            await RabbitMqEventPublisher.PublishAsync(
                //"rfid.authorization.request",
                "authorization.request",
                new
                {
                    ChargerId = chargerId,
                    Token = rfid,
                    TokenType = "RFID",
                    MessageId = messageId
                });
        }
    }
}
