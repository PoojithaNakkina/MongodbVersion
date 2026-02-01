namespace esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents
{
    public class CanonicalAuthEvent
    {
        public string ChargerId { get; init; }
        public string Token { get; init; }
        public string TokenType { get; init; }
        public string MessageId { get; init; }
    }
}
