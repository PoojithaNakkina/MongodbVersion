using esyasoft.mobility.CHRGUP.service.rmqconsumer.Messaging;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer;

public class Worker : BackgroundService
{
    private readonly RmqConsumer _consumer;

    public Worker(RmqConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.Start(stoppingToken);
    }

}
