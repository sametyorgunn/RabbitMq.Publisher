using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace RabbitMq.WaterMarkApp.Services
{
    public class RabbitMQClientServices:IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private static string exchangeName = "ImageDirectExchange";
        private static string routingWaterMark = "watermark-route-image";
        private static string queueName = "queue-watermark-image";

        private readonly ILogger<RabbitMQClientServices> _logger;

        public RabbitMQClientServices(ConnectionFactory connectionFactory, ILogger<RabbitMQClientServices> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            Connect();
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            if(_channel is { IsOpen: true })
            {
                return _channel;
            }
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchangeName, type: "direct", true, false);
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(exchange:exchangeName,queue:queueName,routingKey: routingWaterMark);
            _logger.LogInformation("Rabbitmq ile bağlantı kuruldu");
            return _channel;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel.Dispose();

            _connection.Close();
            _connection.Dispose();

            _logger.LogInformation("Rabbitmq ile bağlantı koptu");
        }
    }
}
