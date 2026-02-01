using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.State;

namespace esyasoft.mobility.CHRGUP.service.ocpp.WebSockets
{
    public class ChargerWatchdog : BackgroundService
    {
        private readonly ILogger<ChargerWatchdog> _logger;

        private static readonly TimeSpan HeartbeatTimeout =
            TimeSpan.FromSeconds(30);

        public ChargerWatchdog(ILogger<ChargerWatchdog> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckChargers();
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task CheckChargers()
        {
            var now = DateTime.Now;

            foreach (var (chargerId, lastSeen) in HeartbeatStore.GetAll())
            {
                if (now - lastSeen > HeartbeatTimeout)
                {
                    await HandlePowerLoss(chargerId);
                    HeartbeatStore.Remove(chargerId);
                }
            }
        }

        private async Task HandlePowerLoss(string chargerId)
        {
            var state = ChargerStateStore.Get(chargerId);
            if (state.IsFaulted)
                return;

            state.IsFaulted = true;
            _logger.LogWarning(
                "Power loss detected for charger {ChargerId}",
                chargerId
            );

            await RabbitMqEventPublisher.PublishAsync(
                "event.charger.faulted",
                new
                {
                    ChargerId = chargerId,
                    FaultCode = "PowerLoss",
                    Timestamp = DbTime.From(DateTime.Now)
                }
            );
        }
    }
}
