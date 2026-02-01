//using esyasoft.mobility.CHRGUP.service.persistence.Data;
//using esyasoft.mobility.CHRGUP.service.api.DTOs.Location;
//using esyasoft.mobility.CHRGUP.service.api.Interfaces;
//using esyasoft.mobility.CHRGUP.service.core.Models;
//using Microsoft.EntityFrameworkCore;
//using NanoidDotNet;

//namespace esyasoft.mobility.CHRGUP.service.api.Services
//{
//    public class LocationService : ILocationService
//    {
//        private readonly AppDbContext _context;

//        public LocationService(AppDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<Location> CreateAsync(CreateLocationDto request)
//        {
//            var location = new Location
//            {
//                Id = Nanoid.Generate(size: 10),
//                Name = request.Name,
//                Address = request.Address,
//                Latitude = request.Latitude,
//                Longitude = request.Longitude
//            };

//            _context.locations.Add(location);
//            await _context.SaveChangesAsync();
//            return location;
//        }

//        public async Task<List<Location>> GetAllAsync()
//        {
//            return await _context.locations.ToListAsync();
//        }

//        public async Task<Location?> GetByIdAsync(string id)
//        {
//            return await _context.locations.FindAsync(id);
//        }

//        public async Task<Location?> UpdateAsync(string id, UpdateLocationDto request)
//        {
//            var location = await _context.locations.FindAsync(id);
//            if (location == null) return null;

//            location.Name = request.Name;
//            location.Address = request.Address;
//            location.Latitude = request.Latitude;
//            location.Longitude = request.Longitude;

//            await _context.SaveChangesAsync();
//            return location;
//        }

//        public async Task<bool> DeleteAsync(string id)
//        {
//            var location = await _context.locations.FindAsync(id);
//            if (location == null) return false;

//            bool hasChargers = await _context.chargers
//                .AnyAsync(c => c.LocationId == id);

//            if (hasChargers)
//                throw new InvalidOperationException("Chargers are assigned to this location");

//            _context.locations.Remove(location);
//            await _context.SaveChangesAsync();
//            return true;
//        }

//        public async Task<List<object>> GetAllWithChargersAsync()
//        {
//            return await _context.locations
//                .Select(l => new
//                {
//                    l.Id,
//                    l.Name,
//                    l.Address,
//                    l.Latitude,
//                    l.Longitude,
//                    Chargers = _context.chargers
//                        .Where(c => c.LocationId == l.Id)
//                        .Select(c => new
//                        {
//                            c.Id,
//                            c.Status,
//                            c.LastSeen
//                        })
//                        .ToList()
//                })
//                .Cast<object>()
//                .ToListAsync();
//        }

//        public async Task<List<Location>> SearchAsync(string name)
//        {
//            return await _context.locations
//                .Where(l => l.Name.ToLower().Contains(name.ToLower()))
//                .ToListAsync();
//        }

//        public async Task<object?> GetLocationWithChargersAsync(string id)
//        {
//            var location = await _context.locations.FindAsync(id);
//            if (location == null) return null;

//            var chargers = await _context.chargers
//                .Where(c => c.LocationId == id)
//                .Select(c => new
//                {
//                    c.Id,
//                    c.Status,
//                    c.LastSeen
//                })
//                .ToListAsync();

//            return new
//            {
//                location.Id,
//                location.Name,
//                location.Address,
//                location.Latitude,
//                location.Longitude,
//                Chargers = chargers
//            };
//        }
//    }
//}

using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.api.DTOs.Location;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.core.Models;
using MongoDB.Driver;
using NanoidDotNet;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class LocationService : ILocationService
    {
        private readonly MongoDbContext _db;

        public LocationService(MongoDbContext db)
        {
            _db = db;
        }

        public async Task<Location> CreateAsync(CreateLocationDto request)
        {
            var location = new Location
            {
                Id = Nanoid.Generate(size: 10),
                Name = request.Name,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            await _db.Locations.InsertOneAsync(location);
            return location;
        }

        public async Task<List<Location>> GetAllAsync()
        {
            return await _db.Locations
                .Find(_ => true)
                .ToListAsync();
        }

        public async Task<Location?> GetByIdAsync(string id)
        {
            return await _db.Locations
                .Find(l => l.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Location?> UpdateAsync(string id, UpdateLocationDto request)
        {
            var update = Builders<Location>.Update
                .Set(l => l.Name, request.Name)
                .Set(l => l.Address, request.Address)
                .Set(l => l.Latitude, request.Latitude)
                .Set(l => l.Longitude, request.Longitude);

            var result = await _db.Locations.FindOneAndUpdateAsync(
                l => l.Id == id,
                update,
                new FindOneAndUpdateOptions<Location>
                {
                    ReturnDocument = ReturnDocument.After
                });

            return result;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var hasChargers = await _db.Chargers
                .Find(c => c.LocationId == id)
                .AnyAsync();

            if (hasChargers)
                throw new InvalidOperationException(
                    "Chargers are assigned to this location");

            var result = await _db.Locations.DeleteOneAsync(l => l.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<object>> GetAllWithChargersAsync()
        {
            var locations = await _db.Locations
                .Find(_ => true)
                .ToListAsync();

            var chargers = await _db.Chargers
                .Find(_ => true)
                .ToListAsync();

            return locations.Select(l => new
            {
                l.Id,
                l.Name,
                l.Address,
                l.Latitude,
                l.Longitude,
                Chargers = chargers
                    .Where(c => c.LocationId == l.Id)
                    .Select(c => new
                    {
                        c.Id,
                        c.Status,
                        c.LastSeen
                    })
                    .ToList()
            }).Cast<object>().ToList();
        }

        public async Task<List<Location>> SearchAsync(string name)
        {
            return await _db.Locations
                .Find(l =>
                    l.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();
        }

        public async Task<object?> GetLocationWithChargersAsync(string id)
        {
            var location = await _db.Locations
                .Find(l => l.Id == id)
                .FirstOrDefaultAsync();

            if (location == null)
                return null;

            var chargers = await _db.Chargers
                .Find(c => c.LocationId == id)
                .ToListAsync();

            return new
            {
                location.Id,
                location.Name,
                location.Address,
                location.Latitude,
                location.Longitude,
                Chargers = chargers.Select(c => new
                {
                    c.Id,
                    c.Status,
                    c.LastSeen
                })
            };
        }
    }
}

