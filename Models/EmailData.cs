using System.ComponentModel.DataAnnotations;

namespace PersonalBlog.Models
{
    public class EmailData
    {
        [Required]
        public string EmailAddress { get; set; } = "";
        [Required]
        public string Subject { get; set; } = "";
        [Required]
        public string Body { get; set; } = "";
        
    }
}
