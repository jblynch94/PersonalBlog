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
    }
}
