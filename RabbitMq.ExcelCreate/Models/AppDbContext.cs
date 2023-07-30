using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RabbitMq.ExcelCreate.Models
{
    public class AppDbContext:IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) :base(dbContextOptions)
        {
        }
        public DbSet<UserFile> UserFiles { get; set; }
    }
}
