using Microsoft.AspNetCore.Identity;

namespace HastaneTakipsistemi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? MedicalHistory { get; set; }
    }
}