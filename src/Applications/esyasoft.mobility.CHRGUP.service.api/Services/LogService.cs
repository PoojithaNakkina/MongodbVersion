//using esyasoft.mobility.CHRGUP.service.api.DTOs.Log;
//using esyasoft.mobility.CHRGUP.service.api.Interfaces;
//using esyasoft.mobility.CHRGUP.service.persistence.Data;
//using Microsoft.EntityFrameworkCore;

//namespace esyasoft.mobility.CHRGUP.service.api.Services
//{
//    public class LogService : ILogService
//    {
//        private readonly AppDbContext _db;

//        public LogService(AppDbContext db)
//        {
//            _db = db;
//        }

//        public async Task<List<LogResponseDto>> GetAllAsync()
//        {
//            return await _db.logs
//                .OrderByDescending(l => l.Timestamp)
//                .Select(l => new LogResponseDto
//                {
//                    Id = l.Id,
//                    Timestamp = l.Timestamp,
//                    Source = l.Source,
//                    EventType = l.EventType,
//                    Message = l.Message,
//                    ChargerId = l.ChargerId,
//                    SessionId = l.SessionId,
//                    DriverId = l.DriverId
//                })
//                .ToListAsync();
//        }

//        public async Task<LogResponseDto> GetByIdAsync(Guid id)
//        {
//            return await _db.logs
//                .Where(l => l.Id == id)
//                .Select(l => new LogResponseDto
//                {
//                    Id = l.Id,
//                    Timestamp = l.Timestamp,
//                    Source = l.Source,
//                    EventType = l.EventType,
//                    Message = l.Message,
//                    ChargerId = l.ChargerId,
//                    SessionId = l.SessionId,
//                    DriverId = l.DriverId
//                })
//                .FirstOrDefaultAsync()
//                ?? throw new KeyNotFoundException("Log not found");
//        }

//        public async Task<List<LogResponseDto>> GetBySessionIdAsync(string sessionId)
//        {
//            return await _db.logs
//                .Where(l => l.SessionId == sessionId)
//                .OrderByDescending(l => l.Timestamp)
//                .Select(l => new LogResponseDto
//                {
//                    Id = l.Id,
//                    Timestamp = l.Timestamp,
//                    Source = l.Source,
//                    EventType = l.EventType,
//                    Message = l.Message,
//                    ChargerId = l.ChargerId,
//                    SessionId = l.SessionId,
//                    DriverId = l.DriverId
//                })
//                .ToListAsync();
//        }

//        public async Task<List<LogResponseDto>> GetByChargerIdAsync(string chargerId)
//        {
//            return await _db.logs
//                .Where(l => l.ChargerId == chargerId)
//                .OrderByDescending(l => l.Timestamp)
//                .Select(l => new LogResponseDto
//                {
//                    Id = l.Id,
//                    Timestamp = l.Timestamp,
//                    Source = l.Source,
//                    EventType = l.EventType,
//                    Message = l.Message,
//                    ChargerId = l.ChargerId,
//                    SessionId = l.SessionId,
//                    DriverId = l.DriverId
//                })
//                .ToListAsync();
//        }

//        public async Task<List<LogResponseDto>> GetByDriverIdAsync(string driverId)
//        {
//            return await _db.logs
//                .Where(l => l.DriverId == driverId)
//                .OrderByDescending(l => l.Timestamp)
//                .Select(l => new LogResponseDto
//                {
//                    Id = l.Id,
//                    Timestamp = l.Timestamp,
//                    Source = l.Source,
//                    EventType = l.EventType,
//                    Message = l.Message,
//                    ChargerId = l.ChargerId,
//                    SessionId = l.SessionId,
//                    DriverId = l.DriverId
//                })
//                .ToListAsync();
//        }
//    }
//}


using esyasoft.mobility.CHRGUP.service.api.DTOs.Log;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using MongoDB.Driver;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class LogService : ILogService
    {
        private readonly MongoDbContext _db;

        public LogService(MongoDbContext db)
        {
            _db = db;
        }

        public async Task<List<LogResponseDto>> GetAllAsync()
        {
            return await _db.Logs
                .Find(_ => true)
                .SortByDescending(l => l.Timestamp)
                .Project(l => new LogResponseDto
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    Source = l.Source,
                    EventType = l.EventType,
                    Message = l.Message,
                    ChargerId = l.ChargerId,
                    SessionId = l.SessionId,
                    DriverId = l.DriverId
                })
                .ToListAsync();
        }

        public async Task<LogResponseDto> GetByIdAsync(Guid id)
        {
            var log = await _db.Logs
                .Find(l => l.Id == id)
                .Project(l => new LogResponseDto
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    Source = l.Source,
                    EventType = l.EventType,
                    Message = l.Message,
                    ChargerId = l.ChargerId,
                    SessionId = l.SessionId,
                    DriverId = l.DriverId
                })
                .FirstOrDefaultAsync();

            return log ?? throw new KeyNotFoundException("Log not found");
        }

        public async Task<List<LogResponseDto>> GetBySessionIdAsync(string sessionId)
        {
            return await _db.Logs
                .Find(l => l.SessionId == sessionId)
                .SortByDescending(l => l.Timestamp)
                .Project(l => new LogResponseDto
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    Source = l.Source,
                    EventType = l.EventType,
                    Message = l.Message,
                    ChargerId = l.ChargerId,
                    SessionId = l.SessionId,
                    DriverId = l.DriverId
                })
                .ToListAsync();
        }

        public async Task<List<LogResponseDto>> GetByChargerIdAsync(string chargerId)
        {
            return await _db.Logs
                .Find(l => l.ChargerId == chargerId)
                .SortByDescending(l => l.Timestamp)
                .Project(l => new LogResponseDto
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    Source = l.Source,
                    EventType = l.EventType,
                    Message = l.Message,
                    ChargerId = l.ChargerId,
                    SessionId = l.SessionId,
                    DriverId = l.DriverId
                })
                .ToListAsync();
        }

        public async Task<List<LogResponseDto>> GetByDriverIdAsync(string driverId)
        {
            return await _db.Logs
                .Find(l => l.DriverId == driverId)
                .SortByDescending(l => l.Timestamp)
                .Project(l => new LogResponseDto
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    Source = l.Source,
                    EventType = l.EventType,
                    Message = l.Message,
                    ChargerId = l.ChargerId,
                    SessionId = l.SessionId,
                    DriverId = l.DriverId
                })
                .ToListAsync();
        }
    }
}

