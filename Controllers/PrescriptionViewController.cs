using System.Security.Claims;
using HastaneTakipsistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Patient")]
public class PrescriptionViewController : Controller
{
    private readonly ApplicationDbContext _context;

    public PrescriptionViewController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> MyPrescriptions()
    {
        var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var prescriptions = await _context.Prescriptions
            .Include(p => p.Appointment) // Randevuyu önceden yükle
            .Where(p => p.PatientId == patientId)
            .ToListAsync();

        return View(prescriptions);
    }
}