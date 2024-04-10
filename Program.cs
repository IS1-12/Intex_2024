using LegosWithAurora.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            // Google authentication
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
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

            // Database context configuration
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration["ConnectionStrings:IntexConnection"]));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDbContext<MfalabContext>(options =>
                options.UseSqlite(builder.Configuration["ConnectionStrings:IntexConnection"]));

            builder.Services.AddScoped<ILegoRepository, EFLegoRepository>();

            builder.Services.AddRazorPages();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();
            builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
            builder.Services.AddSingleton<IHttpContextAccessor,
                HttpContextAccessor>();

            //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            // Configure HSTS - adding HSTS service configuration
            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365); // 1 year
            });

            var app = builder.Build();

            app.UseHttpsRedirection();

           // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {

                app.UseExceptionHandler("/Home/Error");
                // Apply HSTS here
                app.UseHsts();
            }


            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.Use(async (ctx, next) =>
            {
                string csp = "default-src 'self'; " +
                             "script-src 'self' https://kit.fontawesome.com 'unsafe-inline' https://apis.google.com; " +
                             "connect-src 'self' ws://localhost:57798 http://localhost:57798 https://ka-f.fontawesome.com ws://localhost:62719 http://localhost:62719 wss://localhost:44300; " +
                             "style-src 'self' 'unsafe-inline'; " +
                             "font-src 'self' https://fonts.gstatic.com https://ka-f.fontawesome.com; " +
                             "img-src 'self' https://m.media-amazon.com https://www.lego.com/ https://images.brickset.com/ https://www.brickeconomy.com/ data:; ";

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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();


            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[] { "Admin", "Member" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                string email = "testing69@gmail.com";
                string password = "Test1234!";
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new IdentityUser();
                    user.UserName = email;
                    user.Email = email;
                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            app.Run();
        }
    }
}
