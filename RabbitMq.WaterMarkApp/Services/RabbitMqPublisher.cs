using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMq.WaterMarkApp.Services
{
    public class RabbitMqPublisher
    {
        private readonly RabbitMQClientServices _services;

        public RabbitMqPublisher(RabbitMQClientServices services)
        {
            _services = services;
        }

        public void Publish(ProductImageCreatedEvent events)
        {
            var channel = _services.Connect();
            var bodystring = JsonSerializer.Serialize(events);
            var bodybyte = Encoding.UTF8.GetBytes(bodystring);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(exchange: RabbitMQClientServices.exchangeName, routingKey: RabbitMQClientServices.routingWaterMark, basicProperties: properties, body: bodybyte);
        }
    }
}
