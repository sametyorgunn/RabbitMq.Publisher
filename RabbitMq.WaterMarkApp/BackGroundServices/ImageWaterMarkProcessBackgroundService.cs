using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMq.WaterMarkApp.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMq.WaterMarkApp.BackGroundServices
{
    public class ImageWaterMarkProcessBackgroundService : BackgroundService
    {
        private readonly RabbitMQClientServices _rabbitMQClientServices;
        private readonly ILogger<ImageWaterMarkProcessBackgroundService> _logger;
        private IModel _channel;

        public ImageWaterMarkProcessBackgroundService(RabbitMQClientServices rabbitMQClientServices, ILogger<ImageWaterMarkProcessBackgroundService> logger)
        {
            _rabbitMQClientServices = rabbitMQClientServices;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientServices.Connect();
            _channel.BasicQos(0, 1, false);
            return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMQClientServices.queueName, false, consumer);
            consumer.Received += Consumer_Received;
            return Task.CompletedTask;
        }

        private Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            try
            {
                var productcreatedevent = JsonConvert.DeserializeObject<ProductImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", productcreatedevent.ImageName);
                var sitename = "www.samet.com";
                using var img = Image.FromFile(path);
                using var graphic = Graphics.FromImage(img);
                var font = new Font(FontFamily.GenericMonospace, 10, FontStyle.Bold, GraphicsUnit.Pixel);
                var textsize = graphic.MeasureString(sitename, font);
                var color = Color.FromArgb(128, 255, 255, 255);
                var brush = new SolidBrush(color);
                var position = new Point(img.Width - ((int)textsize.Width + 30), img.Height - ((int)textsize.Height + 30));
                graphic.DrawString(sitename, font, brush, position);
                img.Save("wwwroot/images/watermarks/" + productcreatedevent.ImageName);

                img.Dispose();
                graphic.Dispose();
                _channel.BasicAck(@event.DeliveryTag, false);



            }
            catch(Exception ex)
            {
                throw;
            }
            return Task.CompletedTask;


        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
