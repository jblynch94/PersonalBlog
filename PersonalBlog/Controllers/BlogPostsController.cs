﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalBlog.Data;
using PersonalBlog.Extentions;
using PersonalBlog.Models;
using PersonalBlog.Services.Interfaces;
using X.PagedList;

namespace PersonalBlog.Controllers
{
    [Authorize(Roles="Administrator")]

    public class BlogPostsController : Controller
{
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IBlogPostService _blogPostService;
        public BlogPostsController(ApplicationDbContext context,
                                    IImageService imageService,
                                    UserManager<BlogUser> userManager, 
                                    IBlogPostService blogPostService)
        {
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
            _blogPostService = blogPostService;
        }

        // GET: BlogPosts
        public async Task<IActionResult> Index()
        {

            //todo: use service

            var applicationDbContext = _context.BlogPosts
                                               .Where(b=>b.IsDeleted)
                                               .Include(b => b.Category)
                                               .Include(b=>b.Tags);

            return View(await applicationDbContext.ToListAsync());
        }
        [AllowAnonymous]
        public async Task<IActionResult> SearchIndex(string searchTerm, int? pageNum)
        {

            int pageSize = 4;
            int page = pageNum ?? 1;

            ViewData["searchTerm"] = searchTerm;

            IPagedList<BlogPost> blogPosts = await _blogPostService.Search(searchTerm).ToPagedListAsync(page,pageSize); 

            return View(blogPosts);
        }

        // GET: BlogPosts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            BlogPost? blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .Include(b=>b.Comments)
                    .ThenInclude(c=>c.Author)
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public IActionResult Create()
        {

            ViewData["TagList"] = new MultiSelectList(_context.Tags, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,CategoryId,Abstract,IsPublished,ImageData,ImageType,BlogPostImage,Slug")] BlogPost blogPost, List<int> TagList)
        {
            if (ModelState.IsValid)
            {
                //date
                blogPost.Created = DataUtility.GetPostGresDate(DateTime.Now);
                blogPost.LastUpdated = DataUtility.GetPostGresDate(DateTime.Now);

                //slug
                if (!await _blogPostService.ValidateSlugAsync(blogPost.Title!,blogPost.Id))
                {
                    ModelState.AddModelError("Title", "A similar Title or Slug has already been used!");

                    ViewData["TagList"] = new MultiSelectList(_context.Tags, "Id", "Name",TagList);
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name",blogPost.CategoryId);
                    return View(blogPost);
                }

                blogPost.Slug = blogPost.Title!.Slugify();

                //image
                if (blogPost.BlogPostImage != null)
                {
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.BlogPostImage);
                    blogPost.ImageType = blogPost.BlogPostImage.ContentType;
                }

                _context.Add(blogPost);


                foreach(int tagId in TagList)
                {
                    blogPost.Tags.Add(_context.Tags.Find(tagId)!);
                }
                _context.Add(blogPost);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["TagList"] = new MultiSelectList(_context.Tags, "Id", "Name", TagList);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }


            var blogPost = await _context.BlogPosts.Include(b => b.Tags).FirstOrDefaultAsync(b => b.Id == id);

            //var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }
            ViewData["TagList"] = new MultiSelectList(_context.Tags, "Id", "Name",blogPost.Tags.Select(t=>t.Id));
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Created,LastUpdated,CategoryId,Slug,Abstract,IsDeleted,IsPublished,ImageData,ImageType,BlogPostImage")] BlogPost blogPost, List<int> TagList)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (blogPost.BlogPostImage != null)
                    {
                        blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.BlogPostImage);
                        blogPost.ImageType = blogPost.BlogPostImage.ContentType;
                    }
                    blogPost.Created = DataUtility.GetPostGresDate(blogPost.Created);
                    blogPost.LastUpdated = DataUtility.GetPostGresDate(DateTime.Now);

                    _context.Update(blogPost);


                    //slug
                    if (!await _blogPostService.ValidateSlugAsync(blogPost.Title!, blogPost.Id))
                    {
                        ModelState.AddModelError("Title", "A similar Title or Slug has already been used!");

                        ViewData["TagList"] = new MultiSelectList(_context.Tags, "Id", "Name",TagList);
                        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name",blogPost.CategoryId);
                        return View(blogPost);
                    }

                    blogPost.Slug = blogPost.Title!.Slugify();


                    //tags
                    List<Tag> tags = (await _blogPostService.GetBlogPostTagsAsync(blogPost.Id)).ToList();
                    //Version 1
                    //List<Tag> tags = _context.Tags.Where(t=>t.BlogPosts.Contains(blogPost)).ToList();
                    //Version 2
                    //List<Tag> tags = _context.BlogPosts.FirstOrDefault(b=>b.Id == blogPost.Id)!.Tags.ToList();


                    foreach (Tag tag in tags)
                    {
                        await _blogPostService.RemoveTagFromBlogPostAsync(tag.Id, blogPost.Id);

                    }

                    //blogPost.Tags.Clear();

                    foreach (int tagId in TagList)
                    {
                        await _blogPostService.AddTagToBlogPostAsync(tagId, blogPost.Id);

                    }
                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TagList"] = new MultiSelectList(_context.Tags,"Id", "Name",TagList);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BlogPosts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BlogPosts'  is null.");
            }
            var blogPost = await _context.BlogPosts.FindAsync(id);

            blogPost!.IsDeleted = true;
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
          return (_context.BlogPosts?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> PopularIndex()
        {
            //TODO: create service to get blogposts

            List<BlogPost> posts = (await _blogPostService.GetAllBlogPostAsync()).Where(b => b.IsPublished == true).ToList();


            return View(posts);
        }
        public async Task<IActionResult> RecentIndex()
        {
            //TODO: create service to get blogposts

            List<BlogPost> posts = (await _blogPostService.GetAllBlogPostAsync()).Where(b => b.IsPublished == true).ToList();


            return View(posts);
        }
        public async Task<IActionResult> AllPostsIndex()
        {
            //TODO: create service to get blogposts

            List<BlogPost> posts = (await _blogPostService.GetAllBlogPostAsync()).Where(b => b.IsPublished == true).ToList();


            return View(posts);
        }
    }
}
