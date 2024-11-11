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

        //Hasta için randevu oluşturma sayfası
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Create()
        {
            var specializations = new List<string> { "Dahiliye", "Kardiyoloji", "Nöroloji", "Ortopedi", "Pediatri", "Psikiyatri" };
            ViewBag.Specializations = new SelectList(specializations);
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetDoctorsBySpecialization(string specialization)
        {
            var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            var doctorsInSpecialization = doctors
                .Where(d => d.Specialization == specialization)
                .Select(d => new
                {
                    Id = d.Id,
                    FullName = $"Dr. {d.FirstName} {d.LastName}"
                })
                .ToList();

            return Json(doctorsInSpecialization);
        }

        // Randevu oluşturma işlemi
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Create(Appointment appointment, string Specialization)
        {
            if (ModelState.IsValid)
            {
                appointment.PatientId = _userManager.GetUserId(User);
                appointment.Status = AppointmentStatus.Pending;

                var doctor = await _userManager.FindByIdAsync(appointment.DoctorId);
                if (doctor == null || doctor.Specialization != Specialization)
                {
                    ModelState.AddModelError("DoctorId", "Geçersiz doktor seçimi.");
                    return View(appointment);
                }

                // Seçilen tarih ve saatte başka bir randevu olup olmadığını kontrol et
                var existingAppointment = await _context.Appointments
                    .AnyAsync(a => a.DoctorId == appointment.DoctorId &&
                                   a.AppointmentDate == appointment.AppointmentDate);

                if (existingAppointment)
                {
                    ModelState.AddModelError("", "Seçtiğiniz tarih ve saatte doktor zaten bir randevu almıştır.");
                    var specialization = new List<string> { "Dahiliye", "Kardiyoloji", "Nöroloji", "Ortopedi", "Pediatri", "Psikiyatri" };
                    return View(appointment);
                }

                try
                {
                    _context.Appointments.Add(appointment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(MyAppointments));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Randevu oluşturulurken bir hata oluştu.");
                }
            }

            // Hata durumunda formun tekrar yüklenmesi için gerekli verileri hazırlayalım
            var specializations = new List<string> { "Dahiliye", "Kardiyoloji", "Nöroloji", "Ortopedi", "Pediatri", "Psikiyatri" };
            ViewBag.Specializations = new SelectList(specializations);

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

            var viewModel = new PatientDetailsViewModel
            {
                PatientId = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Age = patient.Age,
                Gender = patient.Gender,
                BloodType = patient.BloodType,
                Allergies = patient.Allergies,
                ChronicDiseases = patient.ChronicDiseases,
                MedicalHistory = patient.MedicalHistory
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdatePatientDetails(PatientDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("PatientDetails", model);
            }

            var patient = await _userManager.FindByIdAsync(model.PatientId);
            if (patient == null)
            {
                return NotFound();
            }

            patient.BloodType = model.BloodType;
            patient.Allergies = model.Allergies;
            patient.ChronicDiseases = model.ChronicDiseases;
            patient.MedicalHistory = model.MedicalHistory;

            var result = await _userManager.UpdateAsync(patient);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Hasta bilgileri başarıyla güncellendi.";
            }
            else
            {
                ModelState.AddModelError("", "Güncelleme işlemi başarısız oldu.");
                return View("PatientDetails", model);
            }

            return RedirectToAction(nameof(PatientDetails), new { patientId = model.PatientId });
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> ViewTestRequests(int appointmentId)
        {
            var testRequests = await _context.TestRequests
                .Include(tr => tr.Patient)
                .Where(tr => tr.AppointmentId == appointmentId)
                .ToListAsync();

            ViewBag.AppointmentId = appointmentId;
            return View(testRequests);
        }
    }
}