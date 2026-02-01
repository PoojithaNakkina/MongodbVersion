//using esyasoft.mobility.CHRGUP.service.persistence.Data;
//using esyasoft.mobility.CHRGUP.service.api.DTOs.Driver;
//using esyasoft.mobility.CHRGUP.service.api.Interfaces;
//using esyasoft.mobility.CHRGUP.service.core.Metadata;
//using esyasoft.mobility.CHRGUP.service.core.Models;
//using Microsoft.EntityFrameworkCore;
//using NanoidDotNet;

//namespace esyasoft.mobility.CHRGUP.service.api.Services
//{
//    public class DriverService : IDriverService
//    {
//        private readonly AppDbContext _db;

//        public DriverService(AppDbContext db)
//        {
//            _db = db;
//        }

//        public async Task<DriverResponseDto> CreateAsync(CreateDriverDto dto)
//        {
//            if (await _db.drivers.AnyAsync(d => d.Email == dto.Email))
//                throw new InvalidOperationException("Driver with this email already exists");

//            if (!string.IsNullOrEmpty(dto.VehicleId))
//            {
//                var vehicleExists = await _db.vehicles
//                    .AnyAsync(v => v.Id == dto.VehicleId);

//                if (!vehicleExists)
//                    throw new InvalidOperationException("Vehicle not found");

//                var alreadyAssigned = await _db.drivers
//                    .AnyAsync(d => d.VehicleId == dto.VehicleId);

//                if (alreadyAssigned)
//                    throw new InvalidOperationException("Vehicle already assigned");
//            }

//            var driver = new Driver
//            {
//                Id = Nanoid.Generate(size: 10),
//                FullName = dto.FullName,
//                Email = dto.Email,
//                Password = dto.Password, 
//                Gender = dto.Gender,
//                DateOfBirth = dto.DateOfBirth,
//                Status = DriverStatus.Active,
//                CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
//                VehicleId = dto.VehicleId
//            };

//            _db.drivers.Add(driver);
//            await _db.SaveChangesAsync();

//            return Map(driver);
//        }

//        public async Task<List<DriverResponseDto>> GetAllAsync()
//        {
//            return await _db.drivers
//                .OrderByDescending(d => d.CreatedAt)
//                .Select(d => Map(d))
//                .ToListAsync();
//        }

//        public async Task<DriverResponseDto> GetByIdAsync(string id)
//        {
//            var driver = await _db.drivers.FirstOrDefaultAsync(d => d.Id == id)
//                ?? throw new KeyNotFoundException("Driver not found");

//            return Map(driver);
//        }

//        public async Task UpdateStatusAsync(string id, DriverStatus status)
//        {
//            var driver = await _db.drivers.FirstOrDefaultAsync(d => d.Id == id)
//                ?? throw new KeyNotFoundException("Driver not found");

//            driver.Status = status;
//            driver.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

//            await _db.SaveChangesAsync();
//        }

//        public async Task AssignVehicleAsync(string driverId, string vehicleId)
//        {
//            var driver = await _db.drivers.FirstOrDefaultAsync(d => d.Id == driverId)
//                ?? throw new KeyNotFoundException("Driver not found");

//            var vehicleExists = await _db.vehicles.AnyAsync(v => v.Id == vehicleId);
//            if (!vehicleExists)
//                throw new InvalidOperationException("Vehicle not found");

//            var alreadyAssigned = await _db.drivers
//                .AnyAsync(d => d.VehicleId == vehicleId && d.Id != driverId);

//            if (alreadyAssigned)
//                throw new InvalidOperationException("Vehicle already assigned");

//            driver.VehicleId = vehicleId;
//            driver.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

//            await _db.SaveChangesAsync();
//        }

//        private static DriverResponseDto Map(Driver d) => new()
//        {
//            Id = d.Id,
//            FullName = d.FullName,
//            Email = d.Email,
//            Status = d.Status.ToString(),
//            CreatedAt = d.CreatedAt
//        };
//    }
//}


using esyasoft.mobility.CHRGUP.service.api.DTOs.Driver;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using MongoDB.Driver;
using NanoidDotNet;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class DriverService : IDriverService
    {
        private readonly MongoDbContext _db;

        public DriverService(MongoDbContext db)
        {
            _db = db;
        }

        // ---------------- CREATE ----------------
        public async Task<DriverResponseDto> CreateAsync(CreateDriverDto dto)
        {
            var emailExists = await _db.Drivers
                .Find(d => d.Email == dto.Email)
                .AnyAsync();

            if (emailExists)
                throw new InvalidOperationException("Driver with this email already exists");

            if (!string.IsNullOrEmpty(dto.VehicleId))
            {
                var vehicleExists = await _db.Vehicles
                    .Find(v => v.Id == dto.VehicleId)
                    .AnyAsync();

                if (!vehicleExists)
                    throw new InvalidOperationException("Vehicle not found");

                var alreadyAssigned = await _db.Drivers
                    .Find(d => d.VehicleId == dto.VehicleId)
                    .AnyAsync();

                if (alreadyAssigned)
                    throw new InvalidOperationException("Vehicle already assigned");
            }

            var driver = new Driver
            {
                Id = Nanoid.Generate(size: 10),
                FullName = dto.FullName,
                Email = dto.Email,
                Password = dto.Password,
                RfidTag = Nanoid.Generate(size: 10),
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Status = DriverStatus.Active,
                CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
                VehicleId = dto.VehicleId
            };

            await _db.Drivers.InsertOneAsync(driver);

            return Map(driver);
        }

        // ---------------- GET ALL ----------------
        public async Task<List<DriverResponseDto>> GetAllAsync()
        {
            var drivers = await _db.Drivers
                .Find(_ => true)
                .SortByDescending(d => d.CreatedAt)
                .ToListAsync();

            return drivers.Select(Map).ToList();
        }

        // ---------------- GET BY ID ----------------
        public async Task<DriverResponseDto> GetByIdAsync(string id)
        {
            var driver = await _db.Drivers
                .Find(d => d.Id == id)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Driver not found");

            return Map(driver);
        }

        // ---------------- UPDATE STATUS ----------------
        public async Task UpdateStatusAsync(string id, DriverStatus status)
        {
            var update = Builders<Driver>.Update
                .Set(d => d.Status, status)
                .Set(d => d.UpdatedAt, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified));

            var result = await _db.Drivers.UpdateOneAsync(
                d => d.Id == id,
                update
            );

            if (result.MatchedCount == 0)
                throw new KeyNotFoundException("Driver not found");
        }

        // ---------------- ASSIGN VEHICLE (returns Driver) ----------------
        public async Task<Driver> AssignVehicleAsync(string driverId, string vehicleId)
        {
            var driver = await _db.Drivers
                .Find(d => d.Id == driverId)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Driver not found");

            var vehicleExists = await _db.Vehicles
                .Find(v => v.Id == vehicleId)
                .AnyAsync();

            if (!vehicleExists)
                throw new InvalidOperationException("Vehicle not found");

            var alreadyAssigned = await _db.Drivers
                .Find(d => d.VehicleId == vehicleId && d.Id != driverId)
                .AnyAsync();

            if (alreadyAssigned)
                throw new InvalidOperationException("Vehicle already assigned");

            var update = Builders<Driver>.Update
                .Set(d => d.VehicleId, vehicleId)
                .Set(d => d.UpdatedAt, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified));

            await _db.Drivers.UpdateOneAsync(
                d => d.Id == driverId,
                update
            );

            driver.VehicleId = vehicleId;
            driver.UpdatedAt = DateTime.Now;

            return driver;
        }

        // ---------------- MAP ----------------
        private static DriverResponseDto Map(Driver d) => new()
        {
            Id = d.Id,
            FullName = d.FullName,
            Email = d.Email,
            Status = d.Status.ToString(),
            CreatedAt = d.CreatedAt
        };
    }
}
