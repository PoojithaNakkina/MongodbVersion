
namespace esyasoft.mobility.CHRGUP.service.ocpp.State
{
    public class ChargerState
    {
        public string ChargePointId { get; set; }
        //
        public string OcppVersion { get; set; } = "2.0.1";
        //
        public bool IsConnected { get; set; }
        //public string? ActiveSessionId { get; set; }
        //public string? ActiveTransactionId { get; set; }
        public bool IsFaulted { get; set; }
        public DateTime LastSeenUtc { get; internal set; }
    }

}
