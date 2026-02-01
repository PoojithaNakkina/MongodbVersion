namespace esyasoft.mobility.CHRGUP.service.api.DTOs.Session
{
    public class ChargingSessionResponseDto
    {
        public string SessionId { get; set; }
        public string ChargerId { get; set; }
        public string DriverId { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
