


using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.core.Models;
using MongoDB.Driver;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Services
{
    public class ChargerAuthService
    {
        private readonly MongoDbContext _mongoDb;

        public ChargerAuthService(MongoDbContext mongoDb)
        {
            _mongoDb = mongoDb;
        }

        public async Task<bool> ValidateAsync(string chargerId, string tenantId)
        {
            var charger = await _mongoDb.ChargerConfigs
                .Find(c => c.ChargerId == chargerId)
                .FirstOrDefaultAsync();

            if (charger == null)
            {
                Console.WriteLine($" Charger not found in config: {chargerId}");
                return false;
            }

            return true;
        }
    }
}
