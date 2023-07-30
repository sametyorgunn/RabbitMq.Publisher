using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RabbitMq.ExcelCreate.Models
{
    public enum FileStatus
    {
        Creating,
        Completed
    }
    public class UserFile
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public FileStatus Status { get; set; }


        [NotMapped]
        public string GetCreatedDate => CreatedDate.HasValue ? CreatedDate.Value.ToShortDateString() : "-";
    }
}
