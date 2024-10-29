using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HastaneTakipsistemi.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaneTakipsistemi.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Hasta için randevu oluşturma sayfası
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Create()
        {
            var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            ViewBag.Doctors = new SelectList(doctors, "Id", "UserName");
            return View();
        }

        // Randevu oluşturma işlemi
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (appointment == null)
            {
                ModelState.AddModelError("", "Randevu bilgileri geçersiz.");
                return View(appointment);
            }

            if (ModelState.IsValid)
            {
                appointment.PatientId = _userManager.GetUserId(User);
                appointment.Status = AppointmentStatus.Pending;

                try
                {
                    _context.Appointments.Add(appointment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(MyAppointments));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Hata: {ex.Message}");
                    ModelState.AddModelError("", "Randevu oluşturulurken bir hata oluştu.");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            ViewBag.Doctors = new SelectList(doctors, "Id", "User Name");
            return View(appointment);
        }

        // Hasta randevularını görüntüleme
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyAppointments()
        {
            var userId = _userManager.GetUserId(User);
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == userId)
                .ToListAsync();
            return View(appointments);
        }

        // Doktor randevularını görüntüleme
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DoctorAppointments()
        {
            var userId = _userManager.GetUserId(User);
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == userId)
                .ToListAsync();
            return View(appointments);
        }

        // Doktor randevu durumu güncelleme
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateStatus(int id, AppointmentStatus status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            if (appointment.DoctorId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            appointment.Status = status;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DoctorAppointments));
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> PatientDetails(string patientId)
        {
            var patient = await _userManager.FindByIdAsync(patientId);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateMedicalHistory(string patientId, string medicalHistory)
        {
            var patient = await _userManager.FindByIdAsync(patientId);
            if (patient == null)
            {
                return NotFound();
            }
            patient.MedicalHistory = medicalHistory;
            await _userManager.UpdateAsync(patient);
            return RedirectToAction(nameof(PatientDetails), new { patientId = patientId });
        }
    }
}