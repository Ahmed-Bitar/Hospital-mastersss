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
    public class ManegmentNurseController : Controller
    {
        private readonly EmailVerificationService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly HospitalDbContext _context;
        private static readonly TimeSpan CodeValidityDuration = TimeSpan.FromMinutes(1.5);

        public ManegmentNurseController(
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
        [HttpGet]
        public IActionResult SendVerificationCodeNurse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendVerificationCodeNurse(string email)
        {

            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                return Json(new { success = false, message = "Invalid email address." });
            }

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                _logger.LogWarning($"Attempt to send verification code to existing email: {email}");
                return Json(new { success = false, message = "This email is already associated with an account." });
            }

            var nurseCode = GenerateVerificationCodeEmployee();
            var managerCode = GenerateVerificationCodeManager();
            var codeGeneratedTime = DateTime.UtcNow;

            bool nurseEmailSent = await _emailService.SendVerificationEmail(email, nurseCode);
            bool managerEmailSent = await _emailService.SendVerificationEmail("ahmad.w.bitar@gmail.com", managerCode);

            if (nurseEmailSent && managerEmailSent)
            {
                TempData["NurseVerificationCode"] = nurseCode;
                TempData["ManagerVerificationCodeNurse"] = managerCode;
                TempData["CodeGeneratedTimeNurse"] = codeGeneratedTime;

                return Json(new
                {
                    success = true,
                    message = "Verification codes sent successfully.",
                    redirectUrl = Url.Action("VerifyNurseCodes", "ManegmentNurse")
                });
            }

            return Json(new { success = false, message = "Failed to send verification code." });
        }


        

        [HttpGet]
        public IActionResult VerifyNurseCodes()
        {
            return View();
        }
        [HttpPost]
        public IActionResult VerifyNurseCodes(string nurseCode, string managerCode)
        {
            var savedNurseCode = TempData["NurseVerificationCode"] as string;
            var savedManagerCode = TempData["ManagerVerificationCodeNurse"] as string;
            var codeGeneratedTime = TempData["CodeGeneratedTimeNurse"] as DateTime?;

            if (string.IsNullOrEmpty(savedNurseCode) || string.IsNullOrEmpty(savedManagerCode) || !codeGeneratedTime.HasValue)
            {
                return Json(new { success = false, message = "Verification code has expired or is invalid." });
            }

            var timeElapsed = DateTime.UtcNow - codeGeneratedTime.Value;
            if (timeElapsed > CodeValidityDuration)
            {
                return Json(new { success = false, message = "Verification code has expired." });
            }

            if (nurseCode == savedNurseCode && managerCode == savedManagerCode)
            {
                return Json(new
                {
                    success = true,
                    message = "Verification successful.",
                    redirectUrl = Url.Action("RegisterNurse",
                    "ManegmentNurse")
                });
            }
            else
            {
                return Json(new { success = false, message = "Invalid verification code(s)." });
            }
        }



        [HttpGet]
        public IActionResult RegisterNurse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterNurse(NurseRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = HttpContext.Session.GetString("VerificationEmail");

                if (string.IsNullOrEmpty(email))
                {
                    ModelState.AddModelError(string.Empty, "Session expired. Please start over.");
                    return View(model);
                }

                var user = new Nurse()
                {
                    Name = model.FullName,
                    UserName = model.FullName.Replace(" ", ""),
                    Email = email,
                    PhoneNumber = model.PhoneNumber,
                    UserType = "Nurse",
                    Gender = model.Gender,
                    Salary = model.Salary,



                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    string roleName = "Nurse";

                    var roleExist = await _roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        var role = new ApplicationRole(roleName);
                        await _roleManager.CreateAsync(role);
                    }

                    await _userManager.AddToRoleAsync(user, roleName);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "ManegmentNurse");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }



        private string GenerateVerificationCodeEmployee()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string GenerateVerificationCodeManager()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                email = email.Trim();
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
