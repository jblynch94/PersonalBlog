using System.ComponentModel.DataAnnotations;

namespace PersonalBlog.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long.", MinimumLength = 2)]
        public string? Name { get; set; }


        //nav properties
       public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();

    }
}
