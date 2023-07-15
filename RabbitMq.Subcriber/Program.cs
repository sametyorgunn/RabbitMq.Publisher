using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace RabbitMq.Subcriber
{
    public class Program
    {
        //fanout da tüm instancelara aynı mesajlar iletilir 
        //direct de instaclara değerler pay edilerek gönderilir
        static void Main(string[] args)
        {
            HeadersExchangeGet();
        }
        public static void HeadersExchangeGet()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://ydjrybtk:Pv1BXcTsd8qe6MNRpWJr0rTvZQ4vWidU@cougar.rmq.cloudamqp.com/ydjrybtk");
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.BasicQos(0, 1, true);
            var consumer = new EventingBasicConsumer(channel);
            var queueName = channel.QueueDeclare().QueueName;

            Dictionary<string, Object> headers = new Dictionary<string, Object>();
            headers.Add("format", "pdf");
            headers.Add("shape", "a4");
            headers.Add("x-match", "all");
            channel.QueueBind(queueName, "HeaderExchange",string.Empty,headers);

            channel.BasicConsume(queueName, false, consumer);
            Console.WriteLine("loglar dinleniyor.");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Product product = JsonSerializer.Deserialize<Product>(message);
                Thread.Sleep(1500);
                Console.WriteLine($"Gelen message :{product.Id} - {product.Name} - {product.Price} - {product.Stock}");
                channel.BasicAck(e.DeliveryTag, false);
            };
            Console.ReadLine();
        }

        public static void TopicExchangeGet()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://ydjrybtk:Pv1BXcTsd8qe6MNRpWJr0rTvZQ4vWidU@cougar.rmq.cloudamqp.com/ydjrybtk");
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.BasicQos(0, 1, true);  
            var consumer = new EventingBasicConsumer(channel);
            var queueName = channel.QueueDeclare().QueueName;
            var route = "*.Error.*";
            channel.QueueBind(queueName,"LogsTopic",route);

            channel.BasicConsume(queueName, false, consumer);
            Console.WriteLine("loglar dinleniyor.");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1500);
                Console.WriteLine("Gelen message : " + message);
                channel.BasicAck(e.DeliveryTag, false); 
            };
            Console.ReadLine();
        }
        public static void DirectExchangeGet()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://ydjrybtk:Pv1BXcTsd8qe6MNRpWJr0rTvZQ4vWidU@cougar.rmq.cloudamqp.com/ydjrybtk");
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            //channel.ExchangeDeclare("Logs-fanout", durable: true, type: ExchangeType.Fanout); //exchange i burda olustursakta olur olusturmasakta olur 
            //publisher da zaten olustrduk oyuzden burda olusturmicam 

            //channel.QueueDeclare("Hello-queue", true, false, false);

           

            


            channel.BasicQos(0, 1, true);   /* burda true değer var ise mesajlar subcriberlara toplam değer pay 
                                             * edilir örneğin 3 birine 2 birine yada ordaki değeri 6 varsayarsak
                                             * 3 birine 3 birine
                                             * false ise ordaki değer ne ise o aktarılı yani 5 birine 5 birine */
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume("direct-queue-Info", false, consumer);
            Console.WriteLine("loglar dinleniyor.");

            //channel.QueueBind("direct-queue-Info", "LogsDirect", "route-logInfo", null);
            /*burdaki true olan bool değer mesaj gelir gelmez kuyruktan
              mesajı siler.Eğer false olursa mesajı tutar.
            */


            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1500);
                Console.WriteLine("Gelen message : " + message);
                channel.BasicAck(e.DeliveryTag, false);  //rabbitmq dan gelen tag a göre mesajı inceler
                //false olan değer ilgili mesajın değerin silinmesini sağlar true olursa başka mesajlar var ise 
                //onlarıda sildirir.
            };
            Console.ReadLine();
        }
       public static void FanoutExchangeGet()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://ydjrybtk:Pv1BXcTsd8qe6MNRpWJr0rTvZQ4vWidU@cougar.rmq.cloudamqp.com/ydjrybtk");
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            //channel.ExchangeDeclare("Logs-fanout", durable: true, type: ExchangeType.Fanout); //exchange i burda olustursakta olur olusturmasakta olur 
            //publisher da zaten olustrduk oyuzden burda olusturmicam 

            //channel.QueueDeclare("Hello-queue", true, false, false);

            var randomqueueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(randomqueueName, "Logsfanout", "", null);


            channel.BasicQos(0, 1, true);   /* burda true değer var ise mesajlar subcriberlara toplam değer pay 
                                             * edilir örneğin 3 birine 2 birine yada ordaki değeri 6 varsayarsak
                                             * 3 birine 3 birine
                                             * false ise ordaki değer ne ise o aktarılı yani 5 birine 5 birine */
            var consumer = new EventingBasicConsumer(channel);



            channel.BasicConsume(randomqueueName, false, consumer);
            Console.WriteLine("loglar dinleniyor.");
            /*burdaki true olan bool değer mesaj gelir gelmez kuyruktan
              mesajı siler.Eğer false olursa mesajı tutar.
            */


            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1500);
                Console.WriteLine("Gelen message : " + message);
                channel.BasicAck(e.DeliveryTag, false);  //rabbitmq dan gelen tag a göre mesajı inceler
                //false olan değer ilgili mesajın değerin silinmesini sağlar true olursa başka mesajlar var ise 
                //onlarıda sildirir.
            };
        }

    }
}
