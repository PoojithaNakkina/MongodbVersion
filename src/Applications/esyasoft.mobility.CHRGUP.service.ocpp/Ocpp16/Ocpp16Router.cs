using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16.Handlers;
using System.Net.WebSockets;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16
{
    public class Ocpp16Router
    {
        public static async Task RouteAsync(
            string json,
            string chargerId,
            string tenantId,
            WebSocket socket)
        {
            var msg = JsonDocument.Parse(json).RootElement;
            var action = msg[2].GetString();
            var payload = msg[3];
            var messageId = msg[1].GetString();

            switch (action)
            {
                case "BootNotification":
                    await BootHandler.Handle(chargerId, messageId, socket);
                    break;

                case "Authorize":
                    await AuthorizeHandler.Handle(payload, chargerId, messageId, socket);
                    break;

                case "Heartbeat":
                    await HeartbeatHandler.Handle(chargerId, messageId, socket);
                    break;

                case "StartTransaction":
                    await TransactionHandler.HandleStart(payload, chargerId);
                    break;

                case "StopTransaction":
                    await TransactionHandler.HandleStop(payload, chargerId);
                    break;

                case "MeterValues":
                    await MeterHandler.Handle(payload, chargerId);
                    break;

                case "StatusNotification":
                    await StatusNotificationHandler.Handle(payload, chargerId);
                    break;
            }
        }
    }
}
