using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RabbitMq.WaterMarkApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
        public string? imageName{ get; set; }
        [NotMapped]
        public IFormFile? imageFile { get; set; }
    }
}
