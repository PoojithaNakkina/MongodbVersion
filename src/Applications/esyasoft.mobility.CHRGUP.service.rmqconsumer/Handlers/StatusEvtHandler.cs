


//using esyasoft.mobility.CHRGUP.service.core.Helpers;
//using esyasoft.mobility.CHRGUP.service.core.Metadata;
//using esyasoft.mobility.CHRGUP.service.core.Models;
//using esyasoft.mobility.CHRGUP.service.persistence.Data;
//using esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs;
//using MongoDB.Driver;
//using NanoidDotNet;
//using System.Runtime.InteropServices;

//namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers
//{
//    public class StatusEvtHandler
//    {
//        private readonly MongoDbContext _db;
//        private readonly AuditLogger _auditLogger;

//        public StatusEvtHandler(MongoDbContext db, AuditLogger auditLogger)
//        {
//            _db = db;
//            _auditLogger = auditLogger;
//        }

//        public async Task HandleChargerFault(ChargerFaultEvent evt)
//        {
//            // 🔹 Save fault record
//            var fault = new Fault
//            {
//                Id = Nanoid.Generate(size: 10),
//                ChargerId = evt.ChargerId,
//                FaultCode = evt.FaultCode,
//                Timestamp = DbTime.From(evt.Timestamp)
//            };

//            await _db.Faults.InsertOneAsync(fault);

//            // 🔹 Update charger state
//            await _db.Chargers.UpdateOneAsync(
//                c => c.Id == evt.ChargerId,
//                Builders<Charger>.Update
//                    .Set(c => c.Status, ChargerStatus.Faulted)
//                    .Set(c => c.LastSeen, DateTime.Now)
//            );

//            // 🔹 Find active session
//            var activeSession = await _db.ChargingSessions
//                .Find(s =>
//                    s.ChargerId == evt.ChargerId &&
//                    (s.Status == SessionStatus.Active ||
//                     s.Status == SessionStatus.Pending ||
//                     s.Status == SessionStatus.Stopping))
//                .FirstOrDefaultAsync();

//            if (activeSession != null)
//            {
//                await _db.ChargingSessions.UpdateOneAsync(
//                    s => s.Id == activeSession.Id,
//                    Builders<ChargingSession>.Update
//                        .Set(s => s.Status, SessionStatus.Faulted)
//                        .Set(s => s.EndTime, DbTime.From(evt.Timestamp))
//                );
//            }
//            Console.WriteLine($"charger {evt.ChargerId} is faulted");
//            // 🔹 Audit log
//            var log = await _auditLogger.SaveLogAsync(
//                source: "ocpp",
//                eventType: "CHARGER_FAULTED",
//                message: evt.FaultCode,
//                chargerId: evt.ChargerId,
//                sessionId: activeSession?.Id,
//                driverId: activeSession?.DriverId
//            );

//            await _db.Logs.InsertOneAsync(log);
//        }

//        public async Task HandleChargerRecovered(ChargerRecoverEvent evt)
//        {
//            // 🔹 Update charger state
//            await _db.Chargers.UpdateOneAsync(
//                c => c.Id == evt.ChargerId,
//                Builders<Charger>.Update
//                    .Set(c => c.Status, ChargerStatus.Available)
//                    .Set(c => c.LastSeen, DbTime.From(evt.Timestamp))
//            );


//            // 🔹 Audit log
//            var log = await _auditLogger.SaveLogAsync(
//                source: "ocpp",
//                eventType: "CHARGER_RECOVERED",
//                message: "Charger recovered",
//                chargerId: evt.ChargerId
//            );

//            await _db.Logs.InsertOneAsync(log);
//        }
//    }
//}


using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs;
using MongoDB.Driver;
using NanoidDotNet;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers
{
    public class StatusEvtHandler
    {
        private readonly MongoDbContext _db;
        private readonly AuditLogger _logger;

        public StatusEvtHandler(MongoDbContext db, AuditLogger logger)
        {
            _db = db;
            _logger = logger;
        }

        // -------------------- FAULT HANDLER --------------------
        public async Task HandleChargerFault(ChargerFaultEvent evt)
        {
            // 1️⃣ Save fault
            var fault = new Fault
            {
                Id = Nanoid.Generate(size: 10),
                ChargerId = evt.ChargerId,
                FaultCode = evt.FaultCode,
                Timestamp = DbTime.From(evt.Timestamp)
            };

            await _db.Faults.InsertOneAsync(fault);

            // 2️⃣ Update charger status
            var chargerUpdate = Builders<Charger>.Update
                .Set(c => c.Status, ChargerStatus.Faulted)
                .Set(c => c.LastSeen, DateTime.Now);

            await _db.Chargers.UpdateOneAsync(
                c => c.Id == evt.ChargerId,
                chargerUpdate
            );

            // 3️⃣ Find active session
            var activeSession = await _db.ChargingSessions
                .Find(s =>
                    s.ChargerId == evt.ChargerId &&
                    (s.Status == SessionStatus.Active ||
                     s.Status == SessionStatus.Pending ||
                     s.Status == SessionStatus.Stopping))
                .FirstOrDefaultAsync();

            if (activeSession != null)
            {
                var sessionUpdate = Builders<ChargingSession>.Update
                    .Set(s => s.Status, SessionStatus.Faulted)
                    .Set(s => s.EndTime, DbTime.From(evt.Timestamp));

                await _db.ChargingSessions.UpdateOneAsync(
                    s => s.Id == activeSession.Id,
                    sessionUpdate
                );
            }

            Console.WriteLine($"charger {evt.ChargerId} is faulted");

            // 4️⃣ Audit log
            var log = await _logger.SaveLogAsync(
                source: "ocpp",
                eventType: "CHARGER_FAULTED",
                message: evt.FaultCode,
                chargerId: evt.ChargerId,
                sessionId: activeSession?.Id,
                driverId: activeSession?.DriverId
            );

            await _db.Logs.InsertOneAsync(log);
        }

        // -------------------- RECOVERY HANDLER --------------------
        public async Task HandleChargerRecovered(ChargerRecoverEvent evt)
        {
            // 1️⃣ Update charger back to available
            var update = Builders<Charger>.Update
                .Set(c => c.Status, ChargerStatus.Available)
                .Set(c => c.LastSeen, DbTime.From(evt.Timestamp));

            await _db.Chargers.UpdateOneAsync(
                c => c.Id == evt.ChargerId,
                update
            );

            Console.WriteLine($"charger {evt.ChargerId} recovered");

            // 2️⃣ Audit log
            var log = await _logger.SaveLogAsync(
                source: "ocpp",
                eventType: "CHARGER_RECOVERED",
                message: "Charger recovered",
                chargerId: evt.ChargerId
            );

            await _db.Logs.InsertOneAsync(log);
        }
    }
}
