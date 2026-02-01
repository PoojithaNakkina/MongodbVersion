//using esyasoft.mobility.CHRGUP.service.core.Helpers;
//using esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents;
//using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
//using esyasoft.mobility.CHRGUP.service.ocpp.State;
//using System.Text.Json;

//namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201.Handlers
//{
//    public static class StatusNotificationHandler
//    {
//        public static async Task Handle(
//            JsonElement payload,
//            string chargePointId)
//        {
//            Console.WriteLine("entered statnot handler--v201");
//            HeartbeatStore.Update(chargePointId);

//            var evseId = payload.GetProperty("evseId").GetInt32();
//            //var connectorId = payload.GetProperty("connectorId").GetInt32();
//            var status = payload.GetProperty("connectorStatus").GetString();
//            var timestamp = DbTime.From(payload.GetProperty("timestamp").GetDateTime());
//            Console.WriteLine($"charger: {chargePointId}; status: {status}");

//            string? errorCode = null;
//            if (payload.TryGetProperty("errorCode", out var err))
//            {
//                errorCode = err.GetString();
//            }

//            var state = ChargerStateStore.Get(chargePointId);
//            state.LastSeenUtc = timestamp;

//            if (status == "Faulted")
//            {
//                state.IsFaulted = true;
//                var ev = new ChargerFaultEvent
//                {
//                    ChargerId = chargePointId,
//                    FaultCode = errorCode ?? "Unknown",
//                    Timestamp = timestamp
//                };
//                await RabbitMqEventPublisher.PublishAsync(
//                    "event.charger.faulted",
//                    ev
//                );
//                Console.WriteLine($"charger {chargePointId} is being faulted here--v201");
//            }
//            if (status == "Available")
//            {
//                state.IsFaulted = false;
//                HeartbeatStore.Update(chargePointId);
//                var ev = new ChargerRecoverEvent
//                {
//                    ChargerId = chargePointId,
//                    Timestamp = timestamp
//                };
//                await RabbitMqEventPublisher.PublishAsync(
//                    "event.charger.recovered",ev
//                );
//                Console.WriteLine($"charger {chargePointId} is being recovered here--v201");
//            }
//        }
//    }
//}

using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.ocpp.CanonicalEvents;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using MongoDB.Driver;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201.Handlers
{
    public static class StatusNotificationHandler
    {
        public static async Task Handle(
     JsonElement payload,
     string chargePointId,
     MongoDbContext db)
        {
            Console.WriteLine("entered statnot handler--v201");

            HeartbeatStore.Update(chargePointId);

            var status = payload.GetProperty("connectorStatus").GetString();
            var timestamp = DbTime.From(payload.GetProperty("timestamp").GetDateTime());

            string? errorCode = null;
            if (payload.TryGetProperty("errorCode", out var err))
                errorCode = err.GetString();

            var state = ChargerStateStore.Get(chargePointId);
            state.LastSeenUtc = timestamp;

           
            if (status == "Faulted")
            {
                state.IsFaulted = true;

                await RabbitMqEventPublisher.PublishAsync(
                    "event.charger.faulted",
                    new ChargerFaultEvent
                    {
                        ChargerId = chargePointId,
                        FaultCode = errorCode ?? "Unknown",
                        Timestamp = timestamp
                    }
                );

                var activeSession = await db.ChargingSessions.Find(s =>
                    s.ChargerId == chargePointId &&
                    s.Status == SessionStatus.Active
                ).FirstOrDefaultAsync();

                if (activeSession != null)
                {
                    await db.ChargingSessions.UpdateOneAsync(
                        s => s.Id == activeSession.Id,
                        Builders<ChargingSession>.Update
                            .Set(s => s.Status, SessionStatus.Faulted)
                            .Set(s => s.EndTime, timestamp)
                    );

                    Console.WriteLine($"Session {activeSession.Id} marked FAULTED");
                }
            }

            
            if (status == "Available")
            {
                state.IsFaulted = false;

                await RabbitMqEventPublisher.PublishAsync(
                    "event.charger.recovered",
                    new ChargerRecoverEvent
                    {
                        ChargerId = chargePointId,
                        Timestamp = timestamp
                    }
                );
            }
        }

    }
}
