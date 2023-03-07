using PersonalBlog.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalBlog.Models
{
    public class BlogPost
    {
        //Primary Key
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long.", MinimumLength = 2)]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastUpdated { get; set; }

        //Foreign Key
        public int CategoryId { get; set; }

        public string? Slug { get; set; }

        public string? Abstract { get; set; }

        public bool IsDeleted { get; set; }

        [DisplayName("Published")]
        public bool IsPublished { get; set; }

        //property for storing image
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }

        //property for passing file info from the form(html) to the post.
        //also not saved in the database via [notmapped] attribute
        [NotMapped]
        public IFormFile? BlogPostImage { get; set; }

        //Nav properties
        public virtual Category? Category {get;set;}

        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();

    }
}
