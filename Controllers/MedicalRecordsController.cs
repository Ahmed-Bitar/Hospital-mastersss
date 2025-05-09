using MedicalPark.Dbcontext;
using MedicalPark.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MedicalPark.Controllers
{
    [Authorize(Roles = "Admin,Doctor,Nurse,Patient ")]

    public class MedicalRecordsController : Controller
    {
        private readonly HospitalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MedicalRecordsController(
                 HospitalDbContext context,
                 UserManager<ApplicationUser> userManager)

        {
            _context = context;
            _userManager = userManager;

        }

        [Authorize(Roles = "Admin,Doctor,Nurse ")]

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
        [Authorize(Roles = "Patient ")]
        [HttpGet]
        public async Task<IActionResult> PatientIndex()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser is not Patient patientUser)
            {
                Console.WriteLine("aaaaaaaaaaaaaaaaaa");
            }
            var medicalRecords = await _context.MedicalRecords
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .Where(m => m.PatientId == currentUser.Id)
                .ToListAsync();

            var groupedRecords = medicalRecords
                .GroupBy(m => m.PationName)
                .ToList();

            return View(groupedRecords);
        }

        [Authorize(Roles = "Admin")]


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

        [Authorize(Roles = "Admin")]

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
        [Authorize(Roles = "Admin,Doctor,Nurse")]

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,Nurse")]
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

            var prescriptions = await _context.Prescriptions
                .Include(p => p.Doctor)
                .Where(p => p.PatientID == medicalRecord.PatientId)
                .ToListAsync();

            var doctorMedicines = prescriptions
                .GroupBy(p => p.Doctor.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.SelectMany(p =>
                            p.MedicalsName?
                                .Split(new[] { ',', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(m => m.Trim()) ?? new List<string>())
                        .Distinct()
                        .ToList()
                );

            ViewBag.DoctorMedicines = doctorMedicines;

            return View(medicalRecord);
        }

        [Authorize(Roles = "Admin,Doctor,Nurse")]
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

        [Authorize(Roles = "Admin,Doctor")]

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

       

    }
}