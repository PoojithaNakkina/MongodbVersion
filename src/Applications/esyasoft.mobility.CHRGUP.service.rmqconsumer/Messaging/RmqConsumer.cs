using esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.Messaging
{
    public class RmqConsumer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RmqConsumer> _logger;
        private readonly ConnectionFactory _factory;

        private IConnection _connection;
        private IChannel _channel;

        private const string ExchangeName = "charging_events_ex";
        private const string QueueName = "backend_events_q";

        public RmqConsumer(
            IServiceScopeFactory scopeFactory,
            IConfiguration config,
            ILogger<RmqConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            _factory = new ConnectionFactory
            {
                HostName = config["RabbitMq:HostName"],
                VirtualHost = config["RabbitMq:VirtualHost"],
                UserName = config["RabbitMq:UserName"],
                Password = config["RabbitMq:Password"]
            };
        }

        public async Task Start(CancellationToken stoppingToken)
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 10,
                global: false
            );

            await _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Direct,
                durable: true
            );

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            await _channel.QueueBindAsync(QueueName, ExchangeName, "event.session.started");
            await _channel.QueueBindAsync(QueueName, ExchangeName, "event.session.stopped");
            await _channel.QueueBindAsync(QueueName, ExchangeName, "event.meter.value");
            await _channel.QueueBindAsync(QueueName, ExchangeName, "event.charger.faulted");
            await _channel.QueueBindAsync(QueueName, ExchangeName, "event.charger.recovered");
            await _channel.QueueBindAsync(QueueName, ExchangeName, "event.authorization.request");



            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, args) =>
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    

                    switch (args.RoutingKey)
                    {
                        case "event.session.started":
                            var Startservice = scope.ServiceProvider.GetRequiredService<TransactionEvtHandler>();
                            var started = JsonSerializer.Deserialize<SessionStartEvent>(json) ?? 
                            throw new InvalidOperationException("Invalid SessionStartEvent payload");
                            await Startservice.HandleSessionStarted(started);
                            break;

                        case "event.session.stopped":
                            var Stopservice = scope.ServiceProvider.GetRequiredService<TransactionEvtHandler>();
                            var stopped = JsonSerializer.Deserialize<SessionStopEvent>(json) ??
                            throw new InvalidOperationException("Invalid SessionStopEvent payload"); ;
                            await Stopservice.HandleSessionStopped(stopped);
                            break;

                        case "event.meter.value":
                            var meterService = scope.ServiceProvider.GetRequiredService<MeterValueEvtHandler>();
                            var meter = JsonSerializer.Deserialize<MeterValueEvent>(json) ??
                            throw new InvalidOperationException("Invalid MeterValueEvent payload"); ;
                            await meterService.HandleMeterValue(meter);
                            break;
                        case "event.charger.faulted":
                            var faultService = scope.ServiceProvider.GetRequiredService<StatusEvtHandler>();
                            var fault = JsonSerializer.Deserialize<ChargerFaultEvent>(json) ??
                            throw new InvalidOperationException("Invalid ChargerFaultEvent payload"); ;
                            await faultService.HandleChargerFault(fault);
                            break;
                        case "event.charger.recovered":
                            var recoverService = scope.ServiceProvider.GetRequiredService<StatusEvtHandler>();
                            var result = JsonSerializer.Deserialize<ChargerRecoverEvent>(json) ??
                            throw new InvalidOperationException("Invalid ChargerRecoverEvent payload"); ;
                            await recoverService.HandleChargerRecovered(result);
                            break;
                        case "event.authorization.request":
                            var authService = scope.ServiceProvider.GetRequiredService<AuthEvtHandler>();
                            var req = JsonSerializer.Deserialize<AuthReqEvent>(json) ??
                            throw new InvalidOperationException("Invalid AuthReqEvent payload"); ;
                            await authService.HandleAuthRequest(req);
                            break;
                        default:
                            _logger.LogWarning("Unhandled routing key: {RoutingKey}", args.RoutingKey);
                            break;

                    }

                    await _channel.BasicAckAsync(args.DeliveryTag, false);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Invalid JSON, discarding message");
                    await _channel.BasicNackAsync(args.DeliveryTag, false, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing event {RoutingKey}", args.RoutingKey);
                    await _channel.BasicNackAsync(args.DeliveryTag, false, true);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer
            );

            stoppingToken.Register(async () =>
            {
                _logger.LogInformation("Shutting down RMQ consumer...");
                await _channel.CloseAsync();
                await _connection.CloseAsync();
            });
        }
    }
}
