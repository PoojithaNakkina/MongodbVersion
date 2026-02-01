using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Messaging
{
    public static class RabbitMqEventPublisher
    {
        private const string ExchangeName = "charging_events_ex";

        public static async ValueTask PublishAsync(
            string routingKey,
            object payload)
        {
            await RabbitMqConnection.Channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Direct,
                durable: true
            );

            var props = new BasicProperties
            {
                Persistent = true
            };

            var body = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(payload));

            await RabbitMqConnection.Channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: CancellationToken.None
            );
            Console.WriteLine("published from ocpp");
        }
    }
}

