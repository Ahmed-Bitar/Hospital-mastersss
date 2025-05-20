using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MedicalPark.Dbcontext;
using MedicalPark.Models;
using System;
using System.Linq;

namespace MedicalPark.Controllers
{
   //[Authorize(Roles = "Admin,Doctor")]
   /** public class BirthRecordController : Controller
    {
        private readonly HospitalDbContext _context;

        public BirthRecordController(HospitalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var births = await _context.BirthRecords
                .Include(b => b.Mother)
                .Include(b => b.Doctor)
                .ToListAsync();

            return View(births);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadMothersAndDoctors();
            return View(new BirthRecord());
        }

        [HttpPost]
        public async Task<IActionResult> Create(BirthRecord birthRecord, BirthRecordDto birthRecordDto)
        {
            // اجلب اسم الأم والطبيب من قاعدة البيانات حسب المعرفات من DTO
            var motherName = await _context.Patients
                .Where(p => p.Id == birthRecordDto.MotherId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync();

            var doctorName = await _context.Doctors
                .Where(d => d.Id == birthRecordDto.DoctorId)
                .Select(d => d.Name)
                .FirstOrDefaultAsync();

            // إعداد الكائن birthRecord
            birthRecord.MotherId = birthRecordDto.MotherId;
            birthRecord.MotherName = motherName;

            birthRecord.DoctorId = birthRecordDto.DoctorId;
            birthRecord.DoctorName = doctorName;

            birthRecord.BirthDate = birthRecordDto.BirthDate;
            birthRecord.Notes = birthRecordDto.Notes;

            birthRecord.CreatedDate = DateTime.Now; // نضيف تاريخ الإنشاء تلقائيًا

            _context.BirthRecords.Add(birthRecord);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var birthRecord = await _context.BirthRecords.FindAsync(id);
            if (birthRecord == null)
                return NotFound();

            await LoadMothersAndDoctors();
            return View(birthRecord);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, BirthRecordDto birthRecordDto)
        {
            var birthRecord = await _context.BirthRecords.FindAsync(id);
            if (birthRecord == null)
                return NotFound();

            birthRecord.MotherId = birthRecordDto.MotherId;
            birthRecord.DoctorId = birthRecordDto.DoctorId;
            birthRecord.BirthDate = birthRecordDto.BirthDate;
            birthRecord.Notes = birthRecordDto.Notes;

            birthRecord.MotherName = await _context.Patients
                .Where(p => p.Id == birthRecordDto.MotherId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync();

            birthRecord.DoctorName = await _context.Doctors
                .Where(d => d.Id == birthRecordDto.DoctorId)
                .Select(d => d.Name)
                .FirstOrDefaultAsync();

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var birthRecord = await _context.BirthRecords
                .Include(b => b.Mother)
                .Include(b => b.Doctor)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (birthRecord == null)
                return NotFound();

            return View(birthRecord);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var birthRecord = await _context.BirthRecords
                .Include(b => b.Mother)
                .Include(b => b.Doctor)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (birthRecord == null)
                return NotFound();

            return View(birthRecord);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var birthRecord = await _context.BirthRecords.FindAsync(id);
            if (birthRecord == null)
                return NotFound();

            _context.BirthRecords.Remove(birthRecord);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadMothersAndDoctors()
        {
            ViewBag.Mothers = new SelectList(await _context.Patients.Where(p => !p.IsDeleted).ToListAsync(), "Id", "Name");
            ViewBag.Doctors = new SelectList(await _context.Doctors.Where(d => !d.IsDeleted).ToListAsync(), "Id", "Name");
        }*/
    
}
