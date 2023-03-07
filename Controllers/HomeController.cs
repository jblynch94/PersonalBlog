
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalBlog.Data;
using PersonalBlog.Models;
using PersonalBlog.Services;
using PersonalBlog.Services.Interfaces;
using System.Diagnostics;

namespace PersonalBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBlogPostService _blogPostService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IEmailSender _emailService;




        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IBlogPostService blogPostService, UserManager<BlogUser> userManager, IEmailSender emailService)
        {
            _logger = logger;
            _context = context;
            _blogPostService = blogPostService;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<IActionResult> AuthorPage()
        {
            //TODO: create service to get blogposts

            List<BlogPost> posts = (await _blogPostService.GetAllBlogPostAsync()).Where(b=>b.IsPublished ==true).ToList();


            return View(posts);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult ContactMe()
        {
            EmailData emailData = new EmailData()
            {
                EmailAddress = User.Identity!.Name!,
                Subject = "",
                Body = ""
                
            };
            return View(emailData);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ContactMe(EmailData data)
        {
            
            if (ModelState.IsValid)
            {
                

                try
                {
                    await _emailService.SendEmailAsync(data.EmailAddress, data.Subject, data.Body);
                    return RedirectToAction("ContactMe", "Home", new { swalMessage = "Success: Email Sent!" });
                }
                catch
                {
                    return RedirectToAction("ContactMe", "Home", new { swalMessage = "Error: Email Send Failed!" });
                    throw;
                }

            }
            return View(data);
        }

    }
}