using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalBlog.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long.", MinimumLength = 2)]
        public string? Name { get; set; }

        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long.", MinimumLength = 2)]
        public string? Description { get; set; }

        //property for storing image
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }

        //property for passing file info from the form(html) to the post.
        //also not saved in the database via [notmapped] attribute
        [NotMapped]
        public IFormFile? CategoryImage { get; set; }

        //nav properties
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();
    }
}
