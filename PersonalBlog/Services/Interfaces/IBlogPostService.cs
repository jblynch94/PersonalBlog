using PersonalBlog.Models;

namespace PersonalBlog.Services.Interfaces
{
    public interface IBlogPostService
    {
        public Task<bool> ValidateSlugAsync(string title, int blogId);

        public Task AddTagToBlogPostAsync(int tagId, int blogPostId);

        public Task<bool> IsTagInBlogPostAsync(int tagId, int blogPostId);

        public Task RemoveTagFromBlogPostAsync(int tagId, int blogPostId);

        public Task<IEnumerable<Tag>> GetBlogPostTagsAsync(int blogPostId);

        public Task<List<Category>> GetCategoriesAsync();

        public Task<List<BlogPost>> GetAllBlogPostAsync(int count); //All posts regardless of IsDeleted or Ispublished

        public Task<List<BlogPost>> GetPopularBlogPostAsync(int count); //Defined by the number of comments made

        public Task<List<BlogPost>> GetRecentBlogPostsAsync(int count); //Defined by the date created

    }
}
