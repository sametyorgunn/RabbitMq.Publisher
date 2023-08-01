using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMq.ExcelCreate.Models;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace RabbitMq.ExcelCreate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _appDbContex;

        public FilesController(AppDbContext appDbContex)
        {
            _appDbContex = appDbContex;
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file,int FileId)
        {
            if(file is not { Length:>0})
            {
                return BadRequest();
            }
            var userfile = await _appDbContex.UserFiles.FirstAsync(x => x.Id == FileId);
            var filepath = userfile.FileName + Path.GetExtension(userfile.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files",filepath);
            using FileStream stream = new(path, FileMode.Create);
            await file.CopyToAsync(stream);

            userfile.CreatedDate = DateTime.Now;
            userfile.FilePath = filepath;
            userfile.Status = FileStatus.Completed;

            await _appDbContex.SaveChangesAsync();
            return Ok();
        }
    }
}
