using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using iMicroFin.DAO;
namespace iMicroFin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // Make configuration available throughout the app
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            // Add Session Support
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = ".MicroFin.Session";
            });
            // Add HttpContextAccessor (needed for session access)
            builder.Services.AddHttpContextAccessor();

            // Add Cookie Authentication
            builder.Services.AddAuthentication("MyAuthCookie")
                .AddCookie("MyAuthCookie",options =>
                {
                    options.LoginPath = "/App/Login"; // Redirect here if not authenticated
                    options.LogoutPath = "/App/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8); // Cookie expiration
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                    options.Cookie.Name = ".iMicroFin.Auth";
                });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });

            builder.Services.AddHttpContextAccessor();

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
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            ConfigHelper.Initialize(builder.Configuration);

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=App}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
