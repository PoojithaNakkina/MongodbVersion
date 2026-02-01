using esyasoft.mobility.CHRGUP.service.api.DTOs.Charger;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;

namespace esyasoft.mobility.CHRGUP.service.api.Interfaces
{
    public interface IChargerService
    {
        Task<List<Charger>> GetAllAsync();
        Task<Charger> RegisterAsync(string locationId, string version);
        Task UpdateStatusAsync(string chargerId, ChargerStatus status);
        Task UpdateHeartbeatAsync(string chargerId, DateTime timestamp);

        //Task RemoteStartAsync(string chargerId, StartChargingRequestDto dto);
        //Task RemoteStopAsync(string chargerId, StopChargingRequestDto dto);
    }
}

