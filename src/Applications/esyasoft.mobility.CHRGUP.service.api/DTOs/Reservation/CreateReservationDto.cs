namespace esyasoft.mobility.CHRGUP.service.api.DTOs.Reservation
{
    public class CreateReservationDto
    {
        public string ChargerId { get; set; }
        public string DriverId { get; set; }
        public int ConnectorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string CreatedBy { get; set; }
    }
}
