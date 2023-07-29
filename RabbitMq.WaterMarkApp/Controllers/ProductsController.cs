using Microsoft.AspNetCore.Mvc;
using RabbitMq.WaterMarkApp.Models;
using RabbitMq.WaterMarkApp.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RabbitMq.WaterMarkApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly RabbitMqPublisher _rabbitmqpublisher;
        private readonly AppDbContext _appDbContext;

        public ProductsController(AppDbContext appDbContext, RabbitMqPublisher rabbitmqpublisher)
        {
            _appDbContext = appDbContext;
            _rabbitmqpublisher = rabbitmqpublisher;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(Product product)
        {
            if (product.imageFile.Length > 0)
            {
                var randomimageName = Guid.NewGuid() + Path.GetExtension(product.imageFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", randomimageName);

                await using FileStream stream = new(path, FileMode.Create);
                await product.imageFile.CopyToAsync(stream);
                _rabbitmqpublisher.Publish(new ProductImageCreatedEvent()
                {
                    ImageName = randomimageName
                });
                product.imageName = randomimageName;
            }
            
            _appDbContext.Products.Add(product);
            _appDbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
