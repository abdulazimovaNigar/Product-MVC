using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductMVC.Abstraction;
using ProductMVC.Contexts;
using ProductMVC.Models;
using ProductMVC.Services;

namespace ProductMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var conn = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddDbContext<ProniaDbContext>(opt =>
            {
                opt.UseSqlServer(conn);
            });

            builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 6;
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequireUppercase = true;
                opt.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ProniaDbContext>().AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
            });

            builder.Services.AddControllersWithViews();
            var app = builder.Build();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
              name: "areas",
              pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
            );


            app.MapDefaultControllerRoute();

            app.Run();

        }
    }
}
