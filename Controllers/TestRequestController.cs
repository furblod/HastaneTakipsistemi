// Controllers/TestRequestController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HastaneTakipsistemi.Models;
using Microsoft.AspNetCore.Identity;

namespace HastaneTakipsistemi.Controllers
{
    [Authorize]
    public class TestRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public TestRequestController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyTestRequests(int appointmentId)
        {
            var patientId = _userManager.GetUserId(User);
            var testRequests = await _context.TestRequests
                .Include(tr => tr.Doctor)
                .Include(tr => tr.Appointment)
                .Where(tr => tr.PatientId == patientId && tr.AppointmentId == appointmentId)
                .OrderByDescending(tr => tr.RequestDate)
                .ToListAsync();

            return View(testRequests);
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> RequestedTests()
        {
            var doctorId = _userManager.GetUserId(User);
            var testRequests = await _context.TestRequests
                .Include(tr => tr.Patient)
                .Where(tr => tr.DoctorId == doctorId)
                .OrderBy(tr => tr.RequestDate)
                .ToListAsync();

            return View(testRequests);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> UploadResult(int id, IFormFile resultFile)
        {
            var testRequest = await _context.TestRequests.FindAsync(id);
            if (testRequest == null)
            {
                return NotFound();
            }

            if (resultFile != null && resultFile.Length > 0)
            {
                var fileName = Path.GetFileName(resultFile.FileName);
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "test-results", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await resultFile.CopyToAsync(stream);
                }

                testRequest.ResultFile = fileName;
                testRequest.Status = TestStatus.Completed;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(RequestedTests));
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Create([FromForm] TestRequest testRequest)
        {
            if (ModelState.IsValid)
            {
                testRequest.RequestDate = DateTime.Now;
                testRequest.Status = TestStatus.Requested;
                testRequest.DoctorId = _userManager.GetUserId(User);


                var appointment = await _context.Appointments.FindAsync(testRequest.AppointmentId);
                if (appointment != null)
                {
                    testRequest.PatientId = appointment.PatientId;
                }

                testRequest.TestType = (TestType)Enum.Parse(typeof(TestType), testRequest.TestType.ToString());
                testRequest.Notes = testRequest.Notes;

                _context.TestRequests.Add(testRequest);
                await _context.SaveChangesAsync();

                return Ok();
            }

            return BadRequest();
        }


        //private readonly IWebHostEnvironment _webHostEnvironment;

        [Authorize(Roles = "Doctor,Patient")]
        public IActionResult ViewTestResult(string fileName)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "test-results", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/pdf", fileName);
        }
    }
}