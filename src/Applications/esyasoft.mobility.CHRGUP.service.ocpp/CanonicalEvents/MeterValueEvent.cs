namespace esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents
{
    public class MeterValueEvent
    {
        public string SessionId { get; init; }
        public string ChargerId { get; init; }
        public DateTime Timestamp { get; init; }
        public double EnergyKwh { get; init; }
        public double SOC { get; init; }
    }
}
