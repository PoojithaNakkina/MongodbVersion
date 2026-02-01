//using esyasoft.mobility.CHRGUP.service.api.Data.DTOs.Location;
using esyasoft.mobility.CHRGUP.service.api.DTOs.Location;
using esyasoft.mobility.CHRGUP.service.core.Models;

namespace esyasoft.mobility.CHRGUP.service.api.Interfaces
{
    public interface ILocationService
    {
        Task<Location> CreateAsync(CreateLocationDto request);
        Task<List<Location>> GetAllAsync();
        Task<Location?> GetByIdAsync(string id);
        Task<Location?> UpdateAsync(string id, UpdateLocationDto request);
        Task<bool> DeleteAsync(string id);
        Task<List<object>> GetAllWithChargersAsync();
        Task<List<Location>> SearchAsync(string name);
        Task<object?> GetLocationWithChargersAsync(string id);
    }
}
