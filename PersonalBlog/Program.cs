using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PersonalBlog.Data;
using PersonalBlog.Models;
using PersonalBlog.Services;
using PersonalBlog.Services.Interfaces;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = DataUtility.GetConnectionString(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString,
    o=>o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
    ));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//Custom Services
builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddScoped<IBlogPostService, BlogPostService>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

//--------------

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddMvc();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CFBlogNet6 API",
        Version = "v1",
        Description = "Serving up Blog data using .Net 6",
        Contact = new OpenApiContact
        {
            Name = "Jacob Lynch",
            Email = "JacobLynchDev@gmail.com",
           
        }
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

//configure a cors policy
builder.Services.AddCors(obj =>
{
    obj.AddPolicy("DefaultPolicy",
        builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});


var app = builder.Build();
app.UseCors("DefaultPolicy");

var scope = app.Services.CreateScope();
await DataUtility.SeedDataAsync(scope.ServiceProvider);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PublicAPI v1");
    c.InjectStylesheet("/css/swagger.css");
    c.InjectJavascript("/js/swagger.js");
    c.DocumentTitle = "PersonalBlog Public API";
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name:"custom",
    pattern: "Content/{slug}",
    defaults: new {contoller = "BlogPosts", action = "Details"});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=AuthorPage}/{id?}");

app.MapRazorPages();

app.Run();
