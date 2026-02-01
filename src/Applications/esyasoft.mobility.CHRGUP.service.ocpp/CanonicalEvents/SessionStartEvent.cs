namespace esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents
{
    public class SessionStartEvent
    {
        public string SessionId { get; init; }
        public string ChargerId { get; init; }
        public string UserId { get; init; }
        public DateTime StartTime { get; init; }
        public double SOC { get; init; }
    }
}
