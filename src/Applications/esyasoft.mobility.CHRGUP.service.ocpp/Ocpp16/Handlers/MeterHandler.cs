using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16.Handlers
{
    public class MeterHandler
    {
        public static async Task Handle(JsonElement payload, string chargerId)
        {


            //var session = CanonicalSessionStore.GetOrCreate(chargerId, OcppProtocol.V16,1);
            //if (!session.Active || session.SessionId == null)
            //    return;

            double energy = 0;
            double soc = 0;
            var sessionId = payload
            .GetProperty("transactionId")
            .GetInt32()
            .ToString();

            foreach (var sample in payload.EnumerateArray())
                {
                var value = double.Parse(sample.GetProperty("value").GetString());
                var measurand = sample.TryGetProperty("measurand", out var m)
                    ? m.GetString()
                    : null;


                if (measurand == "Energy.Active.Import.Register" || measurand == null)
                {
                    energy = value;
                }
                if (measurand == "SoC")
                {
                    soc = value;
                }
            }


            var ev = new MeterValueEvent
            {
                ChargerId = chargerId,
                SessionId = sessionId,
                Timestamp = DateTime.Now,
                EnergyKwh = energy,
                SOC = soc
            };

            //CanonicalSessionReducer.ReduceMeterValue(ev, OcppProtocol.V16, 1);

            await RabbitMqEventPublisher.PublishAsync("event.meter.value",ev);


           
        }
    }
}
