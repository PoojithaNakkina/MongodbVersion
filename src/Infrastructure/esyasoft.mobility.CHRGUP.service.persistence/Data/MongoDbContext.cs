using esyasoft.mobility.CHRGUP.service.core.Models;
using esyasoft.mobility.CHRGUP.service.persistence.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace esyasoft.mobility.CHRGUP.service.persistence.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        //public MongoDbContext(IOptions<MongoDbSettings> settings)
        //{
        //    var client = new MongoClient(settings.Value.ConnectionString);
        //    _database = client.GetDatabase(settings.Value.DatabaseName);
        //}

        public MongoDbContext(MongoDbSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new InvalidOperationException("MongoDB ConnectionString is null");

            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        // Collections (replacement for DbSet<T>)
        public IMongoCollection<Admin> Admins => _database.GetCollection<Admin>("admins");
        public IMongoCollection<Charger> Chargers => _database.GetCollection<Charger>("chargers");
        public IMongoCollection<ChargingSession> ChargingSessions => _database.GetCollection<ChargingSession>("charging_sessions");
        public IMongoCollection<Driver> Drivers => _database.GetCollection<Driver>("drivers");
        public IMongoCollection<Fault> Faults => _database.GetCollection<Fault>("faults");
        public IMongoCollection<Location> Locations => _database.GetCollection<Location>("locations");
        public IMongoCollection<Log> Logs => _database.GetCollection<Log>("logs");
        public IMongoCollection<Manager> Managers => _database.GetCollection<Manager>("managers");
        public IMongoCollection<Supervisor> Supervisors => _database.GetCollection<Supervisor>("supervisors");
        public IMongoCollection<Vehicle> Vehicles => _database.GetCollection<Vehicle>("vehicles");
        public IMongoCollection<ChargerConfig> ChargerConfigs => _database.GetCollection<ChargerConfig>("charger_configs");
        public IMongoCollection<Reservation> Reservations => _database.GetCollection<Reservation>("reservations");

    }
}



