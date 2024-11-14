
using Microsoft.AspNetCore.Identity;

namespace HastaneTakipsistemi.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        
        public string? PatientId { get; set; }
        public ApplicationUser? Patient { get; set; }
        
        public string? DoctorId { get; set; }
        public ApplicationUser? Doctor { get; set; }
        
        public DateTime AppointmentDate { get; set; }
        
        public AppointmentStatus Status { get; set; }
        
        public string? Notes { get; set; }
    }

    public enum AppointmentStatus
    {
        Pending,
        Approved,
        Cancelled,
        Completed
    }
}