namespace HastaneTakipsistemi.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public string? DoctorId { get; set; }
        public string? PatientId { get; set; }
        public int AppointmentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Medications { get; set; }
        public string? Usage { get; set; }
        public string? Notes { get; set; }

        public virtual ApplicationUser? Doctor { get; set; }
        public virtual ApplicationUser? Patient { get; set; }
        public virtual Appointment? Appointment { get; set; }
    }
}