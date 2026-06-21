using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NarrowCasting_V5.Data;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;
using NarrowCasting_V5.Services;
using Serilog;

namespace NarrowCasting_V5
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password rules
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                // Sign-in
                options.SignIn.RequireConfirmedAccount = false;

                // Lockout
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

            // Cookie path
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            });

            // Services
            builder.Services.AddScoped<IAuditService, AuditService>();
            builder.Services.AddScoped<IScreenService, ScreenService>();
            builder.Services.AddScoped<IPlaylistService, PlaylistService>();
            builder.Services.AddScoped<IPlaylistItemService, PlaylistItemService>();
            builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IMediaFileService, MediaFileService>();

            // Razor Pages
            builder.Services.AddRazorPages(options =>
            {
                // All /Admin pages require login by default
                options.Conventions.AuthorizeFolder("/Admin");
                // Users management: Admin only
                options.Conventions.AuthorizeFolder("/Admin/Users", "Admin");
                options.Conventions.AuthorizeFolder("/Admin/AuditLog", "Admin");
                // Login page is public
                options.Conventions.AllowAnonymousToPage("/Admin/Index");
            });

            // API controllers
            builder.Services.AddControllers();

            // Role-based policy
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", p => p.RequireRole("Admin"));
                options.AddPolicy("Employee", p => p.RequireRole("Admin", "Employee"));
            });

            var app = builder.Build();

            // Database migration + role seeding
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                await db.Database.MigrateAsync();

                // Ensure roles exist
                foreach (var role in new[] { "Admin", "Employee" })
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                const string adminEmail = "admin@ziekenhuis.nl";
                const string adminPassword = "Admin@123!";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser is null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        FullName = "Systeem Administrator",
                        DepartmentId = null,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                    if (!createResult.Succeeded)
                    {
                        Log.Warning("Could not create default admin account: {Errors}",
                            string.Join("; ", createResult.Errors.Select(e => e.Description)));
                    }
                }

                // Ensure the account is in the Admin role (idempotent)
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    var addRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                    if (!addRoleResult.Succeeded)
                    {
                        Log.Warning("Could not add admin role to account: {Errors}",
                            string.Join("; ", addRoleResult.Errors.Select(e => e.Description)));
                    }
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            //app.UseSerilogRequestLogging();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();
            app.MapControllers();

            await app.RunAsync();
        }
    }
}
