using HastaneTakipsistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Doctor")]
public class PrescriptionController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PrescriptionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize(Roles = "Doctor")]
    public IActionResult Create(int appointmentId)
    {
        ViewBag.AppointmentId = appointmentId;
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Create(Prescription prescription, int appointmentId)
    {
        if (ModelState.IsValid)
        {
            // Randevuyu al
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound();
            }

            // Doktorun kendi ID'sini al
            prescription.DoctorId = _userManager.GetUserId(User);

            // Hastanın ID'sini randevudan al
            prescription.PatientId = appointment.PatientId;

            // Reçete tarihini ayarla
            prescription.CreatedDate = DateTime.Now;

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return RedirectToAction("DoctorAppointments", "Appointment");
        }

        // Hata durumunda view'a geri dön
        return View(prescription);
    }
}