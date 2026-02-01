


using esyasoft.mobility.CHRGUP.service.api.DTOs.Vehicle;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using MongoDB.Driver;
using NanoidDotNet;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly MongoDbContext _db;

        public VehicleService(MongoDbContext db)
        {
            _db = db;
        }

        public async Task<VehicleResponseDto> CreateAsync(CreateVehicleDto dto)
        {
            var vinExists = await _db.Vehicles
                .Find(v => v.VIN == dto.VIN)
                .AnyAsync();

            if (vinExists)
                throw new InvalidOperationException("Vehicle with this VIN already exists");

            var regExists = await _db.Vehicles
                .Find(v => v.RegistrationNumber == dto.RegistrationNumber)
                .AnyAsync();

            if (regExists)
                throw new InvalidOperationException(
                    "Vehicle with this registration number already exists");

            var vehicle = new Vehicle
            {
                Id = Nanoid.Generate(size: 10),
                VehicleName = dto.VehicleName,
                VIN = dto.VIN,
                RegistrationNumber = dto.RegistrationNumber,
                MakeandModel = dto.MakeandModel,
                RangeKm = dto.RangeKm,
                BatteryCapacityKwh = dto.BatteryCapacityKwh,
                MaxChargeRateKw = dto.MaxChargeRateKw
            };

            await _db.Vehicles.InsertOneAsync(vehicle);

            return await MapAsync(vehicle);
        }

        public async Task<List<VehicleResponseDto>> GetAllAsync()
        {
            var vehicles = await _db.Vehicles
                .Find(_ => true)
                .ToListAsync();

            var result = new List<VehicleResponseDto>();

            foreach (var v in vehicles)
                result.Add(await MapAsync(v));

            return result;
        }

        public async Task<VehicleResponseDto> GetByIdAsync(string vehicleId)
        {
            var vehicle = await _db.Vehicles
                .Find(v => v.Id == vehicleId)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Vehicle not found");

            return await MapAsync(vehicle);
        }

        public async Task DeleteAsync(string vehicleId)
        {
            var vehicle = await _db.Vehicles
                .Find(v => v.Id == vehicleId)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Vehicle not found");

            var isAssigned = await _db.Drivers
                .Find(d => d.VehicleId == vehicleId)
                .AnyAsync();

            if (isAssigned)
                throw new InvalidOperationException("Vehicle is assigned to a driver");

            await _db.Vehicles.DeleteOneAsync(v => v.Id == vehicleId);
        }

        private async Task<VehicleResponseDto> MapAsync(Vehicle v)
        {
            var driver = await _db.Drivers
                .Find(d => d.VehicleId == v.Id)
                .FirstOrDefaultAsync();

            return new VehicleResponseDto
            {
                VehicleId = v.Id,
                DriverId = driver?.Id,
                VehicleName = v.VehicleName,
                Make = ExtractMake(v.MakeandModel),
                Model = ExtractModel(v.MakeandModel),
                Variant = ExtractVariant(v.MakeandModel),
                RegistrationNumber = v.RegistrationNumber,
                VIN = v.VIN,
                RangeKm = v.RangeKm,
                BatteryCapacityKwh = v.BatteryCapacityKwh,
                MaxChargeRateKw = v.MaxChargeRateKw
            };
        }

        private static string ExtractMake(string value) =>
            value.Split(' ').FirstOrDefault() ?? "";

        private static string ExtractModel(string value) =>
            value.Split(' ').Skip(1).FirstOrDefault() ?? "";

        private static string ExtractVariant(string value) =>
            value.Split(' ').Skip(2).FirstOrDefault() ?? "";
    }
}
 