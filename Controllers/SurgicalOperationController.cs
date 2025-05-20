using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MedicalPark.Dbcontext;
using MedicalPark.Models;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;

namespace MedicalPark.Controllers
{
    [ValidateAntiForgeryToken]

    [Authorize(Roles = "Admin,Doctor")]
    public class SurgicalOperationController : Controller
    {
        private readonly HospitalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SurgicalOperationController> _logger;

        public SurgicalOperationController(
            HospitalDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<SurgicalOperationController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize(Roles = "Patient")]
        [HttpGet]
        public async Task<IActionResult> IndexPatient()
        {
            var currentPatient = await _userManager.GetUserAsync(User) as Patient;
            if (currentPatient == null)
                return Unauthorized("Only patients can access this page.");

            var list = await _context.SurgicalOperation
                .Include(s => s.Doctor)
                .Include(s => s.Patient)
                .Include(s => s.Nurse)
                .Include(s => s.Room)
                .Where(s => s.PatientId == currentPatient.Id && !s.IsDeleted)
                .ToListAsync();

            return View(list);
        }

        [Authorize(Roles = "Admin,Doctor,Nurse")]
        [HttpGet]
        public async Task<IActionResult> IndexAdmin()
        {
            var list = await _context.SurgicalOperation
                .Include(s => s.Doctor)
                .Include(s => s.Patient)
                .Include(s => s.Nurse)
                .Include(s => s.Room)
                .ToListAsync();
            return View(list);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDoctorAndPatientData();
            ViewBag.DefaultStart = DateTime.Now;
            return View(new SurgicalOperation());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(SurgicalOperation model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDoctorAndPatientData();
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User) as Admin;
            if (currentUser == null)
                return Unauthorized("Only Admin can create surgeries.");

            model.CreatedByAdminName = currentUser.Name;

            var patient = await _context.Patients.FindAsync(model.PatientId);
            var doctor = await _context.Doctors.FindAsync(model.DoctorId);
            var nurse = await _context.Nurses.FindAsync(model.NurseId);
            var room = await _context.Rooms.FindAsync(model.RoomId);
            if (patient == null || doctor == null || nurse == null || room == null)
            {
                ModelState.AddModelError("", "Please select valid patient/doctor/nurse/room.");
                await LoadDoctorAndPatientData();
                return View(model);
            }

            model.PatientName = patient.Name;
            model.DoctorName = doctor.Name;
            model.NurseName = nurse.Name;
            model.RoomName = room.RoomName;

            if (model.OperationStartTime.AddMinutes(model.DurationInMinutes) <= DateTime.Now)
                model.IsPatientDischarged = true;


            _context.SurgicalOperation.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexAdmin));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var operation = await _context.SurgicalOperation.FindAsync(id);
            if (operation == null)
                return NotFound();

            await LoadDoctorAndPatientData();
            return View(operation);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SurgicalOperation model)
        {
            if (id != model.Id || !ModelState.IsValid)
            {
                await LoadDoctorAndPatientData();
                return View(model);
            }

            var operation = await _context.SurgicalOperation.FindAsync(id);
            if (operation == null)
                return NotFound();

            operation.Name = model.Name;
            operation.OperationStartTime = model.OperationStartTime;
            operation.DurationInMinutes = model.DurationInMinutes;
            operation.Description = model.Description;
            operation.CostOfOperation = model.CostOfOperation;

            operation.PatientId = model.PatientId;
            operation.DoctorId = model.DoctorId;
            operation.NurseId = model.NurseId;
            operation.RoomId = model.RoomId;

            operation.PatientName = (await _context.Patients.FindAsync(model.PatientId))?.Name;
            operation.DoctorName = (await _context.Doctors.FindAsync(model.DoctorId))?.Name;
            operation.NurseName = (await _context.Nurses.FindAsync(model.NurseId))?.Name;
            operation.RoomName = (await _context.Rooms.FindAsync(model.RoomId))?.RoomName;

            operation.IsPatientDischarged = operation.OperationStartTime
                .AddMinutes(operation.DurationInMinutes) <= DateTime.Now;

            _context.Update(operation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexAdmin));
        }

        [Authorize(Roles = "Admin,Doctor,Nurse")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var operation = await _context.SurgicalOperation
                .Include(s => s.Doctor)
                .Include(s => s.Patient)
                .Include(s => s.Nurse)
                .Include(s => s.Room)
                .Include(s => s.PatientConditions)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
            if (operation == null)
                return NotFound();

            return View(operation);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var operation = await _context.SurgicalOperation
                .Include(s => s.Doctor)
                .Include(s => s.Patient)
                .Include(s => s.Nurse)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
            if (operation == null)
                return NotFound();

            return View(operation);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var operation = await _context.SurgicalOperation.FindAsync(id);
            if (operation == null)
                return NotFound();

            operation.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexAdmin));
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> StartOperation(int id)
        {
            var operation = await _context.SurgicalOperation.FindAsync(id);
            if (operation == null)
                return NotFound();

            await _context.SaveChangesAsync();

            TempData["Success"] = "Operation started successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }



        private async Task LoadDoctorAndPatientData()
        {
            ViewBag.Doctors = new SelectList(await _context.Doctors.Where(d => !d.IsDeleted).ToListAsync(), "Id", "Name");
            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => !p.IsDeleted).ToListAsync(), "Id", "Name");
            ViewBag.Nurses = new SelectList(await _context.Nurses.Where(n => !n.IsDeleted).ToListAsync(), "Id", "Name");
            ViewBag.Rooms = new SelectList(await _context.Rooms.Where(r => !r.IsDeleted && r.IsSterile && r.IsAvailable).ToListAsync(), "Id", "RoomName");
        }
    }
}
