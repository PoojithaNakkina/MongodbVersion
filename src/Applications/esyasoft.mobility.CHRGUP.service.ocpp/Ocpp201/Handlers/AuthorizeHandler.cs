using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201.Handlers
{
    public static class AuthorizeHandler
    {
        public static async Task Handle(
            string messageId,
        JsonElement payload,
        //int evseId,
        string chargePointId,
        WebSocket socket)
        {
            var vin = payload
       .GetProperty("idToken")
       .GetProperty("value")
       .GetString();


            if (!payload.TryGetProperty("evseId", out var evseEl) ||
            evseEl.ValueKind != JsonValueKind.Number)
            {
                Console.WriteLine("Invalid Authorize payload: missing or invalid evseId");
                return;
            }



            var evseId = evseEl.GetInt32();

            Console.WriteLine($"VIN authorization request: {vin}");

            //
            AuthRequestStore.Register(messageId, chargePointId,evseId, socket);
            //
            //VinAuthorizationStore.Register(messageId, socket);
            //

            await RabbitMqEventPublisher.PublishAsync(
                //"vin.authorization.request",
                "event.authorization.request",
                new
                {
                    //MessageId = messageId,
                    //ChargerId = chargePointId,
                    //Vin = vin,
                    //Timestamp = DateTime.Now
                    ChargerId = chargePointId,
                    Token = vin,
                    TokenType = "VIN",
                    MessageId = messageId
                }
            );


    //        var accepted = await VinAuthorizationStore.Wait(messageId);

    //        var responsePayload = new
    //        {
    //            idTokenInfo = new
    //            {
    //                status = accepted ? "Accepted" : "Rejected"
    //            }
    //        };

    //        var json = OcppMessage.CreateCallResult(messageId, responsePayload);
    //        var bytes = Encoding.UTF8.GetBytes(json);

    //        await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
