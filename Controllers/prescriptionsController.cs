using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using MedicalPark.Models;
using Microsoft.AspNetCore.Authorization;
using MedicalPark.Dbcontext;
using Microsoft.AspNetCore.Identity;

namespace MedicalPark.Controllers
{
    [Authorize(Roles = "Doctor,Patient,Admin")]
    public class PrescriptionsController : Controller
    {
        private readonly HospitalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PrescriptionsController(
                 HospitalDbContext context,
                 UserManager<ApplicationUser> userManager)

        {
            _context = context;
            _userManager = userManager;

        }
        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> DoctorIndex()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !(currentUser is Doctor doctorUser))
            {
                return Unauthorized("Only doctors can access this page.");
            }

            var prescriptionsList = await _context.Prescriptions
                    .Include(p => p.Appointment)
                    .ThenInclude(a => a.Doctor)
                    .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient)
                    .Where(m => m.DoctorID == doctorUser.Id)
                    .ToListAsync();

            return View(prescriptionsList);
        }
        [Authorize(Roles = "Patient")]
        [HttpGet]
        public async Task<IActionResult> PatientIndex()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser is not Patient patientUser)
            {
                Console.WriteLine("aaaaaaaaaaaaaaaaaa");
            }

            var prescriptionsList = await _context.Prescriptions
                    .Include(p => p.Appointment)
                    .ThenInclude(a => a.Doctor)
                    .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient)
                    .Where(m => m.PatientID == currentUser.Id)
                    .ToListAsync();

            return View(prescriptionsList);
        }



        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !(currentUser is Doctor doctorUser))
            {
                return Unauthorized("Only doctors can create prescriptions.");
            }

            var availableAppointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.Prescription == null && a.DoctorId == doctorUser.Id)
                .ToListAsync();

            ViewBag.Appointments = availableAppointments;

            return View(new Prescription());
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> Create(PrescriptionDto prescriptionDto, int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .SingleOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            var prescription = new Prescription
            {
                AppointmentId = appointmentId,
                PatientID = appointment.Patient.Id,
                DoctorID = appointment.Doctor.Id,
                Sickness = appointment.Sickness,
                DoctorName = appointment.Doctor.Name,
                PatientName = appointment.Patient.Name,
                CreatedDate = DateTime.Now,
                MedicalsName = prescriptionDto.MedicalsName ?? "No sickness description provided",
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(DoctorIndex));
        }

        [Authorize(Roles = "Doctor")]

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .ToListAsync();

            ViewBag.Appointments = appointments;

            var prescription = await _context.Prescriptions
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .SingleOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
            {
                return NotFound("Prescription not found.");
            }

            return View(prescription);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, PrescriptionDto prescriptionDto)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .SingleOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
            {
                return NotFound("Prescription not found.");
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .SingleOrDefaultAsync(a => a.Id == prescription.AppointmentId);

            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            prescription.AppointmentId = appointment.Id;
            prescription.PatientID = appointment.Patient.Id;
            prescription.DoctorID = appointment.Doctor.Id;
            prescription.DoctorName = appointment.Doctor.Name;
            prescription.PatientName = appointment.Patient.Name;
            prescription.MedicalsName = prescriptionDto.MedicalsName ?? "No sickness description provided";

            _context.Update(prescription);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(DoctorIndex));
        }

        [HttpGet]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<IActionResult> Details(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .Include(p => p.Appointment)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
            {
                return NotFound("Prescription not found.");
            }

            return View(prescription);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var prescription = await _context.Prescriptions
                .FirstOrDefaultAsync(a => a.Id == id);

            return View(prescription);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Presepriction = await _context.Prescriptions.FindAsync(id);

            _context.Prescriptions.Remove(Presepriction);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(DoctorIndex));
        }
    }
}
