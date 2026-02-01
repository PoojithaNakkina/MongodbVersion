//using esyasoft.mobility.CHRGUP.service.core.Metadata;
//using esyasoft.mobility.CHRGUP.service.core.Models;
//using esyasoft.mobility.CHRGUP.service.persistence.Data;
//using esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs;
//using esyasoft.mobility.CHRGUP.service.rmqconsumer.Messaging;
//using Microsoft.EntityFrameworkCore;

//namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers
//{
//    public class AuthEvtHandler
//    {
//        private readonly AppDbContext _db;
//        private readonly ILogger<AuthEvtHandler> _logger;
//        private readonly RmqPublisher _publisher;
//        public AuthEvtHandler(AppDbContext db, ILogger<AuthEvtHandler> logger, RmqPublisher publisher) 
//        {  
//            _db = db; 
//            _logger = logger;
//            _publisher = publisher;
//        }
//        public async Task HandleAuthRequest(AuthReqEvent evt)
//        {
//            Driver? driver = null;

//            switch (evt.TokenType)
//            {
//                case "VIN":
//                    driver = await _db.drivers
//                        .Include(d => d.Vehicle)
//                        .FirstOrDefaultAsync(d =>
//                            d.Vehicle != null &&
//                            d.Vehicle.VIN == evt.Token);
//                    break;

//                case "RFID":
//                    driver = await _db.drivers
//                        .Include(d => d.Vehicle)
//                        .FirstOrDefaultAsync(d =>
//                            d.RfidTag == evt.Token);
//                    break;

//                default:
//                    _logger.LogWarning(
//                        "Unsupported token type {TokenType} for token {Token}",
//                        evt.TokenType,
//                        evt.Token);
//                    break;
//            }

//            bool accepted =
//                driver != null &&
//                driver.Status == DriverStatus.Active;

//            await _publisher.PublishAsync(
//                "event.authorization.result",
//                new
//                {
//                    evt.MessageId,
//                    Accepted = accepted
//                });

//            _logger.LogInformation(
//                "TOKEN {Token} ({Type}) auth = {Result}",
//                evt.Token,
//                evt.TokenType,
//                accepted ? "ACCEPTED" : "REJECTED"
//            );

//            if (!accepted)
//                return;

//            var charger = await _db.chargers
//                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

//            if (charger == null)
//            {
//                _logger.LogError(
//                    "Charger {Id} not found for auth",
//                    evt.ChargerId);
//                return;
//            }

//            if (charger.Status == ChargerStatus.Available)
//            {
//                charger.Status = ChargerStatus.Preparing;
//                charger.LastSeen = DateTime.Now;

//                await _db.SaveChangesAsync();

//                _logger.LogInformation(
//                    "Charger {Id} moved to PREPARING after auth",
//                    charger.Id);
//            }
//        }

//    }
//}


using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.Messaging;
using MongoDB.Driver;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers
{
    public class AuthEvtHandler
    {
        private readonly MongoDbContext _db;
        private readonly ILogger<AuthEvtHandler> _logger;
        private readonly RmqPublisher _publisher;

        public AuthEvtHandler(
            MongoDbContext db,
            ILogger<AuthEvtHandler> logger,
            RmqPublisher publisher)
        {
            _db = db;
            _logger = logger;
            _publisher = publisher;
        }

        public async Task HandleAuthRequest(AuthReqEvent evt)
        {
            Driver? driver = null;

            switch (evt.TokenType)
            {
                case "VIN":
                    {
                        // 1️⃣ Find vehicle by VIN
                        var vehicle = await _db.Vehicles
                            .Find(v => v.VIN == evt.Token)
                            .FirstOrDefaultAsync();

                        if (vehicle != null)
                        {
                            // 2️⃣ Find driver mapped to that vehicle
                            driver = await _db.Drivers
                                .Find(d => d.VehicleId == vehicle.Id)
                                .FirstOrDefaultAsync();
                        }
                        break;
                    }

                case "RFID":
                    {
                        driver = await _db.Drivers
                            .Find(d => d.RfidTag == evt.Token)
                            .FirstOrDefaultAsync();
                        break;
                    }

                default:
                    _logger.LogWarning(
                        "Unsupported token type {TokenType} for token {Token}",
                        evt.TokenType,
                        evt.Token);
                    break;
            }

            bool accepted =
                driver != null &&
                driver.Status == DriverStatus.Active;

            // 🔹 Publish authorization result back to OCPP
            await _publisher.PublishAsync(
                "event.authorization.result",
                new
                {
                    evt.MessageId,
                    Accepted = accepted
                });

            _logger.LogInformation(
                "TOKEN {Token} ({Type}) auth = {Result}",
                evt.Token,
                evt.TokenType,
                accepted ? "ACCEPTED" : "REJECTED"
            );

            if (!accepted)
            {
                Console.WriteLine("driver error");
                return;
            }
            // 🔹 Update charger state
            var charger = await _db.Chargers
                .Find(c => c.Id == evt.ChargerId)
                .FirstOrDefaultAsync();

            if (charger == null)
            {
                _logger.LogError(
                    "Charger {Id} not found for auth",
                    evt.ChargerId);
                return;
            }

            if (charger.Status == ChargerStatus.Available)
            {
                var update = Builders<Charger>.Update
                    .Set(c => c.Status, ChargerStatus.Preparing)
                    .Set(c => c.LastSeen, DateTime.Now);

                await _db.Chargers.UpdateOneAsync(
                    c => c.Id == charger.Id,
                    update);

                _logger.LogInformation(
                    "Charger {Id} moved to PREPARING after auth",
                    charger.Id);
            }
        }
    }
}

