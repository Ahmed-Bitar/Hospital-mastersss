using MedicalPark.Servis;
using AspNetCoreGeneratedDocument;
using MedicalPark.Dbcontext;
using MedicalPark.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MedicalPark.Controllers
{
    public class AccountController : Controller
    {
        private readonly EmailVerificationService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly HospitalDbContext _context;
        private static readonly TimeSpan CodeValidityDuration = TimeSpan.FromMinutes(1.5);

        public AccountController(
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

        public IActionResult IndexLoginSigin()
        {
            return View();
        }
        public IActionResult PatientManegment()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SendVerificationCode()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendVerificationCode(string email)
        {
            TempData["Email"] = email;

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

            var verificationCode = GenerateVerificationCode();
            var codeGeneratedTime = DateTime.UtcNow;

            bool emailSent = await _emailService.SendVerificationEmail(email, verificationCode);

            if (emailSent)
            {
                TempData["VerificationCode"] = verificationCode;
                TempData["CodeGeneratedTime"] = codeGeneratedTime;

                _logger.LogInformation($"Verification code sent to {email}.");
                return Json(new
                {
                    success = true,
                    message = "Verification code sent successfully.",
                    redirectUrl = Url.Action("VerifyCode", "Account")
                });
            }

            _logger.LogError($"Failed to send verification code to {email}.");
            return Json(new { success = false, message = "Failed to send verification code. Please try again." });
        }

        [HttpGet]
        public IActionResult VerifyCode()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCode(string code)
        {
            var savedCode = TempData["VerificationCode"] as string;
            var codeGeneratedTime = TempData["CodeGeneratedTime"] as DateTime?;

            if (string.IsNullOrEmpty(savedCode) || !codeGeneratedTime.HasValue)
            {
                return Json(new { success = false, message = "Verification code has expired or is invalid." });
            }

            if (code == savedCode)
            {
                var timeElapsed = DateTime.UtcNow - codeGeneratedTime.Value;
                if (timeElapsed <= CodeValidityDuration)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Verification has been completed successfully.",
                        redirectUrl = Url.Action("RegisterPationt", "Account")
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Verification code has expired." });
                }
            }
            else
            {
                return Json(new { success = false, message = "Invalid verification code." });
            }
        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        public IActionResult RegisterPationt()
        {
            return View();
        }

         
        [HttpPost]
        public async Task<IActionResult> RegisterPationt(patientRegisterViewModel model)
        {
            if (ModelState.IsValid) { 
                var email = TempData["Email"] as string;
                string roleName = "Patient";

                var user = new Patient()
                {
                    Name = model.Name,
                    UserName = model.Name.Replace(" ", ""),
                    Email = email,
                    PhoneNumber = model.PhoneNumber,
                    Gender=model.Gender,
                    UserType = model.UserType,
                    Adres = model.Adres,
                    Age = model.Age,
                };

                var result = await _userManager.CreateAsync(user, model.Password);


                if (result.Succeeded)
                {

                    var roleExist = await _roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        var role = new ApplicationRole(roleName);
                        await _roleManager.CreateAsync(role);
                    }
                    
                    await _userManager.AddToRoleAsync(user, roleName);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("IndexLoginSigin", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else { Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaaaaaaaa"); }
           

            return View(model);
        }
        [Authorize(Roles = "Patient,AllRole")]
        [HttpGet]
        public async Task<IActionResult> MedicalRecordsByPatient( )
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

            if (medicalRecords == null || !medicalRecords.Any())
            {
                return NotFound($"No medical records noutfound foundD ");
            }

            return View(medicalRecords); 
        }
        [Authorize(Roles = "Doctor,Patient,Management")]
        public async Task<IActionResult> PatientPrescriptions()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser is not Patient patientUser)
            {
                Console.WriteLine("aaaaaaaaaaaaaaaaaa");
            }
            var prescriptions = await _context.Prescriptions
             .Include(p => p.Doctor)
             .Include(p => p.Appointment)
             .Where(p => p.PatientID == currentUser.Id)
             .ToListAsync();

            return View("PrescriptionsList", prescriptions);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = null;

                user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(model.Email);
                }

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("index", "home");
                    }
                    else
                    {
                        _logger.LogWarning("Login failed for user {Email}. Reason: {Reason}", model.Email, result.ToString());
                        ModelState.AddModelError(string.Empty, "Error in login, please check your email address or password.");
                    }
                }
                else
                {
                    _logger.LogWarning("Login failed for user {Email}. User not found.", model.Email);
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            else
            {
                _logger.LogWarning("ModelState is invalid for login attempt of user {Email}.", model.Email);
            }

            return View(model);
        }


        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> ProfileUpdate()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("IndexLoginSigin", "Account");
        }
    }
}
