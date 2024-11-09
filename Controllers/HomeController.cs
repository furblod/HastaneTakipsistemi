using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HastaneTakipsistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HastaneTakipsistemi.Controllers
{
    [AllowAnonymous]
    //[Authorize(Roles = "Admin,Doctor,Patient")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        [AllowAnonymous]
        public IActionResult Welcome()
        {
            // Eğer kullanıcı giriş yapmışsa rolüne göre yönlendir
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Patient"))
                {
                    return RedirectToAction("Profile", "Patient");
                }
                else if (User.IsInRole("Doctor"))
                {
                    return RedirectToAction("DoctorAppointments", "Appointment");
                }
                else if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
            }

            return View();
        }

        public IActionResult Index()
        {
            return RedirectToAction("Welcome");
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