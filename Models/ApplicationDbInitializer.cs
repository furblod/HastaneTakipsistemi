using Microsoft.AspNetCore.Identity;

public static class ApplicationDbInitializer
{
    public static void Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<IdentityUser>>();

            // Rolleri ekle
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

            // Admin kullanıcı ekle
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
