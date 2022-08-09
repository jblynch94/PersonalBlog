using System.ComponentModel.DataAnnotations;

namespace PersonalBlog.Models
{
    public class Comment
    {
        //primary key
        public int Id { get; set; }

        //Foreign Key
        public int BlogPostId { get; set; }

        //Foreign Key
        [Required]
        public string? AuthorId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastUpdated { get; set; }

        public string? UpdateReason { get; set; }

        [StringLength(5000, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long.", MinimumLength = 2)]
        public string? Body { get; set; }


        //nav properties
        public virtual BlogPost? BlogPost { get; set; }

        public virtual BlogUser? Author { get; set; }


    }
}
