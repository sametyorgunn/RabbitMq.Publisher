using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RabbitMq.ExcelCreate.Models;
using RabbitMq.ExcelCreate.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMq.ExcelCreate.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RabbitMqPublisher _rabbitMqPublisher;

        public ProductController(AppDbContext appDbContext, UserManager<IdentityUser> userManager, RabbitMqPublisher rabbitMqPublisher)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateProductExcel()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userid = _userManager.GetUserIdAsync(user);
            var stringUserid = userid.ToString();
            var filename = $"product-excel-{Guid.NewGuid().ToString().Substring(1, 10)}";
            UserFile file = new UserFile
            {
                UserId = stringUserid,
                FileName = filename,
                Status = FileStatus.Creating
            };
            await _appDbContext.UserFiles.AddAsync(file);
            await _appDbContext.SaveChangesAsync();

            _rabbitMqPublisher.Publish(new Shared.CreateExcelMessage()
            {
                FileId = file.Id,
                Userid = file.UserId
            });

            TempData["startcreatingexcel"] = true;
            return RedirectToAction("Files", "Product");
        }

        public async Task<IActionResult> Files()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            string userid =_userManager.GetUserIdAsync(user.Result).ToString();

            var files = _appDbContext.UserFiles.Where(x => x.UserId == userid).ToList();

            return View(files);
        }
    }
}
