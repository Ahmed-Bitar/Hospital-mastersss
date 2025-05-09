using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MedicalPark.Dbcontext;
using MedicalPark.Models;
using Microsoft.AspNetCore.Identity;

namespace MedicalPark.Controllers
{
    [Authorize(Roles = "Admin,Patient,Nurse,Doctor,Hospital Manager")]


    public class ReportsController : Controller
    {
        private readonly HospitalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public ReportsController(
            HospitalDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }


        [Authorize(Roles = " Patient,Nurse,Doctor,Admin")]
        [HttpGet]
        public async Task<IActionResult> UserIndex()
        {

            var currentUser = await _userManager.GetUserAsync(User);
            var reports = await _context.Reports
                .Where(r => r.Id == currentUser.Id)
                .ToListAsync();
            return View(reports);
        }
        [Authorize(Roles = "Hospital Manager")]

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            if (_context == null)
            {
                return NotFound();
            }
            var test = await _context.Reports?.ToListAsync() ?? throw new NotImplementedException();



            var reports = await _context.Reports

                .ToListAsync();

            return View(reports);
        }
        [Authorize(Roles = "Admin,Patient,Nurse,Doctor")]

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.ReportTypes = Enum.GetValues(typeof(Report.ReportType)).Cast<Report.ReportType>().ToList();
            ViewBag.PatientReports = Enum.GetValues(typeof(Report.PatientReportType)).Cast<Report.PatientReportType>().ToList();
            ViewBag.StaffReports = Enum.GetValues(typeof(Report.StaffReportType)).Cast<Report.StaffReportType>().ToList();
            ViewBag.ManagementReports = Enum.GetValues(typeof(Report.ManagementReportType)).Cast<Report.ManagementReportType>().ToList();
            ViewBag.EmergencyReports = Enum.GetValues(typeof(Report.EmergencyReportType)).Cast<Report.EmergencyReportType>().ToList();
            ViewBag.VisitorReports = Enum.GetValues(typeof(Report.VisitorReportType)).Cast<Report.VisitorReportType>().ToList();

            return View(new Report());
        }
        [Authorize(Roles = "Admin,Patient,Nurse,Doctor")]

        [HttpPost]
        public async Task<IActionResult> Create(Report report, ReportDto reportDto)
        {

            switch (report.ReportTypes)
            {
                case Report.ReportType.PatientReportType:
                    ViewBag.PatientReports = Enum.GetValues(typeof(Report.PatientReportType)).Cast<Report.PatientReportType>().ToList();
                    break;
                case Report.ReportType.StaffReportType:
                    ViewBag.StaffReports = Enum.GetValues(typeof(Report.StaffReportType)).Cast<Report.StaffReportType>().ToList();
                    break;
                case Report.ReportType.ManagementReportType:
                    ViewBag.ManagementReports = Enum.GetValues(typeof(Report.ManagementReportType)).Cast<Report.ManagementReportType>().ToList();
                    break;
                case Report.ReportType.EmergencyReportType:
                    ViewBag.EmergencyReports = Enum.GetValues(typeof(Report.EmergencyReportType)).Cast<Report.EmergencyReportType>().ToList();
                    break;
                case Report.ReportType.VisitorReportType:
                    ViewBag.VisitorReports = Enum.GetValues(typeof(Report.VisitorReportType)).Cast<Report.VisitorReportType>().ToList();
                    break;
            }
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser is not Patient patientUser)
            {
                Console.WriteLine("aaaaaaaaaaaaaaaaaa");
            }
            report.Description = reportDto.Description;
            report.UserId = currentUser.Id;
            report.UserName = currentUser.Name;
            report.CreatedDate = DateTime.Now;
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(UserIndex));
        }
        [Authorize(Roles = "Admin,Patient,Nurse,Doctor")]

        public async Task<IActionResult> Details(int id)
        {
            var report = await _context.Reports

                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }
        [Authorize(Roles = "Admin,Patient,Nurse,Doctor")]

        public async Task<IActionResult> Edit(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }
        [Authorize(Roles = "Admin,Patient,Nurse,Doctor")]

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Report report)
        {
            if (id != report.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(report);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(UserIndex));
            }
            return View(report);
        }
        [Authorize(Roles = "Admin,Patient,Nurse,Doctor")]

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports.FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }
        [Authorize(Roles = "Admin,Patient,Nurse,Doctor")]

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.Reports.FindAsync(id);

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(UserIndex));
        }



    }
}