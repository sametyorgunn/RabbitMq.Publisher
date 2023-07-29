using Microsoft.VisualBasic;
using System;

namespace RabbitMq.ExcelCreate.Models
{
    public enum FileStatus
    {
        Creating,
        Completed
    }
    public class UserFile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public FileStatus Status { get; set; }
    }
}
