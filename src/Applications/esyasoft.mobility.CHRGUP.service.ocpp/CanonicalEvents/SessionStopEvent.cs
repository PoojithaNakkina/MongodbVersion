namespace esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents
{
    public class SessionStopEvent
    {
        public string SessionId { get; init; }
        public string ChargerId { get; init; }
        public DateTime StopTime { get; init; }
        public double EnergyConsumedKwh { get; init; }
        public double SOC { get; init; }
        public string TriggerReason { get; init; }
    }
}
