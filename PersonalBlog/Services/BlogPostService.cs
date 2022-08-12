using Microsoft.EntityFrameworkCore;
using PersonalBlog.Data;
using PersonalBlog.Extentions;
using PersonalBlog.Models;
using PersonalBlog.Services.Interfaces;

namespace PersonalBlog.Services
{
    public class BlogPostService : IBlogPostService
    {

        private readonly ApplicationDbContext _context;

        public BlogPostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidateSlugAsync(string title, int blogId)
        {
            try
            {
                string newSlug = title.Slugify();

                if(blogId == 0)
                {
                    return !(await _context.BlogPosts.AnyAsync(b=>b.Slug == newSlug));
                }
                else
                {
                    BlogPost blogPost = await _context.BlogPosts.AsNoTracking().FirstAsync(b => b.Id == blogId);

                    string oldSlug = blogPost.Slug!;

                    if(!string.Equals(oldSlug, newSlug))
                    {
                        return !(await _context.BlogPosts.AnyAsync(b => b.Slug == newSlug));
                    }
                }

                return true;

            }
            catch (Exception)
            {

                throw;
            }
            
        }
        public async Task<bool> IsTagInBlogPostAsync(int tagId, int blogPostId)
        {
            try
            {
                Tag? tag = await _context.Tags.FindAsync(tagId);

                return (await _context.BlogPosts.FirstOrDefaultAsync(c => c.Id == blogPostId))!.Tags.Contains(tag!);
                                     
            }
            catch
            {
                throw;
            }
        }
        public async Task AddTagToBlogPostAsync(int tagId, int blogPostId)
        {
            try
            {
                //check to see if the catecgory has already been added
                if (!await IsTagInBlogPostAsync(tagId, blogPostId))
                {
                    //add the category to the database
                    BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);
                    Tag? tag = await _context.Tags.FindAsync(tagId);

                    if (blogPost != null && tag != null)
                    {
                        blogPost.Tags.Add(tag);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public async Task RemoveTagFromBlogPostAsync(int tagId, int blogPostId)
        {
            //Remove the category to the database
            BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);
            Tag? tag = await _context.Tags.FindAsync(tagId);

            if (blogPost != null && tag != null)
            {
                blogPost.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Tag>> GetBlogPostTagsAsync(int blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts.Include(c => c.Tags)
                                                         .FirstOrDefaultAsync(c => c.Id == blogPostId);
                return blogPost!.Tags;
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<Category>> GetCategoriesAsync()
        {
            {
                List<Category> categories = new List<Category>();

                try
                {
                    categories = await _context.Categories.ToListAsync();
                }
                catch
                {
                    throw;
                }
                return categories;
            }
        }

        public async Task<List<BlogPost>> GetAllBlogPostAsync(int count)
        {
            {
                List<BlogPost> blogPost = new List<BlogPost>();

                try
                {
                    blogPost = await _context.BlogPosts.ToListAsync();
                }
                catch
                {
                    throw;
                }
                return blogPost;
            }
        }

        public async Task<List<BlogPost>> GetPopularBlogPostAsync(int count)
        {
            {
                List<BlogPost> blogPost = new List<BlogPost>();

                try
                {
                    blogPost = await _context.BlogPosts.OrderBy(c=>c.Comments.Count)
                                                       .ToListAsync();
                }
                catch
                {
                    throw;
                }
                return blogPost;
            }
        }

        public async Task<List<BlogPost>> GetRecentBlogPostsAsync(int count)
        {
            {
                List<BlogPost> blogPost = new List<BlogPost>();

                try
                {
                    blogPost = await _context.BlogPosts.OrderBy(c => c.Created)
                                                       .ToListAsync();
                }
                catch
                {
                    throw;
                }
                return blogPost;
            }
        }
    }
}
