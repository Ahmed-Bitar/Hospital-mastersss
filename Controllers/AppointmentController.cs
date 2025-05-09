using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MedicalPark.Controllers;
using Microsoft.AspNetCore.Identity;
using MedicalPark.Dbcontext;
using MedicalPark.Models;

namespace MedicalPark.Controllers
{
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public class AppointmentController : Controller
    {
        private readonly HospitalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
       

        public AppointmentController(
            HospitalDbContext context,
            UserManager<ApplicationUser> userManager)
          
        {
            _context = context;
            _userManager = userManager;
          
        }
        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> PatientIndex()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser is not Patient patientUser)
            {
                Console.WriteLine("aaaaaaaaaaaaaaaaaa");
            }
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.PatientId == currentUser.Id)
                .ToListAsync();

            return View(appointments);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .ToListAsync();

            return View(appointments);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await LoadDoctorAndPatientData();
            return View(new Appointment());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Appointment appointments, AppointmentDto appointmentDto)
        {
            var doctorName = await _context.Doctors
                .Where(d => d.Id == appointments.DoctorId)
                .Select(d => d.Name)
                .FirstOrDefaultAsync();

            var patientName = await _context.Patients
                .Where(p => p.Id == appointments.PatientId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync();
         
            appointments.DoctorId = appointmentDto.DoctorId;
            appointments.DoctorName = doctorName;
            appointments.PatientId = appointmentDto.PatientId;
            appointments.PatientName = patientName;
            appointments.CreatedDate = DateTime.Now;
            appointments.ClosedDate = DateTime.MinValue;
            appointments.Rendezvous = appointmentDto.Rendezvous;
            appointments.Sickness = appointmentDto.Sickness;

            _context.Appointments.Add(appointments);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CreatePatient()
        {
            await LoadDoctorAndPatientData();
            return View(new Appointment());
        }
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CreatePatient(Appointment appointments, AppointmentDto appointmentDto)
        {
            var doctorName = await _context.Doctors
                .Where(d => d.Id == appointments.DoctorId)
                .Select(d => d.Name)
                .FirstOrDefaultAsync();

          
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser is not Patient patientUser)
            {
                Console.WriteLine("aaaaaaaaaaaaaaaaaa");
            }
            appointments.DoctorId = appointmentDto.DoctorId;
            appointments.DoctorName = doctorName;
            appointments.PatientId = currentUser.Id;
            appointments.PatientName = currentUser.Name;
            appointments.CreatedDate = DateTime.Now;
            appointments.ClosedDate = DateTime.MinValue;
            appointments.Rendezvous = appointmentDto.Rendezvous;
            appointments.Sickness = appointmentDto.Sickness;

            _context.Appointments.Add(appointments);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PatientIndex));
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            await LoadDoctorAndPatientData();
            return View(appointment);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Edit(int id, AppointmentDto appointmentDto)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            appointment.Rendezvous = appointmentDto.Rendezvous;
            appointment.Sickness = appointmentDto.Sickness;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            return View(appointment);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            return View(appointment);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> PatientDelete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PatientIndex));
        }
        private async Task LoadDoctorAndPatientData()
        {
            ViewBag.Doctors = new SelectList(await _context.Doctors.Where(e => e!.IsDeleted == false).ToListAsync(), "Id", "Name");
            ViewBag.Patients = new SelectList(await _context.Patients.Where(e => e!.IsDeleted == false).ToListAsync(), "Id", "Name");
        }
    }
}
