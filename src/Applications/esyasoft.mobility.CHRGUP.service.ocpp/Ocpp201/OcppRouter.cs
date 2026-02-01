using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201.Handlers;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using System.Net.WebSockets;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201
{
  
    public static class OcppRouter
    {
        public static async Task RouteAsync(
            string json,
            string chargePointId,
            string tenantId,
            WebSocket socket,
            MongoDbContext db)
        {
            var message = OcppMessage.Parse(json);

            switch (message.Action)
            {
                case "BootNotification":
                    await BootNotificationHandler.Handle(
                        chargePointId,
                        message.MessageId,
                        socket);
                    break;

                case "Authorize":
                    await AuthorizeHandler.Handle(
                        message.MessageId,
                        //payload,
                        message.Payload,
                        chargePointId,
                        socket);
                    break;

                case "Heartbeat":
                    await HeartbeatHandler.Handle(
                        message.MessageId,
                        chargePointId,
                        socket);
                    break;

                case "TransactionEvent":
                    await TransactionEventHandler.Handle(
                        message.Payload,
                        chargePointId);
                    break;

                case "MeterValues":
                    await MeterValuesHandler.Handle(
                        message.Payload,
                        chargePointId);
                    break;

                case "StatusNotification":
                    Console.WriteLine("redirected to statnot handler--v201");
                    await StatusNotificationHandler.Handle(
                        message.Payload,
                        chargePointId,
                        db);
                    break;
            }
        }

    }

    }
