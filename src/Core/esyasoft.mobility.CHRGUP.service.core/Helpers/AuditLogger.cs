using esyasoft.mobility.CHRGUP.service.core.Models;

namespace esyasoft.mobility.CHRGUP.service.core.Helpers
{
    public class AuditLogger
    {
        public async Task<Log> SaveLogAsync(
            string source,
            string eventType,
            string message,
            string? chargerId = null,
            string? sessionId = null,
            string? driverId = null)
        {
            var log = new Log
            {
                Id = Guid.NewGuid(),
                Timestamp = DbTime.From(DateTime.Now),
                Source = source,
                EventType = eventType,
                Message = message,
                ChargerId = chargerId,
                SessionId = sessionId,
                DriverId = driverId
            };

            return log;
        }
    }
}
