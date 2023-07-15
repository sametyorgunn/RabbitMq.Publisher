using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMq.Publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://zcjsooff:xTtEdMEjx_LXzSVivzd9vNgbH5F5IA8w@cougar.rmq.cloudamqp.com/zcjsooff ");
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare("Hello-queue",true,false,false);


            //coklu mesaj gönderme
            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"hello world{x}";
                var messagebody = Encoding.UTF8.GetBytes(message); //messagı byte a çevirdik.
                channel.BasicPublish(string.Empty, "Hello-queue", null, messagebody);
                Console.WriteLine($"Mesaj gönderilmiştir - {x}");
            });

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
