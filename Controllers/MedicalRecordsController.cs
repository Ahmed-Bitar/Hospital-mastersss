using MedicalPark.Dbcontext;
using MedicalPark.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MedicalPark.Controllers
{
    [Authorize(Roles = "Management,Doctor,Nurse,AllRole")]

    public class MedicalRecordsController : Controller
    {
        private readonly HospitalDbContext _context;

        public MedicalRecordsController(HospitalDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var medicalRecords = await _context.MedicalRecords
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .ToListAsync();

            var groupedRecords = medicalRecords
                .GroupBy(m => m.PationName)
                .ToList();

            return View(groupedRecords);
        }


        [Authorize(Roles = "Management")]


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .AsNoTracking()
                .ToListAsync();
            ViewBag.Prescriptions = prescriptions;
            return View(new MedicalRecord());
        }

        [Authorize(Roles = "Management")]

        [HttpPost]
        public async Task<IActionResult> Create(int[] selectedPrescriptions)
        {
            foreach (var prescriptionId in selectedPrescriptions)
            {
                var prescription = await _context.Prescriptions
                    .Include(p => p.Patient)
                    .Include(p => p.Doctor)
                    .SingleOrDefaultAsync(p => p.Id == prescriptionId);

                if (prescription == null)
                {
                    return NotFound($"Prescription with ID {prescriptionId} not found.");
                }

                var medicalRecord = new MedicalRecord()
                {
                    PatientId = prescription.PatientID,
                    DoctorId = prescription.DoctorID,
                    PationName = prescription.Patient.Name,
                    DoctorName = prescription.Doctor.Name,
                    Medicals = prescription.MedicalsName
                };

                await _context.MedicalRecords.AddAsync(medicalRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Management,Doctor,Nurse")]

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var medicalRecord = await _context.MedicalRecords
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (medicalRecord == null)
            {
                return NotFound($"Medical record with ID {id} not found.");
            }


            var doctors = await _context.MedicalRecords
                .Where(m => m.PatientId == medicalRecord.PatientId)
                .Select(m => m.DoctorName)
                .Distinct()
                .ToListAsync();


            var medicines = _context.Prescriptions
                .Where(p => p.PatientID == medicalRecord.PatientId)
                .AsEnumerable()
                .SelectMany(p => p.MedicalsName.Split(new[] { ',', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .ToList();


            ViewBag.Doctors = doctors;
            ViewBag.Medicines = medicines;

            return View(medicalRecord);
        }
        [Authorize(Roles = "Management,Doctor,Nurse")]
        [HttpGet]
        public async Task<IActionResult> GetMedicalRecordsByPatientId(int patientId)
        {
            var medicalRecords = await _context.MedicalRecords
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .Where(m => m.PatientId == patientId)
                .ToListAsync();

            if (medicalRecords == null || !medicalRecords.Any())
            {
                return NotFound($"No medical records found for patient with ID {patientId}.");
            }

            return View(medicalRecords); 
        }
        public async Task<IActionResult> Edit()
        {
            var prescriptions = await _context.Prescriptions
                           .Include(p => p.Patient)
                           .Include(p => p.Doctor)
                           .AsNoTracking()
                           .ToListAsync();
            ViewBag.Prescriptions = prescriptions;
            return View(new MedicalRecord());
        }

        [Authorize(Roles = "Management,Doctor")]

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MedicalRecord medicalRecord)
        {
            if (id != medicalRecord.Id)
            {
                return BadRequest("Record ID mismatch.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalRecord);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {

                    throw;
                }
            }
            return View(medicalRecord);
        }
        [Authorize(Roles = "Management")]

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var medicalRecord = await _context.MedicalRecords
                                    .Include(m => m.Patient)
                                    .Include(m => m.Doctor)
                                    .SingleOrDefaultAsync(m => m.Id == id);

            if (medicalRecord == null)
            {
                return NotFound($"Medical record with ID {id} not found.");
            }

            return View(medicalRecord);
        }
        [Authorize(Roles = "Management")]

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord == null)
            {
                return NotFound($"Medical record with ID {id} not found.");
            }

            _context.MedicalRecords.Remove(medicalRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        
        
        
        }

    }
}