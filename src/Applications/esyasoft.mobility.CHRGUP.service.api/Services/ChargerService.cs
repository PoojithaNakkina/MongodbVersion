
//using esyasoft.mobility.CHRGUP.service.api.Infrastructure.Messaging;
//using esyasoft.mobility.CHRGUP.service.api.Interfaces;
//using esyasoft.mobility.CHRGUP.service.core.Metadata;
//using esyasoft.mobility.CHRGUP.service.core.Models;
//using esyasoft.mobility.CHRGUP.service.persistence.Data;
//using MongoDB.Driver;
//using NanoidDotNet;

//namespace esyasoft.mobility.CHRGUP.service.api.Services
//{
//    public class ChargerService : IChargerService
//    {
//        private readonly MongoDbContext _db;
//        private readonly RmqPublisher _publisher;
//        private static readonly Random random = new Random();

//        public ChargerService(MongoDbContext db, RmqPublisher publisher)
//        {
//            _db = db;
//            _publisher = publisher;
//        }

//        public async Task<List<Charger>> GetAllAsync()
//        {
//            return await _db.Chargers
//                .Find(_ => true)
//                .ToListAsync();
//        }

//        public async Task<Charger> RegisterAsync(string locationId, string version)
//        {
//            var locationExists = await _db.Locations
//                .Find(l => l.Id == locationId)
//                .AnyAsync();

//            if (!locationExists)
//                throw new ArgumentException("Invalid LocationId");

//               var config = new ChargerConfig
//            {
//                ChargerId = Nanoid.Generate(size: 10),
//                Manufacturer = "esyasoft",
//                FirmwareVersion = version,
//                InputPower = 10.0 + (random.NextDouble() * (20)),
//                OutputPower = 15.0 + random.NextDouble() * 10,
//                ConnectorType = "Type-1",
//                NoOfPorts = 1
//            };

//            var charger = new Charger
//            {
//                Id = config.ChargerId,
//                Status = ChargerStatus.Available,
//                LastSeen = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
//                LocationId = locationId
//            };

//            await _db.Chargers.InsertOneAsync(charger);
//            return charger;
//        }

//        public async Task UpdateStatusAsync(string chargerId, ChargerStatus status)
//        {
//            var result = await _db.Chargers.UpdateOneAsync(
//                c => c.Id == chargerId,
//                Builders<Charger>.Update.Set(c => c.Status, status)
//            );

//            if (result.MatchedCount == 0)
//                throw new KeyNotFoundException("Charger not found");
//        }
//        //Remove UpdateHeartbeatAsync
//        public async Task UpdateHeartbeatAsync(string chargerId, DateTime timestamp)
//        {
//            var result = await _db.Chargers.UpdateOneAsync(
//                c => c.Id == chargerId,
//                Builders<Charger>.Update.Set(c => c.LastSeen, timestamp)
//            );

//            if (result.MatchedCount == 0)
//                throw new KeyNotFoundException("Charger not found");
//        }

//        //public Task<Charger> RegisterAsync(string locationId, string version)
//        //{
//        //    throw new NotImplementedException();
//        //}
//    }
//}


using esyasoft.mobility.CHRGUP.service.api.Infrastructure.Messaging;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using MongoDB.Driver;
using NanoidDotNet;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class ChargerService : IChargerService
    {
        private readonly MongoDbContext _db;
        private readonly RmqPublisher _publisher;
        private static readonly Random _random = new();

        public ChargerService(MongoDbContext db, RmqPublisher publisher)
        {
            _db = db;
            _publisher = publisher;
        }

        // ---------------- GET ALL ----------------
        public async Task<List<Charger>> GetAllAsync()
        {
            return await _db.Chargers
                .Find(_ => true)
                .ToListAsync();
        }

        // ---------------- REGISTER ----------------
        public async Task<Charger> RegisterAsync(string locationId, string version)
        {
            // 1️⃣ Validate location
            var locationExists = await _db.Locations
                .Find(l => l.Id == locationId)
                .AnyAsync();

            if (!locationExists)
                throw new ArgumentException("Invalid LocationId");

            // 2️⃣ Generate ChargerId
            var chargerId = Nanoid.Generate(size: 10);

            // 3️⃣ Create charger config
            var config = new ChargerConfig
            {
                ChargerId = chargerId,
                Manufacturer = "esyasoft",
                FirmwareVersion = version,
                InputPower = 10.0 + (_random.NextDouble() * 20),
                OutputPower = 15.0 + (_random.NextDouble() * 10),
                ConnectorType = "Type-1",
                NoOfPorts = 1
            };

            // 4️⃣ Create charger
            var charger = new Charger
            {
                Id = chargerId,
                Status = ChargerStatus.Available,
                LastSeen = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
                LocationId = locationId
            };

            // 5️⃣ Persist (MongoDB = explicit)
            await _db.ChargerConfigs.InsertOneAsync(config);
            await _db.Chargers.InsertOneAsync(charger);

            return charger;
        }

        // ---------------- UPDATE STATUS ----------------
        public async Task UpdateStatusAsync(string chargerId, ChargerStatus status)
        {
            var update = Builders<Charger>.Update
                .Set(c => c.Status, status);

            var result = await _db.Chargers.UpdateOneAsync(
                c => c.Id == chargerId,
                update
            );

            if (result.MatchedCount == 0)
                throw new KeyNotFoundException("Charger not found");
        }

        // ---------------- UPDATE HEARTBEAT ----------------
        public async Task UpdateHeartbeatAsync(string chargerId, DateTime timestamp)
        {
            var update = Builders<Charger>.Update
                .Set(c => c.LastSeen, DbTime.From(timestamp));

            var result = await _db.Chargers.UpdateOneAsync(
                c => c.Id == chargerId,
                update
            );

            if (result.MatchedCount == 0)
                throw new KeyNotFoundException("Charger not found");
        }
    }
}
