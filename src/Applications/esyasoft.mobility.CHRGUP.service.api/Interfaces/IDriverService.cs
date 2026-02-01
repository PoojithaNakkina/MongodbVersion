//using esyasoft.mobility.CHRGUP.service.api.Data.DTOs.Driver;
using esyasoft.mobility.CHRGUP.service.api.DTOs.Driver;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;


namespace esyasoft.mobility.CHRGUP.service.api.Interfaces
{
    public interface IDriverService
    {
        Task<DriverResponseDto> CreateAsync(CreateDriverDto dto);
        Task<List<DriverResponseDto>> GetAllAsync();
        Task<DriverResponseDto> GetByIdAsync(string id);
        Task UpdateStatusAsync(string id, DriverStatus status);
        Task<Driver> AssignVehicleAsync(string driverId, string vehicleId);
    }
}
