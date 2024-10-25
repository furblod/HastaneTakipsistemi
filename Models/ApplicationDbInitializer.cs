using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HastaneTakipsistemi.Models
{
    public static class ApplicationDbInitializer
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var roleManager = services.GetService<RoleManager<IdentityRole>>();
                var userManager = services.GetService<UserManager<IdentityUser>>();

                if (roleManager != null && userManager != null)
                {
                    // Rolleri ekleyin
                    if (!roleManager.RoleExistsAsync("Admin").Result)
                    {
                        roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
                    }
                    if (!roleManager.RoleExistsAsync("Doctor").Result)
                    {
                        roleManager.CreateAsync(new IdentityRole("Doctor")).Wait();
                    }
                    if (!roleManager.RoleExistsAsync("Patient").Result)
                    {
                        roleManager.CreateAsync(new IdentityRole("Patient")).Wait();
                    }

                    // Admin kullanıcı ekleyin
                    var adminUser = userManager.FindByEmailAsync("admin@domain.com").Result;
                    if (adminUser == null)
                    {
                        var user = new IdentityUser { UserName = "admin@domain.com", Email = "admin@domain.com" };
                        var result = userManager.CreateAsync(user, "Admin123!").Result;
                        if (result.Succeeded)
                        {
                            userManager.AddToRoleAsync(user, "Admin").Wait();
                        }
                    }
                }
            }
        }
    }
}
