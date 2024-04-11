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

            // Google authentication
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection =
                    builder.Configuration.GetSection("Authentication:Google");
                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                });

            // Add services to the container.
            builder.Services.AddMvc().AddRazorRuntimeCompilation();

            // Database context configuration for ApplicationDbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("IntexConnection")));

            // Optional: Configuration for another DbContext (MfalabContext)
            // Ensure you have valid reasons to maintain separate contexts pointing to the same database.
            builder.Services.AddDbContext<MfalabContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("IntexConnection")));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddScoped<ILegoRepository, EFLegoRepository>();

            // Session and HttpContext configuration
            builder.Services.AddRazorPages();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();
            builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
                // Enable HSTS in production environment as needed
                // app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.Use(async (ctx, next) =>
            {
                var csp = "default-src 'self'; " +
                          "script-src 'self' https://kit.fontawesome.com 'unsafe-inline' https://apis.google.com; " +
                          "connect-src 'self' ws://localhost:57798 http://localhost:57798 https://ka-f.fontawesome.com ws://localhost:62719 http://localhost:62719 wss://localhost:44300; " +
                          "style-src 'self' 'unsafe-inline'; " +
                          "font-src 'self' https://fonts.gstatic.com https://ka-f.fontawesome.com; " +
                          "img-src 'self' https://m.media-amazon.com https://www.lego.com/ https://images.brickset.com/ https://www.brickeconomy.com/ data:; ";

                ctx.Response.Headers.Append("Content-Security-Policy", csp);
                ctx.Response.Headers.Append("X-Content-Type-Options", "nosniff");

                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            // Role and user initialization (Consider moving to a script for production environments)
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[] { "Admin", "Member" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                string email = "testing69@gmail.com";
                string password = "Test1234!";
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new IdentityUser { UserName = email, Email = email };
                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            app.Run();
        }
    }
}
