using RabbitMQ.Client;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using static RabbitMq.Publisher.Program;

namespace RabbitMq.Publisher
{
    public class Program
    {
        public enum Logs
        {
            Critical = 1,
            Error = 2,
            Warning =3,
            Info = 4
        }
        static void Main(string[] args)
        {
            HeaderExchange();
        }
        public static void HeaderExchange()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://ydjrybtk:Pv1BXcTsd8qe6MNRpWJr0rTvZQ4vWidU@cougar.rmq.cloudamqp.com/ydjrybtk");
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare("HeaderExchange", durable: true, type: ExchangeType.Headers); 
            Dictionary<string,Object> headers= new Dictionary<string,Object>();
            headers.Add("format","pdf");
            headers.Add("shape","a4");


            var product = new Product { Id = 1, Name = "deneme", Price = 10, Stock = 2 };
            var seralizejsonproduct = JsonSerializer.Serialize(product);

            var properties = channel.CreateBasicProperties();
            properties.Headers = headers;
            properties.Persistent = true; //mesajları kalıcı hale getirir.
            channel.BasicPublish("HeaderExchange",string.Empty,properties,Encoding.UTF8.GetBytes(seralizejsonproduct));
            Console.WriteLine("mesaj gönderilmiştir");



        }
        public static void TopicExchange()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://ydjrybtk:Pv1BXcTsd8qe6MNRpWJr0rTvZQ4vWidU@cougar.rmq.cloudamqp.com/ydjrybtk");
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare("LogsTopic", durable: true, type: ExchangeType.Topic); //durable true exchange ı saklamasına yarar.

            Random rand = new Random();
            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
 
                Logs log1 = (Logs)rand.Next(1, 5);
                Logs log2 = (Logs)rand.Next(1, 5);
                Logs log3 = (Logs)rand.Next(1, 5);

                var route = $"{log1}.{log2}.{log3}";
                string message = $"Log-type {log1} + {log2} + {log3}";
                var messagebody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("LogsTopic", route, null, messagebody);
                Console.WriteLine($"Log gönderilmiştir - {message}");

                Console.ReadLine();
            });

        }
        public static void DirectExchange()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://ydjrybtk:Pv1BXcTsd8qe6MNRpWJr0rTvZQ4vWidU@cougar.rmq.cloudamqp.com/ydjrybtk");
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare("Logsdirect", durable: true, type: ExchangeType.Direct); //durable true exchange ı saklamasına yarar.

            Enum.GetNames(typeof(Logs)).ToList().ForEach(x =>
            {
                var route = $"route-log{x}";

                var queueName = $"direct-queue-{x}";
                channel.QueueDeclare(queueName, true, false, false);
                channel.QueueBind(queueName, "Logsdirect",route, null);
            });


            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                Logs log = (Logs)new Random().Next(1, 5);
                string message = $"Log-type {log}";
                var messagebody = Encoding.UTF8.GetBytes(message);

                var route = $"route-log{log}";
                channel.BasicPublish("Logsdirect", route, null, messagebody);
                Console.WriteLine($"Log gönderilmiştir - {message}");
            });

            


        }
        public static void FanoutExchange()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://ydjrybtk:Pv1BXcTsd8qe6MNRpWJr0rTvZQ4vWidU@cougar.rmq.cloudamqp.com/ydjrybtk");
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            /* channel.QueueDeclare("Hello-queue",true,false,false);*/ //publisher tarafından kuyruk olusturulmicak
            channel.ExchangeDeclare("Logsfanout", durable: true, type: ExchangeType.Fanout); //durable true exchange ı saklamasına yarar.

            //coklu mesaj gönderme
            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"Log {x}";
                var messagebody = Encoding.UTF8.GetBytes(message); //messagı byte a çevirdik.
                channel.BasicPublish("Logsfanout", "", null, messagebody);
                Console.WriteLine($"Loglar gönderilmiştir - {x}");
            });
            Console.ReadLine();

            /*durable yani birinci bool değer eğer proje restart olsa 
             * bile veriler silinmez kaydedilir eğer false olursa 
             * memoryde tutar ve her defasında silini.*/

            /* ikinci bool değer ise true ise sadece aynı proje
             * üzerinden mesaj gönderilir kuyruğa.Eğer false ise
             * diğer projeler üzerindende mesaj gönderilebilir kuyruğa
             * */

            /* sonucu bool değer true ise son alıcı yani subcriber sistemden
             * düşerse yada yok olursa otomatik kuyruğu siler.
             * eğer false ise silmez */




            //tekli mesaj gönderme

            //string message = "helo world";
            //var messagebody = Encoding.UTF8.GetBytes(message); //messagı byte a çevirdik.
            //channel.BasicPublish(string.Empty, "Hello-queue", null,messagebody);
            //Console.WriteLine("Mesaj gönderilmiştir");
            //Console.ReadLine();



        }
    }
}
