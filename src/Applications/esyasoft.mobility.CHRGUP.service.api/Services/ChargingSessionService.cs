//using esyasoft.mobility.CHRGUP.service.api.DTOs.Session;
////using esyasoft.mobility.CHRGUP.service.api.DTOs.Charger;
//using esyasoft.mobility.CHRGUP.service.api.Interfaces;
//using esyasoft.mobility.CHRGUP.service.persistence.Data;
//using esyasoft.mobility.CHRGUP.service.core.Metadata;
//using esyasoft.mobility.CHRGUP.service.core.Models;
//using Microsoft.EntityFrameworkCore;
//using NanoidDotNet;
//using esyasoft.mobility.CHRGUP.service.core.Helpers;
//using esyasoft.mobility.CHRGUP.service.api.DTOs.Messaging;
//using esyasoft.mobility.CHRGUP.service.api.Infrastructure.Messaging;

//namespace esyasoft.mobility.CHRGUP.service.api.Services
//{
//    public class ChargingSessionService : IChargingSessionService
//    {
//        private readonly AppDbContext _db;
//        private readonly ILogger<ChargingSessionService> _logger;
//        private readonly AuditLogger _auditLogger;
//        private readonly RmqPublisher _publisher;

//        public ChargingSessionService(AppDbContext db, ILogger<ChargingSessionService> logger, AuditLogger auditLogger, RmqPublisher publisher)
//        {
//            _db = db;
//            _logger = logger;
//            _auditLogger = auditLogger;
//            _publisher = publisher;
//        }




//        public async Task<ChargingSessionResponseDto> GetByIdAsync(string sessionId)
//        {
//            var session = await _db.chargingSessions.FindAsync(sessionId)
//                ?? throw new InvalidOperationException("Session not found");

//            return Map(session);
//        }

//        public async Task<List<ChargingSessionResponseDto>> GetByChargerAsync(string chargerId)
//        {
//            return await _db.chargingSessions
//                .Where(s => s.ChargerId == chargerId)
//                .OrderByDescending(s => s.StartTime)
//                .Select(s => Map(s))
//                .ToListAsync();
//        }

//        public async Task<string> StartSessionAsync(
//            string chargerId,
//            string driverId)
//        {
//            var charger = await _db.chargers
//                .FirstOrDefaultAsync(c => c.Id == chargerId);

//            if (charger == null)
//                throw new Exception("charger not found");

//            if (charger.Status != ChargerStatus.Preparing)
//                throw new Exception("vehicle not authorized or charger not ready");

//            var driver = await _db.drivers
//                .FirstOrDefaultAsync(d => d.Id == driverId);

//            if (driver == null)
//                throw new Exception("Driver not authorized");

//            var existingSession = await _db.chargingSessions
//                .FirstOrDefaultAsync(s =>
//                    s.ChargerId == charger.Id &&
//                    s.Status == SessionStatus.Active);

//            if (existingSession != null)
//                throw new Exception("Charger already in use");

//            _logger.LogInformation(
//                "Starting session for Charger={ChargerId}, DriverId={DriverId}",
//                charger.Id,
//                driver.Id
//            );

//            var session = new ChargingSession
//            {
//                Id = Nanoid.Generate(size: 10),
//                ChargerId = charger.Id,
//                DriverId = driver.Id,
//                Status = SessionStatus.Pending
//            };

//            _db.chargingSessions.Add(session);
//            charger.Status = ChargerStatus.Preparing;

//            var log = await _auditLogger.SaveLogAsync(
//                source: "backend-api",
//                eventType: "SESSION_REQUESTED",
//                message: "Start charging session requested",
//                chargerId: charger.Id,
//                sessionId: session.Id,
//                driverId: driver.Id
//            );

//            _db.logs.Add( log );

//            await _db.SaveChangesAsync();

//            var command = new RemoteStartCommand
//            {
//                ChargerId = chargerId,
//                SessionId = session.Id,
//                DriverId = driverId,
//                Timestamp = DateTime.Now
//            };

//            await _publisher.PublishAsync("command.start", command);

//            return session.Id;
//        }

//        public async Task StopSessionAsync(string sessionId)
//        {
//            var session = await _db.chargingSessions
//                .FirstOrDefaultAsync(s => s.Id == sessionId);

//            if (session == null)
//                throw new Exception("Session not found");

//            if (session.Status != SessionStatus.Active)
//                throw new Exception("Session not active");

//            session.Status = SessionStatus.Stopping;

//            var log = await _auditLogger.SaveLogAsync(
//                source: "backend-api",
//                eventType: "SESSION_STOP_REQUESTED",
//                message: "Stop charging session requested",
//                chargerId: session.ChargerId,
//                sessionId: session.Id,
//                driverId: session.DriverId
//            );
//            _db.logs.Add(log);

//            await _db.SaveChangesAsync();

//            var command = new RemoteStopCommand
//            {
//                ChargerId = session.ChargerId,
//                SessionId = sessionId,
//                Timestamp = DateTime.Now
//            };

//            await _publisher.PublishAsync("command.stop", command);
//        }

//        private static ChargingSessionResponseDto Map(ChargingSession s)
//        {
//            return new ChargingSessionResponseDto
//            {
//                SessionId = s.Id,
//                ChargerId = s.ChargerId,
//                DriverId = s.DriverId,
//                Status = s.Status.ToString(),
//                StartTime = s.StartTime,
//                EndTime = s.EndTime
//            };
//        }
//    }
//}



using esyasoft.mobility.CHRGUP.service.api.DTOs.Session;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;
using NanoidDotNet;
using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.api.DTOs.Messaging;
using esyasoft.mobility.CHRGUP.service.api.Infrastructure.Messaging;
using MongoDB.Driver;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class ChargingSessionService : IChargingSessionService
    {
        private readonly MongoDbContext _db;
        private readonly ILogger<ChargingSessionService> _logger;
        private readonly AuditLogger _auditLogger;
        private readonly RmqPublisher _publisher;

        public ChargingSessionService(
            MongoDbContext db,
            ILogger<ChargingSessionService> logger,
            AuditLogger auditLogger,
            RmqPublisher publisher)
        {
            _db = db;
            _logger = logger;
            _auditLogger = auditLogger;
            _publisher = publisher;
        }

        public async Task<ChargingSessionResponseDto> GetByIdAsync(string sessionId)
        {
            var session = await _db.ChargingSessions
                .Find(s => s.Id == sessionId)
                .FirstOrDefaultAsync()
                ?? throw new InvalidOperationException("Session not found");

            return Map(session);
        }

        public async Task<List<ChargingSessionResponseDto>> GetByChargerAsync(string chargerId)
        {
            var sessions = await _db.ChargingSessions
                .Find(s => s.ChargerId == chargerId)
                .SortByDescending(s => s.StartTime)
                .ToListAsync();

            return sessions.Select(Map).ToList();
        }

        public async Task<string> StartSessionAsync(
            string chargerId,
            string driverId)
        {
            var charger = await _db.Chargers
                .Find(c => c.Id == chargerId)
                .FirstOrDefaultAsync()
                ?? throw new Exception("Charger not found");

            if (charger.Status != ChargerStatus.Preparing)
                throw new Exception("Vehicle not authorized or charger not ready");

            var driver = await _db.Drivers
                .Find(d => d.Id == driverId)
                .FirstOrDefaultAsync()
                ?? throw new Exception("Driver not authorized");

            var existingSession = await _db.ChargingSessions
                .Find(s =>
                    s.ChargerId == charger.Id &&
                    s.Status == SessionStatus.Active)
                .FirstOrDefaultAsync();

            if (existingSession != null)
                throw new Exception("Charger already in use");

            _logger.LogInformation(
                "Starting session for Charger={ChargerId}, DriverId={DriverId}",
                charger.Id,
                driver.Id
            );

            var session = new ChargingSession
            {
                Id = Nanoid.Generate(size: 10),
                ChargerId = charger.Id,
                DriverId = driver.Id,
                Status = SessionStatus.Pending,
                //StartTime = DateTime.UtcNow
            };

            await _db.ChargingSessions.InsertOneAsync(session);

            await _db.Chargers.UpdateOneAsync(
                c => c.Id == charger.Id,
                Builders<Charger>.Update.Set(c => c.Status, ChargerStatus.Preparing)
            );

            await _auditLogger.SaveLogAsync(
                source: "backend-api",
                eventType: "SESSION_REQUESTED",
                message: "Start charging session requested",
                chargerId: charger.Id,
                sessionId: session.Id,
                driverId: driver.Id
            );

            var command = new RemoteStartCommand
            {
                ChargerId = chargerId,
                SessionId = session.Id,
                DriverId = driverId,
                Timestamp = DbTime.From(DateTime.Now)
            };

            await _publisher.PublishAsync("command.start", command);

            return session.Id;
        }

        public async Task StopSessionAsync(string sessionId)
        {
            var session = await _db.ChargingSessions
                .Find(s => s.Id == sessionId)
                .FirstOrDefaultAsync()
                ?? throw new Exception("Session not found");

            if (session.Status != SessionStatus.Active)
                throw new Exception("Session not active");

            await _db.ChargingSessions.UpdateOneAsync(
                s => s.Id == sessionId,
                Builders<ChargingSession>.Update
                    .Set(s => s.Status, SessionStatus.Stopping)
                    .Set(s => s.EndTime, DbTime.From(DateTime.Now)
)
            );

            await _auditLogger.SaveLogAsync(
                source: "backend-api",
                eventType: "SESSION_STOP_REQUESTED",
                message: "Stop charging session requested",
                chargerId: session.ChargerId,
                sessionId: session.Id,
                driverId: session.DriverId
            );

            var command = new RemoteStopCommand
            {
                ChargerId = session.ChargerId,
                SessionId = sessionId,
                Timestamp = DbTime.From(DateTime.Now)

            };

            await _publisher.PublishAsync("command.stop", command);
        }

        private static ChargingSessionResponseDto Map(ChargingSession s)
        {
            return new ChargingSessionResponseDto
            {
                SessionId = s.Id,
                ChargerId = s.ChargerId,
                DriverId = s.DriverId,
                Status = s.Status.ToString(),
                StartTime = s.StartTime,
                EndTime = s.EndTime
            };
        }
    }
}

