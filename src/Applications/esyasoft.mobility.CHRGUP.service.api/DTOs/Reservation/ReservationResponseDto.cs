namespace esyasoft.mobility.CHRGUP.service.api.DTOs.Reservation
{
    public class ReservationResponseDto
    {
        public string Id { get; set; }
        public string ChargerId { get; set; }
        public string DriverId { get; set; }
        public int ConnectorId { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
