using System.Net.Mail;
using MedicalPark.Dbcontext;
using MedicalPark.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static MedicalPark.Models.Doctor;

namespace MedicalPark.Controllers
{
    [Authorize(Roles = "Hospital Manager,AllRole")]

    public class UserManegmentController : Controller
    {
        static  private string _email { get; set; }
        private readonly EmailVerificationService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly HospitalDbContext _context;
        private static readonly TimeSpan CodeValidityDuration = TimeSpan.FromMinutes(1.5);

        public UserManegmentController(
            HospitalDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            EmailVerificationService mailService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailService = mailService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var doctorsAndNurses = await _userManager.GetUsersInRoleAsync("Doctor");
            var nurses = await _userManager.GetUsersInRoleAsync("Nurse");
            var allDoctorsAndNurses = doctorsAndNurses.Concat(nurses).ToList();
            return View(allDoctorsAndNurses);

        }

        public IActionResult CreateDoctorAndNurs()
        {
            return View();
        }

    }
}
