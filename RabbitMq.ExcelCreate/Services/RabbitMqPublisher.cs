using RabbitMQ.Client;
using Shared;
using System.Text;
using System.Text.Json;

namespace RabbitMq.ExcelCreate.Services
{
    public class RabbitMqPublisher
    {
        private readonly RabbitMQClientService _services;

        public RabbitMqPublisher(RabbitMQClientService services)
        {
            _services = services;
        }

        public void Publish(CreateExcelMessage createExcelMessage)
        {
            var channel = _services.Connect();
            var bodystring = JsonSerializer.Serialize(createExcelMessage);
            var bodybyte = Encoding.UTF8.GetBytes(bodystring);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(exchange: RabbitMQClientService.exchangeName, routingKey: RabbitMQClientService.routingExcel, basicProperties: properties, body: bodybyte);
        }
    }

}
