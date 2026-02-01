namespace esyasoft.mobility.CHRGUP.service.api.DTOs.Driver
{
    public class CreateDriverDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? VehicleId { get; set; }
    }
}
