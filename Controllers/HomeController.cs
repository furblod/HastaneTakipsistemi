using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HastaneTakipsistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HastaneTakipsistemi.Controllers
{
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        // public IActionResult Index()
        // {
        //     return View();
        // }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null && await _userManager.IsInRoleAsync(user, "Patient"))
            {
                return RedirectToAction("Profile", "Patient");
            }
            else if (user != null && await _userManager.IsInRoleAsync(user, "Doctor"))
            {
                return RedirectToAction("DoctorAppointments", "Appointment");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}