namespace HastaneTakipsistemi.Models
{
    public class TestRequest
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }
        public string? DoctorId { get; set; }
        public ApplicationUser? Doctor { get; set; }
        public string? PatientId { get; set; }
        public ApplicationUser? Patient { get; set; }
        public DateTime RequestDate { get; set; }
        public TestType TestType { get; set; }
        public TestStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? ResultFile { get; set; } // Tahlil sonuç dosyasının yolu
    }

    public enum TestType
    {
        BloodTest,
        UrineTest,
        XRay,
        MRI,
        Ultrasound,
        Other
    }

    public enum TestStatus
    {
        Requested,
        Completed,
        Cancelled
    }
}