using esyasoft.mobility.CHRGUP.service.api.DTOs.Charger;
using esyasoft.mobility.CHRGUP.service.api.DTOs.Session;

namespace esyasoft.mobility.CHRGUP.service.api.Interfaces
{
    public interface IChargingSessionService
    {
        Task<ChargingSessionResponseDto> GetByIdAsync(string sessionId);
        Task<List<ChargingSessionResponseDto>> GetByChargerAsync(string chargerId);
        Task<string> StartSessionAsync(string chargerId, string driverId);
        Task StopSessionAsync(string sessionId);

    }
}
