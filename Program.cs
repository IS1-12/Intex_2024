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
                             "script-src 'self' https://kit.fontawesome.com https://apis.google.com 'unsafe-inline'; " + // Include FontAwesome and Google APIs
                             "connect-src 'self' ws://localhost:57798 http://localhost:57798; " + // Allow BrowserLink in development
                             "style-src 'self' 'unsafe-inline'; " + // Assuming you have inline styles
                             "font-src 'self' https://fonts.gstatic.com;"; // If using Google Fonts

                if (ctx.Request.IsHttps || app.Environment.IsDevelopment())
                {
                    ctx.Response.Headers.Add("Content-Security-Policy", csp);
                }
                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.Run();
        }
    }
}
