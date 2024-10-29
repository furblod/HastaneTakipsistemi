using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HastaneTakipsistemi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Veritabanını oluştur
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.EnsureCreated();

            // Rolleri oluştur
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                var role = new IdentityRole("Admin");
                roleManager.CreateAsync(role).Wait();
            }
            if (!roleManager.RoleExistsAsync("Doctor").Result)
            {
                var role = new IdentityRole("Doctor");
                roleManager.CreateAsync(role).Wait();
            }
            if (!roleManager.RoleExistsAsync("Patient").Result)
            {
                var role = new IdentityRole("Patient");
                roleManager.CreateAsync(role).Wait();
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while creating the database.");
        }
    }
}
else
{
    app.UseExceptionHandler("/Home/Error");
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

app.MapControllerRoute(
    name: "patient",
    pattern: "{controller=Patient}/{action=Profile}/{id?}");
app.MapRazorPages();

app.Run();