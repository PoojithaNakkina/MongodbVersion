//using esyasoft.mobility.CHRGUP.service.api.Data.DTOs.Vehicle;
using esyasoft.mobility.CHRGUP.service.api.DTOs.Vehicle;

namespace esyasoft.mobility.CHRGUP.service.api.Interfaces
{
    public interface IVehicleService
    {
        Task<VehicleResponseDto> CreateAsync(CreateVehicleDto dto);
        Task<List<VehicleResponseDto>> GetAllAsync();
        Task<VehicleResponseDto> GetByIdAsync(string vehicleId);
        Task DeleteAsync(string vehicleId);
    }
}
