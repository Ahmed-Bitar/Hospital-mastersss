using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MedicalPark.Models;
using System.Linq;

namespace MedicalPark.Dbcontext
{
    public class HospitalDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options)
        {
        }

        public DbSet<Report> Reports { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<Admin> Managements { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<SurgicalOperation> SurgicalOperation { get; set; }
        public DbSet<BirthRecord> BirthRecords { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<PatientConditionAfterSurgery> PatientConditionAfterSurgerys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Doctor>().HasQueryFilter(d => !d.IsDeleted);
            modelBuilder.Entity<Patient>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Nurse>().HasQueryFilter(n => !n.IsDeleted);
            modelBuilder.Entity<Room>().HasQueryFilter(u => !u.IsAvailable);

            modelBuilder.Entity<Appointment>().HasQueryFilter(a => !a.Doctor.IsDeleted && !a.Patient.IsDeleted);
            modelBuilder.Entity<MedicalRecord>().HasQueryFilter(m => !m.Patient.IsDeleted);
            modelBuilder.Entity<Prescription>().HasQueryFilter(p => !p.Doctor.IsDeleted && !p.Patient.IsDeleted);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(p => p.DoctorID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany(pa => pa.Prescriptions)
                .HasForeignKey(p => p.PatientID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Patient)
                .WithMany()
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted &&
                           (e.Entity is ApplicationUser ||e.Entity is SurgicalOperation)))
            {
                entry.State = EntityState.Modified;
                entry.CurrentValues["IsDeleted"] = true;
            }
            return base.SaveChanges();
        }
    }
}
