namespace esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents
{
    public class ChargerFaultEvent
    {
        public string ChargerId { get; init; }
        public string FaultCode { get; init; }
        public DateTime Timestamp { get; init; }
    }
}
