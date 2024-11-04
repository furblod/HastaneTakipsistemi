using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HastaneTakipsistemi.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<TestRequest> TestRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Appointment için ilişkileri tanımlayın
            builder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TestRequest>()
            .HasOne(tr => tr.Doctor)
            .WithMany()
            .HasForeignKey(tr => tr.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TestRequest>()
                .HasOne(tr => tr.Patient)
                .WithMany()
                .HasForeignKey(tr => tr.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TestRequest>()
                .HasOne(tr => tr.Appointment)
                .WithMany()
                .HasForeignKey(tr => tr.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}

