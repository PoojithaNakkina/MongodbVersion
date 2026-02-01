//using esyasoft.mobility.CHRGUP.service.core.Helpers;
//using esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents;
//using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
//using esyasoft.mobility.CHRGUP.service.ocpp.State;
//using System.Text.Json;

//namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201.Handlers
//{
//    public static class TransactionEventHandler
//    {


//        public static async Task Handle(
//            JsonElement payload,
//            string chargePointId)
//        {
//            HeartbeatStore.Update(chargePointId);
//            var eventType = payload.GetProperty("eventType").GetString();
//            var timestamp = DbTime.From(payload.GetProperty("timestamp").GetDateTime());
//            var triggerReason = payload.GetProperty("triggerReason").GetString();

//            var transactionInfo = payload.GetProperty("transactionInfo");
//            var sessionId = transactionInfo
//                .GetProperty("sessionId")
//                .GetString();
//            var userId = transactionInfo.GetProperty("userId").GetString();



//            //
//            //var evseId = payload.GetProperty("evse").GetProperty("id").GetInt32();
//            //var vin = payload.GetProperty("idTag").GetString();
//            //

//            var soc = transactionInfo.TryGetProperty("soc", out var s)
//              ? s.GetDouble()
//              : 0;

//            //string? userId = null;
//            //if (payload.TryGetProperty("idToken", out var token))
//            //{
//            //    userId = token.GetProperty("value").GetString();
//            //}



//            if (eventType == "Started")
//            {
//                var TransactionStart = new SessionStartEvent
//                {
//                    SessionId = sessionId!,
//                    ChargerId = chargePointId,
//                    UserId = userId,
//                    StartTime = timestamp,
//                    SOC = soc
//                };


//                await RabbitMqEventPublisher.PublishAsync("event.session.started",TransactionStart);
//            }
//            else if (eventType == "Ended")
//            {
//                Console.WriteLine("stop event received at ocpp");
//                double energy = 0;
//                if (payload.TryGetProperty("meterStop", out var meterStop))
//                {
//                    energy = meterStop.GetDouble();
//                }

//                var TransactionStop = new SessionStopEvent
//                {
//                    SessionId = sessionId,
//                    ChargerId = chargePointId,
//                    StopTime = DateTime.Now,
//                    EnergyConsumedKwh = energy,
//                    TriggerReason = triggerReason,
//                    SOC = soc
//                };




//                await RabbitMqEventPublisher.PublishAsync("event.session.stopped",TransactionStop);


//            }
//        }
//    }
//}


using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201.Handlers
{
    public static class TransactionEventHandler
    {
        public static async Task Handle(
            JsonElement payload,
            string chargePointId)
        {
            HeartbeatStore.Update(chargePointId);

            var eventType = payload.GetProperty("eventType").GetString();
            var timestamp = DbTime.From(
                payload.GetProperty("timestamp").GetDateTime()
            );

            var triggerReason = payload
                .GetProperty("triggerReason")
                .GetString();

            var transactionInfo = payload.GetProperty("transactionInfo");

            var sessionId = transactionInfo
                .GetProperty("sessionId")
                .GetString();

            var userId = transactionInfo
                .GetProperty("userId")
                .GetString();

            var soc = transactionInfo.TryGetProperty("soc", out var s)
                ? s.GetDouble()
                : 0;

            if (eventType == "Started")
            {
                var transactionStart = new SessionStartEvent
                {
                    SessionId = sessionId!,
                    ChargerId = chargePointId,
                    UserId = userId,
                    StartTime = timestamp,
                    SOC = soc
                };

                await RabbitMqEventPublisher.PublishAsync(
                    "event.session.started",
                    transactionStart
                );
            }
            else if (eventType == "Ended")
            {
                Console.WriteLine(
                    $"TransactionEvent.Ended received, triggerReason={triggerReason}"
                );

                // 🔒 GUARD: DO NOT NORMAL-STOP FAULTED SESSIONS
                if (triggerReason == "Fault" ||
                    triggerReason == "EmergencyStop" ||
                    triggerReason == "PowerLoss")
                {
                    Console.WriteLine(
                        $"Skipping normal stop for faulted session {sessionId}"
                    );
                    return;
                }

                double energy = 0;
                if (payload.TryGetProperty("meterStop", out var meterStop))
                {
                    energy = meterStop.GetDouble();
                }

                var transactionStop = new SessionStopEvent
                {
                    SessionId = sessionId!,
                    ChargerId = chargePointId,
                    StopTime = DateTime.Now,
                    EnergyConsumedKwh = energy,
                    TriggerReason = triggerReason,
                    SOC = soc
                };

                await RabbitMqEventPublisher.PublishAsync(
                    "event.session.stopped",
                    transactionStop
                );
            }
        }
    }
}


