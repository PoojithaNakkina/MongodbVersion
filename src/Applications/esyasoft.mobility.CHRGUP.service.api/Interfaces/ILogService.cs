using esyasoft.mobility.CHRGUP.service.api.DTOs.Log;

namespace esyasoft.mobility.CHRGUP.service.api.Interfaces
{
    public interface ILogService
    {
        Task<List<LogResponseDto>> GetAllAsync();
        Task<LogResponseDto> GetByIdAsync(Guid id);
        Task<List<LogResponseDto>> GetBySessionIdAsync(string sessionId);
        Task<List<LogResponseDto>> GetByChargerIdAsync(string chargerId);
        Task<List<LogResponseDto>> GetByDriverIdAsync(string driverId);
    }
}
