using esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16.Handlers
{
    public class TransactionHandler
    {
        public static async Task HandleStart(JsonElement payload, string chargerId)
        {
            var rfid = payload.GetProperty("idTag").GetString();
            var txId = payload.GetProperty("transactionId").GetInt32().ToString();
            var sessionId = payload
           .GetProperty("transactionId")
           .GetInt32()
           .ToString();
            var soc = payload.TryGetProperty("soc", out var s)
            ? s.GetDouble()
            : 0;

            var TransactionStart = new SessionStartEvent
            {
                SessionId = txId,
                ChargerId = chargerId,
                UserId = rfid!,
                StartTime = DateTime.Now,
                SOC = soc
            };

           

            await RabbitMqEventPublisher.PublishAsync("event.session.started", TransactionStart);
            }

        public static async Task HandleStop(JsonElement payload, string chargerId)
        {
            //var session = CanonicalSessionStore.GetOrCreate(chargerId, OcppProtocol.V16,1);
            //session.Active = false;
            var triggerReason = payload.GetProperty("triggerReason").GetString();
            var sessionId = payload
           .GetProperty("transactionId")
           .GetInt32()
           .ToString();
            double energy = 0;
            if (payload.TryGetProperty("meterStop", out var meterStop))
            {
                energy = meterStop.GetDouble();
            }
            var soc = payload.TryGetProperty("soc", out var s)
            ? s.GetDouble()
            : 0;

            //session.Active = false;

            var TransactionStop = new SessionStopEvent
            {
                SessionId = sessionId,
                ChargerId = chargerId,
                StopTime = DateTime.Now,
                EnergyConsumedKwh = energy,
                TriggerReason = triggerReason,
                SOC = soc
            };



            await RabbitMqEventPublisher.PublishAsync("event.session.stopped", TransactionStop);

            //CanonicalSessionStore.Remove(chargerId, 1);
        }
    }
}
