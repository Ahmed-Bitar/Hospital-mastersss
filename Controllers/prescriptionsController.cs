using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using MedicalPark.Models;
using Microsoft.AspNetCore.Authorization;
using MedicalPark.Dbcontext;
using MedicalPark.Models;

namespace MedicalPark.Controllers
{
    [Authorize(Roles = "Management,Doctor,AllRole")]

    public class PrescriptionsController : Controller
    {
        private readonly HospitalDbContext _context;

        public PrescriptionsController(HospitalDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Management,Doctor,AllRole")]

        public async Task<IActionResult> Index()
        {
            var prescriptionsList = await _context.Prescriptions
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Doctor)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient)
                .ToListAsync();

            return View(prescriptionsList);
        }
        [Authorize(Roles = "Doctor")]

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var availableAppointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.Prescription == null)
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

            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(int id, PrescriptionDto prescriptionDto, int appointmentId)
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
                .SingleOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            prescription.AppointmentId = appointmentId;
            prescription.PatientID = appointment.Patient.Id;
            prescription.DoctorID = appointment.Doctor.Id;
            prescription.DoctorName = appointment.Doctor.Name;
            prescription.PatientName = appointment.Patient.Name;
            prescription.MedicalsName = prescriptionDto.MedicalsName ?? "No sickness description provided";

            _context.Update(prescription);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Management,Patient,Doctor")]

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .SingleOrDefaultAsync(p => p.Id == id);

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
            var manegmaent = await _context.Managements
                .FirstOrDefaultAsync(a => a.Id == id);


            return View(manegmaent);
        }
        [Authorize(Roles = "Doctor")]

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var manegmaent = await _context.Managements.FindAsync(id);


            _context.Managements.Remove(manegmaent);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
