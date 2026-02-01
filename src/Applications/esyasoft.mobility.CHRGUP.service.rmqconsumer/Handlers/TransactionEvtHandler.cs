//using esyasoft.mobility.CHRGUP.service.core.Helpers;
//using esyasoft.mobility.CHRGUP.service.core.Metadata;
//using esyasoft.mobility.CHRGUP.service.persistence.Data;
//using esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs;
//using Microsoft.EntityFrameworkCore;

//namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers
//{
//    public class TransactionEvtHandler
//    {
//        private readonly AppDbContext _db;
//        private readonly ILogger<TransactionEvtHandler> _logger;
//        private readonly AuditLogger _logger1;
//        public TransactionEvtHandler(AppDbContext db, ILogger<TransactionEvtHandler> logger, AuditLogger logger1)
//        {
//            _db = db;
//            _logger = logger;
//            _logger1 = logger1;
//        }
//        public async Task HandleSessionStarted(SessionStartEvent evt)
//        {
//            var session = await _db.chargingSessions
//                .FirstOrDefaultAsync(s => s.Id == evt.SessionId);

//            if (session == null || session.Status != SessionStatus.Pending)
//                return;

//            if (session.ChargerId != evt.ChargerId)
//            {
//                session.Status = SessionStatus.Faulted;
//                await _db.SaveChangesAsync();
//                return;
//            }

//            session.Status = SessionStatus.Active;
//            session.StartTime = evt.StartTime;
//            session.InitialCharge = evt.SOC;
//            session.SOC = evt.SOC;


//            var charger = await _db.chargers
//                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

//            if (charger != null)
//                charger.Status = ChargerStatus.Engaged;

//            var log = await _logger1.SaveLogAsync(
//                source: "charger",
//                eventType: "SESSION_STARTED",
//                message: "Charging session started",
//                chargerId: evt.ChargerId,
//                sessionId: evt.SessionId,
//                driverId: session.DriverId
//            );
//            _db.logs.Add( log );

//            await _db.SaveChangesAsync();
//        }

//        public async Task HandleSessionStopped(SessionStopEvent evt)
//        {
//            var session = await _db.chargingSessions
//                .FirstOrDefaultAsync(s => s.Id == evt.SessionId);

//            if (session == null ||
//                session.Status == SessionStatus.Completed ||
//                session.ChargerId != evt.ChargerId)
//                return;

//            if (evt.triggerReason == "Fault")
//            {
//                _logger.LogWarning(
//                    "Stop event due to fault for Session={SessionId}",
//                    evt.SessionId
//                );
//                return;
//            }

//            session.Status = SessionStatus.Completed;
//            session.EndTime = evt.StopTime;
//            session.SOC = evt.SOC;

//            var charger = await _db.chargers
//                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

//            if (charger != null)
//                charger.Status = ChargerStatus.Available;

//            var log = await _logger1.SaveLogAsync(
//                source: "charger",
//                eventType: "SESSION_ENDED",
//                message: "Charging session completed",
//                chargerId: evt.ChargerId,
//                sessionId: evt.SessionId,
//                driverId: session.DriverId
//            );
//            _db.logs.Add( log );

//            await _db.SaveChangesAsync();
//        }
//    }
//}


using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs;
using MongoDB.Driver;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers
{
    public class TransactionEvtHandler
    {
        private readonly MongoDbContext _db;
        private readonly ILogger<TransactionEvtHandler> _logger;
        private readonly AuditLogger _auditLogger;

        public TransactionEvtHandler(
            MongoDbContext db,
            ILogger<TransactionEvtHandler> logger,
            AuditLogger auditLogger)
        {
            _db = db;
            _logger = logger;
            _auditLogger = auditLogger;
        }

        public async Task HandleSessionStarted(SessionStartEvent evt)
        {
            var session = await _db.ChargingSessions
                .Find(s => s.Id == evt.SessionId)
                .FirstOrDefaultAsync();

            if (session == null || session.Status != SessionStatus.Pending)
                return;

            if (session.ChargerId != evt.ChargerId)
            {
                await _db.ChargingSessions.UpdateOneAsync(
                    s => s.Id == evt.SessionId,
                    Builders<ChargingSession>.Update
                        .Set(s => s.Status, SessionStatus.Faulted)
                );
                return;
            }

            // 🔹 Activate session
            await _db.ChargingSessions.UpdateOneAsync(
                s => s.Id == evt.SessionId,
                Builders<ChargingSession>.Update
                    .Set(s => s.Status, SessionStatus.Active)
                    .Set(s => s.StartTime, evt.StartTime)
                    .Set(s => s.InitialCharge, evt.SOC)
                    .Set(s => s.SOC, evt.SOC)
            );

            // 🔹 Update charger status
            await _db.Chargers.UpdateOneAsync(
                c => c.Id == evt.ChargerId,
                Builders<Charger>.Update
                    .Set(c => c.Status, ChargerStatus.Engaged)
            );

            // 🔹 Audit log
            var log = await _auditLogger.SaveLogAsync(
                source: "charger",
                eventType: "SESSION_STARTED",
                message: "Charging session started",
                chargerId: evt.ChargerId,
                sessionId: evt.SessionId,
                driverId: session.DriverId
            );

            await _db.Logs.InsertOneAsync(log);
        }

        //public async Task HandleSessionStopped(SessionStopEvent evt)
        //{
        //    var session = await _db.ChargingSessions
        //        .Find(s => s.Id == evt.SessionId)
        //        .FirstOrDefaultAsync();

        //    if (session == null ||
        //        session.Status == SessionStatus.Completed ||
        //        session.ChargerId != evt.ChargerId)
        //        return;

        //    if (evt.triggerReason == "Fault")
        //    {
        //        _logger.LogWarning(
        //            "Stop event due to fault for Session={SessionId}",
        //            evt.SessionId
        //        );
        //        return;
        //    }

        //    // 🔹 Complete session
        //    await _db.ChargingSessions.UpdateOneAsync(
        //        s => s.Id == evt.SessionId,
        //        Builders<ChargingSession>.Update
        //            .Set(s => s.Status, SessionStatus.Completed)
        //            .Set(s => s.EndTime, evt.StopTime)
        //            .Set(s => s.SOC, evt.SOC)
        //    );

        //    // 🔹 Make charger available
        //    await _db.Chargers.UpdateOneAsync(
        //        c => c.Id == evt.ChargerId,
        //        Builders<Charger>.Update
        //            .Set(c => c.Status, ChargerStatus.Available)
        //    );

        //    // 🔹 Audit log
        //    var log = await _auditLogger.SaveLogAsync(
        //        source: "charger",
        //        eventType: "SESSION_ENDED",
        //        message: "Charging session completed",
        //        chargerId: evt.ChargerId,
        //        sessionId: evt.SessionId,
        //        driverId: session.DriverId
        //    );

        //    await _db.Logs.InsertOneAsync(log);
        //}

        public async Task HandleSessionStopped(SessionStopEvent evt)
        {
            var session = await _db.ChargingSessions
                .Find(s => s.Id == evt.SessionId)
                .FirstOrDefaultAsync();

            if (session == null ||
                session.Status == SessionStatus.Completed ||
                session.Status == SessionStatus.Faulted ||   // 🔥 BLOCK FAULTED
                session.ChargerId != evt.ChargerId)
                return;

            if (evt.triggerReason == "Fault" ||
                evt.triggerReason == "PowerLoss" ||
                evt.triggerReason == "EmergencyStop")
            {
                _logger.LogWarning(
                    "Stop event ignored due to fault for Session={SessionId}, Reason={Reason}",
                    evt.SessionId,
                    evt.triggerReason
                );
                return;
            }

            await _db.ChargingSessions.UpdateOneAsync(
                s => s.Id == evt.SessionId &&
                     s.Status != SessionStatus.Faulted,   // 🔒 DB PROTECTION
                Builders<ChargingSession>.Update
                    .Set(s => s.Status, SessionStatus.Completed)
                    .Set(s => s.EndTime, evt.StopTime)
                    .Set(s => s.SOC, evt.SOC)
            );

            await _db.Chargers.UpdateOneAsync(
                c => c.Id == evt.ChargerId,
                Builders<Charger>.Update
                    .Set(c => c.Status, ChargerStatus.Available)
            );

            var log = await _auditLogger.SaveLogAsync(
                source: "charger",
                eventType: "SESSION_ENDED",
                message: "Charging session completed",
                chargerId: evt.ChargerId,
                sessionId: evt.SessionId,
                driverId: session.DriverId
            );

            await _db.Logs.InsertOneAsync(log);
        }

    }
}

