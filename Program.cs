using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            //google authentication
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection =
                    config.GetSection("Authentication:Google");
                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                });

            // Add services to the container.
            builder.Services.AddMvc().AddRazorRuntimeCompilation();

            //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration["ConnectionStrings:IntexConnection"]));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Use(async (ctx, next) =>
            {
                string csp = "default-src 'self'; " +
                             "script-src 'self' https://kit.fontawesome.com 'unsafe-inline' https://apis.google.com; " +
                             "connect-src 'self' ws://localhost:57798 http://localhost:57798 https://ka-f.fontawesome.com ws://localhost:62719 http://localhost:62719 wss://localhost:44300; " +
                             "style-src 'self' 'unsafe-inline'; " +
                             "font-src 'self' https://fonts.gstatic.com https://ka-f.fontawesome.com; " +
                             "img-src 'self' data:; ";


                if (ctx.Request.IsHttps || app.Environment.IsDevelopment())
                {
                    ctx.Response.Headers.Add("Content-Security-Policy", csp);
                }

                // Adding X-Content-Type-Options header
                ctx.Response.Headers.Add("X-Content-Type-Options", "nosniff");

                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.Run();
        }
    }
}
