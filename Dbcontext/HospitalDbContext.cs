using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Cryptography;
using MedicalPark.Models;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(u => !u.IsDeleted);
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
        public async Task SeedManagerRoleAndUser(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            var role = await roleManager.FindByNameAsync("Hospital Manager");
            if (role == null)
            {
                role = new ApplicationRole("Hospital Manager");
                await roleManager.CreateAsync(role);
            }

            var user = await userManager.FindByEmailAsync("ahmad.bitar@gmail.com");
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = "ahmadbitar",
                    Gender="Male",
                    Name = "Ahmad Bitar",
                    Email = "ahmad.w.bitar@gmail.com",
                    PhoneNumber = "123456789",
                    UserType = "Hospital Manager",
                };
                string Password = "Ahmad@ab12";
                var result = await userManager.CreateAsync(user, Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Hospital Manager");
                }
            }
        }
      
        public async Task DoctorRole(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            var role = await roleManager.FindByNameAsync("Doctor");
            if (role == null)
            {
                role = new ApplicationRole("Doctor");
                await roleManager.CreateAsync(role);
            }

            var user = await userManager.FindByEmailAsync("Doctor@gmail.comm");
            if (user == null)
            {
                user = new Doctor
                {
                    UserName = "Doctor",
                    Gender = "Male",
                    Name = "Doctor",
                    Email = "Doctor@gmail.com",
                    PhoneNumber = "123456789",
                    UserType = " Doctor",



                };
                string Password = "Ahmad@ab12";
                var result = await userManager.CreateAsync(user, Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Doctor");
                }
            }
        }
      
        public async Task AdminRole(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            var role = await roleManager.FindByNameAsync("Admin");
            if (role == null)
            {
                role = new ApplicationRole("Admin");
                await roleManager.CreateAsync(role);
            }

            var user = await userManager.FindByEmailAsync("bitar@gmail.comm");
            if (user == null)
            {
                user = new Admin
                {
                    UserName = "Admin",
                    Gender = "Male",
                    Name = "Admin",
                    Email = "Admin@gmail.com",
                    PhoneNumber = "123456789",
                    UserType = " Admin",
                };
                string Password = "Ahmad@ab12";
                var result = await userManager.CreateAsync(user, Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }

        public async Task NurseRole(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            var role = await roleManager.FindByNameAsync("Nurse");
            if (role == null)
            {
                role = new ApplicationRole("Nurse");
                await roleManager.CreateAsync(role);
            }

            var user = await userManager.FindByEmailAsync("Nurse@gmail.comm");
            if (user == null)
            {
                user = new Nurse
                {
                    UserName = "Nurse",
                    Gender = "Male",
                    Name = "Nurse",
                    Email = "Nurse@gmail.com",
                    PhoneNumber = "123456789",
                    UserType = " Nurse",
                };
                string Password = "Ahmad@ab12";
                var result = await userManager.CreateAsync(user, Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Nurse");
                }
            }
        }
        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted && e.Entity is Doctor))
            {
                entry.State = EntityState.Modified;
                entry.CurrentValues["IsDeleted"] = true;
            }
            return base.SaveChanges();
        }
    }
}
