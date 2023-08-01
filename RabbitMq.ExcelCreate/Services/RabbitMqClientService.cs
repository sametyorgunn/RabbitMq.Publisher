using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace RabbitMq.ExcelCreate.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static string exchangeName = "ExcelDirectExchange";
        public static string routingExcel = "Excel-route-image";
        public static string queueName = "queue-Excel-file";

        private readonly ILogger<RabbitMQClientService> _logger;

        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            if (_channel is { IsOpen: true })
            {
                return _channel;
            }
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchangeName, type: "direct", true, false);
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(exchange: exchangeName, queue: queueName, routingKey: routingExcel);
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
