using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppVideoCamersOperzal.Models.Entities
{
    public class VideoCamera
    {
        [Key]
        public int id { get; set; }    
        
        public string orgCode { get; set; }

        [ForeignKey("orgCode")]
        public Organization organization { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string url { get; set; }

        public string user { get; set; }

        public string password { get; set; }

        public bool disabled { get; set; } = false;

        public string disabledMessage { get; set; }

        public long dateCreate { get; set; } = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

    }
}
