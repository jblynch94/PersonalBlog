﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalBlog.Models
{
    public class BlogUser : IdentityUser
    {
        [Required]
        [DisplayAttribute(Name = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long.", MinimumLength = 2)]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long.", MinimumLength = 2)]
        public string? LastName { get; set; }

        [NotMapped]
        public string? FullName { get { return $"{FirstName} {LastName}"; } }

        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }

        //property for passing file info from the form(html) to the post.
        //also not saved in the database via [notmapped] attribute
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        //Nav properties
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
    }
}
