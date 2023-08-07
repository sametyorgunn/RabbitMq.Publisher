using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileCreateWorkerService
{
    public class Worker : BackgroundService
    {
       
        private readonly ILogger _logger;   
        private readonly RabbitMQClientService _rabbitmqClientService;
        private IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger logger, RabbitMQClientService rabbitmqClientService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _rabbitmqClientService = rabbitmqClientService;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel =  _rabbitmqClientService.Connect();
            _channel.BasicQos(0, 1, false);
          
            return Task.CompletedTask;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMQClientService.queueName,false, consumer);
            consumer.Received += Consumer_Received;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            await Task.Delay(5000);
            var createexcelmessage = JsonConvert.DeserializeObject<CreateExcelMessage>(Encoding.UTF8.GetString(@event.Body.ToArray()));

            using var ms = new MemoryStream();
            var wb = new XLWorkbook();
            var ds = new DataSet();
            ds.Tables.Add(GetTable("products"));

            wb.Worksheets.Add(ds);
            wb.SaveAs(ms);

            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(new ByteArrayContent(ms.ToArray()), "file", Guid.NewGuid().ToString() + ".xlxs");

            var baseurl = "https://localhost:44383/api/Files";

            using(var httpclient =  new HttpClient())
            {
                var response = await httpclient.PostAsync($"{baseurl}?fileId={createexcelmessage.FileId}",multipartFormDataContent);
                if(response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"File(Id:{createexcelmessage.FileId}) was createad by succesfull");
                    _channel.BasicAck(@event.DeliveryTag, false);
                }
            }
        }

        private DataTable GetTable(string tableName)
        {
            List<FileCreateWorkerService.Models.Product> products;

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AdventureWorks2019Context>();
                products = context.Products.ToList();

            }

            DataTable table = new DataTable
            {
                TableName = tableName
            };
            table.Columns.Add("ProductId",typeof(int));
            table.Columns.Add("Name",typeof(string));
            table.Columns.Add("ProductNumber",typeof(string));
            table.Columns.Add("Color",typeof(string));

            products.ForEach(x =>
            {
                table.Rows.Add(x.ProductId,x.Name,x.ProductNumber,x.Color);
            });
            return table;
        }
    }
}
