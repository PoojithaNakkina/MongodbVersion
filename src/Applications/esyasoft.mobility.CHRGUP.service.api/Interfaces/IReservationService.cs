using esyasoft.mobility.CHRGUP.service.api.DTOs.Reservation;

namespace esyasoft.mobility.CHRGUP.service.api.Interfaces
{
    public interface IReservationService
    {
        Task<ReservationResponseDto> CreateAsync(CreateReservationDto dto);
        Task<ReservationResponseDto?> CancelAsync(string id, CancelReservationDto dto);
        Task<List<ReservationResponseDto>> GetAllAsync();
    }
}
