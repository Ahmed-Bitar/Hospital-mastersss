using System.Net.Mail;
using System.Numerics;
using MedicalPark.Dbcontext;
using MedicalPark.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static MedicalPark.Models.Doctor;


namespace MedicalPark.Controllers
{
    [Authorize(Roles = "Hospital Manager")]

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
        public IActionResult SendVerificationCodeNurse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendVerificationCodeNurse(string email)
        {
            HttpContext.Session.SetString("NurseEmail", email);

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



        [Authorize(Roles = "Hospital Manager")]

        [HttpGet]
        public IActionResult VerifyNurseCodes()
        {
            return View();
        }
        [Authorize(Roles = "Hospital Manager")]

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


        [Authorize(Roles = "Hospital Manager")]

        [HttpGet]
        public IActionResult RegisterNurse()
        {
            var email = HttpContext.Session.GetString("NurseEmail");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("SendVerificationCodeNurse");
            }

            ViewBag.email = email;

            return View();
        }

        [Authorize(Roles = "Hospital Manager")]
        [HttpPost]
        public async Task<IActionResult> RegisterNurse(NurseRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Nurse()
                {
                    Name = model.Name,
                    UserName = model.Name.Replace(" ", ""),
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserType = "Nurse",
                    Gender = model.Gender,
                    Salary = model.Salery,
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


                    return RedirectToAction("Index", "UserManegment");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            HttpContext.Session.SetString("NurseEmail", model.Email);
            ViewBag.email = model.Email;

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Hospital Manager")]

        public async Task<IActionResult> Details(int id)
        {
            var nurse = await _context.Nurses.FirstOrDefaultAsync(n => n.Id == id);
            if (nurse == null)
            {
                return NotFound("Nurse not found.");
            }

            return View(nurse);
        }


        [HttpGet]
        [Authorize(Roles = "Hospital Manager")]
        public async Task<IActionResult> EditNurse(int id)
        {
            var nurs = await _context.Nurses.FindAsync(id);
            if (nurs == null)
            {
                return NotFound();
            }

            return View(nurs);
        }


        [HttpPost]
        [Authorize(Roles = "Hospital Manager")]
        public async Task<IActionResult> EditNurse(int id, NurseRegisterViewModel editNurse)
        {
            var nurs = await _context.Nurses.FindAsync(id);
            if (!ModelState.IsValid)
            {
                if (nurs == null)
                {
                    return NotFound();
                }

                nurs.Name = editNurse.Name;
                nurs.Gender = editNurse.Gender;
                nurs.Email = editNurse.Email;
                nurs.PhoneNumber = editNurse.PhoneNumber;
                nurs.Salary = editNurse.Salery;
                nurs.UserType = "Nurse";

                _context.Update(nurs);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "UserManegment");

            }

            return View(nurs);
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
