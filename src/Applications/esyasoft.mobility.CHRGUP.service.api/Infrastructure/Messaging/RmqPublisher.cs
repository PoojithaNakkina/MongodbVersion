using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.api.Infrastructure.Messaging
{
    public class RmqPublisher
    {
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IChannel _channel;

        private const string ExchangeName = "charging_commands_ex";

        public RmqPublisher(IConfiguration config)
        {
            _factory = new ConnectionFactory
            {
                HostName = config["RabbitMq:HostName"],
                VirtualHost = config["RabbitMq:VirtualHost"],
                UserName = config["RabbitMq:UserName"],
                Password = config["RabbitMq:Password"]
            };
        }

        private async Task EnsureConnectionAsync()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = await _factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                await _channel.ExchangeDeclareAsync(
                    exchange: ExchangeName,
                    type: ExchangeType.Direct,
                    durable: true
                );
            }
        }

        public async Task PublishAsync<T>(string routingKey, T message)
        {
            await EnsureConnectionAsync();

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            await _channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: routingKey,
                body: body
            );
        }
    }

}
