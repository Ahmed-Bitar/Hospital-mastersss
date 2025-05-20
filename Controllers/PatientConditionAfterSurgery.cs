using MedicalPark.Dbcontext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static PatientConditionAfterSurgery;
namespace MedicalPark.Controllers
{
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public class PatientConditionAfterSurgeryController : Controller
    {
        private readonly HospitalDbContext _context;

        public PatientConditionAfterSurgeryController(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var operation = await _context.SurgicalOperation
                .Include(o => o.PatientConditions)
                .Include(o => o.Patient)
                .Include(o => o.Doctor)
                .Include(o => o.Nurse)
                .Include(o => o.Room)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (operation == null) return NotFound();

            return View(operation);
        }

        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> AddPatientCondition(int? operationId)
        {
            if (operationId == null) return NotFound();

            var operation = await _context.SurgicalOperation.FindAsync(operationId);
            if (operation == null) return NotFound();

            var operationEndTime = operation.OperationStartTime.AddMinutes(operation.DurationInMinutes);
            if (DateTime.Now < operationEndTime)
            {
                TempData["Error"] = "Patient condition cannot be recorded before operation ends.";
                return RedirectToAction(nameof(Details), new { id = operationId });
            }

            var model = new PatientConditionAfterSurgery
            {
                PatientId = operation.PatientId,
                Date = DateTime.Now
            };

            ViewBag.OperationId = operationId;
            ViewBag.StatusList = Enum.GetValues(typeof(PatientStatus))
                .Cast<PatientStatus>()
                .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> AddPatientCondition(int operationId, PatientConditionAfterSurgery model)
        {
            var operation = await _context.SurgicalOperation.FindAsync(operationId);
            if (operation == null) return NotFound();

            var operationEndTime = operation.OperationStartTime.AddMinutes(operation.DurationInMinutes);
            if (DateTime.Now < operationEndTime)
            {
                TempData["Error"] = "Patient condition cannot be recorded before operation ends.";
                return RedirectToAction(nameof(Details), new { id = operationId });
            }

            if (!ModelState.IsValid)
            {
                ViewBag.OperationId = operationId;
                ViewBag.StatusList = Enum.GetValues(typeof(PatientStatus))
                    .Cast<PatientStatus>()
                    .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() });
                return View(model);
            }

            model.PatientId = operation.PatientId;
            model.Date = DateTime.Now;

            _context.PatientConditionAfterSurgerys.Add(model);  
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = operationId });
        }
        




    }
}

