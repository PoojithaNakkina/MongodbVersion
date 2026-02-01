//using DotNetEnv;
//using esyasoft.mobility.CHRGUP.service.core.Helpers;
//using esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents;
//using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
//using esyasoft.mobility.CHRGUP.service.ocpp.State;
//using System.Text.Json;

//namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201.Handlers
//{
//    public static class MeterValuesHandler
//    {
//        public static async Task Handle(
//            JsonElement payload,
//            string chargePointId)
//        {


//            var evseId = payload.GetProperty("evseId").GetInt32();


//            double energy = 0;
//            double soc = 0;
//            var sessionId = payload
//            .GetProperty("transactionId")
//            .ToString();

//            foreach (var meterValue in payload
//                .GetProperty("meterValue")
//                .EnumerateArray())
//            {
//                var timestamp = DbTime.From(meterValue.GetProperty("timestamp").GetDateTime());

//                foreach (var sampledValue in meterValue
//                    .GetProperty("sampledValue")
//                    .EnumerateArray())
//                {
//                    var measurand = sampledValue
//                        .GetProperty("measurand")
//                        .GetString();

//                    if (measurand != "Energy.Active.Import.Register")
//                        continue;

//                    var value = double.Parse(
//                        sampledValue.GetProperty("value").GetString()!
//                    );

//                    if (measurand == "Energy.Active.Import.Register")
//                    {
//                        energy = value;
//                    }
//                    if (measurand == "SoC")
//                    {
//                        soc = value;
//                    }

//                    var meterEvent = new MeterValueEvent
//                    {
//                        ChargerId = chargePointId,
//                        SessionId = sessionId,
//                        Timestamp = timestamp,
//                        EnergyKwh = energy,
//                        SOC = soc
//                    };


//                    await RabbitMqEventPublisher.PublishAsync(
//                        "event.meter.value",
//                        meterEvent
//                    );
//                }
//            }
//        }
//    }
//}



using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using System.Text.Json;

public static class MeterValuesHandler
{
    public static async Task Handle(
        JsonElement payload,
        string chargePointId)
    {
        var sessionId = payload
            .GetProperty("transactionId")
            .ToString();

        foreach (var meterValue in payload
            .GetProperty("meterValue")
            .EnumerateArray())
        {
            var timestamp = DbTime.From(
                meterValue.GetProperty("timestamp").GetDateTime()
            );

            double? energy = null;
            double? soc = null;

            foreach (var sampledValue in meterValue
                .GetProperty("sampledValue")
                .EnumerateArray())
            {
                var measurand = sampledValue
                    .GetProperty("measurand")
                    .GetString();

                var value = double.Parse(
                    sampledValue.GetProperty("value").GetString()!
                );

                if (measurand == "Energy.Active.Import.Register")
                {
                    energy = value;
                }
                else if (measurand == "SoC")
                {
                    soc = value;
                }
            }

            // 🔥 Publish once per timestamp
            if (energy.HasValue || soc.HasValue)
            {
                var meterEvent = new MeterValueEvent
                {
                    ChargerId = chargePointId,
                    SessionId = sessionId,
                    Timestamp = timestamp,
                    EnergyKwh = energy ?? 0,
                    SOC = soc ?? 0
                };

                await RabbitMqEventPublisher.PublishAsync(
                    "event.meter.value",
                    meterEvent
                );
            }
        }
    }
}
