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
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound();
            }
            prescription.DoctorId = _userManager.GetUserId(User);

            prescription.PatientId = appointment.PatientId;

            prescription.CreatedDate = DateTime.Now;

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return RedirectToAction("DoctorAppointments", "Appointment");
        }
        return View(prescription);
    }
}