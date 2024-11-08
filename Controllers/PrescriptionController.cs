using HastaneTakipsistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Doctor")]
public class PrescriptionController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser > _userManager;

    public PrescriptionController(ApplicationDbContext context, UserManager<ApplicationUser > userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int appointmentId)
    {
        var appointment = await _context.Appointments.FindAsync(appointmentId);
        if (appointment == null || appointment.DoctorId != _userManager.GetUserId(User))
        {
            return NotFound();
        }

        var model = new Prescription
        {
            AppointmentId = appointmentId,
            DoctorId = _userManager.GetUserId(User),
            PatientId = appointment.PatientId,
            CreatedDate = DateTime.Now
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Prescription prescription)
    {
        if (ModelState.IsValid)
        {
            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();
            return RedirectToAction("DoctorAppointments");
        }

        return View(prescription);
    }
}