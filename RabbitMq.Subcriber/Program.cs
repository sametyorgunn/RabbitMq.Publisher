using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMq.Subcriber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://zcjsooff:xTtEdMEjx_LXzSVivzd9vNgbH5F5IA8w@cougar.rmq.cloudamqp.com/zcjsooff ");
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare("Hello-queue", true, false, false);
            var consumer = new EventingBasicConsumer(channel);

            channel.BasicQos(0, 5, true);   /* burda true değer var ise mesajlar subcriberlara toplam değer pay 
                                             * edilir örneğin 3 birine 2 birine yada ordaki değeri 6 varsayarsak
                                             * 3 birine 3 birine
                                             * false ise ordaki değer ne ise o aktarılı yani 5 birine 5 birine


            channel.BasicConsume("Hello-queue", false,consumer);
            /*burdaki true olan bool değer mesaj gelir gelmez kuyruktan
              mesajı siler.Eğer false olursa mesajı tutar.
            */


            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("Gelen message : " + message);
                channel.BasicAck(e.DeliveryTag,false);  //rabbitmq dan gelen tag a göre mesajı inceler
                //false olan değer ilgili mesajın değerin silinmesini sağlar true olursa başka mesajlar var ise 
                //onlarıda sildirir.
            };
        }

     
    }
}
