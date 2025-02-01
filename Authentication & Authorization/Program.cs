using Authentication___Authorization.Data;
using Authentication___Authorization.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Authentication___Authorization.Services;
using Authentication___Authorization.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options=>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.SlidingExpiration = true; // Enable  sliding expiration for session cookies
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Use 'Always' if using HTTPS
    options.Cookie.SameSite = SameSiteMode.Lax;

    // Make persistent cookies valid for 14 days
    options.ExpireTimeSpan = TimeSpan.FromDays(14);

    //this for browser only not tabs ----------note-------
    // Configure events to handle session cookies
    options.Events = new CookieAuthenticationEvents
    {
        OnSigningIn = context =>
        {
            var isPersistent = context.Properties.IsPersistent;
            if (!isPersistent)
            {
                // Set the cookie expiration to null for session cookies
                context.Properties.ExpiresUtc = null;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddTransient<IEmailSender, EmailSender>();
//builder.Services.Configure<EmailSettings>(_configuration.GetSection("EmailSettings"));




builder.Services.AddControllersWithViews();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
