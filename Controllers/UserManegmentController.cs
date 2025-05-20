using System.Net.Mail;
using MedicalPark.Dbcontext;
using MedicalPark.Models;
using MedicalPark.Servis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static MedicalPark.Models.Doctor;

namespace MedicalPark.Controllers
{
    [ValidateAntiForgeryToken]

    [Authorize(Roles = "Hospital Manager")]
    public class UserManegmentController : Controller
    {
        static private string _email { get; set; }
        private readonly EmailVerificationService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly HospitalDbContext _context;
        private readonly IDeleteService _deleteService;
        private static readonly TimeSpan CodeValidityDuration = TimeSpan.FromMinutes(1.5);

        public UserManegmentController(
            HospitalDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            EmailVerificationService mailService,
            IDeleteService deleteService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailService = mailService;
            _deleteService = deleteService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            var nurses = await _userManager.GetUsersInRoleAsync("Nurse");
            var admins = await _userManager.GetUsersInRoleAsync("admin");

            var allUsers = doctors.Concat(nurses).Concat(admins).ToList();
            return View(allUsers);
        }

        [HttpGet]
        [Authorize(Roles = "Hospital Manager")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Hospital Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _deleteService.DeleteAsync<ApplicationUser>(id);
            if (!result)
                return NotFound($"User with ID {id} not found.");

            return RedirectToAction("Index", "UserManegment");
        }

        public IActionResult CreateDoctorAndNurs()
        {
            return View();
        }
    }
}
