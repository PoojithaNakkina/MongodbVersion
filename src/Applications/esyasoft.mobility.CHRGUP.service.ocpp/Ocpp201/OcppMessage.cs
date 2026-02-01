using System.Text.Json;
namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201
{
    public class OcppMessage
    {
        public int MessageType { get; set; }
        public string MessageId { get; set; }
        public string Action { get; set; }
        public JsonElement Payload { get; set; }

        public static OcppMessage Parse(string json)
        {
            var arr = JsonSerializer.Deserialize<JsonElement[]>(json);

            return new OcppMessage
            {
                MessageType = arr[0].GetInt32(),
                MessageId = arr[1].GetString(),
                Action = arr[2].GetString(),
                Payload = arr[3]
            };
        }

        public static string CreateCallResult(string messageId, object payload)
        {
            return JsonSerializer.Serialize(new object[]
            {
                3,
                messageId,
                payload
            });
        }

        public static string CreateCall(string messageId, string action, object payload)
        {
            return JsonSerializer.Serialize(new object[]
            {
                2,
                messageId,
                action,
                payload
            });
        }
    }
}
