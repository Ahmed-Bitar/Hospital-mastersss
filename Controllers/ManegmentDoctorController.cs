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

    public class ManegmentDoctorController : Controller
    {

        private readonly EmailVerificationService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly HospitalDbContext _context;
        private static readonly TimeSpan CodeValidityDuration = TimeSpan.FromMinutes(1.5);

        public ManegmentDoctorController(
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
        public IActionResult SendVerificationCodeForDoctor()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendVerificationCodeForDoctor(string email)
        {
            HttpContext.Session.SetString("DoctorEmail", email);

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

            var doctorCode = GenerateVerificationCodeEmployee();
            var managerCode = GenerateVerificationCodeManager();
            var codeGeneratedTime = DateTime.UtcNow;

            bool doctorEmailSent = await _emailService.SendVerificationEmail(email, doctorCode);
            bool managerEmailSent = await _emailService.SendVerificationEmail("ahmad.w.bitar@gmail.com", managerCode);

            if (doctorEmailSent && managerEmailSent)
            {
                TempData["DoctorVerificationCode"] = doctorCode;
                TempData["ManagerVerificationCodeDoctor"] = managerCode;
                TempData["CodeGeneratedTimeDoctor"] = codeGeneratedTime;

                return Json(new
                {
                    success = true,
                    message = "Verification codes sent successfully.",
                    redirectUrl = Url.Action("VerifyDoctorCodes", "ManegmentDoctor")

                });

            }

            return Json(new { success = false, message = "Failed to send verification code." });
        }
        [HttpGet]
        public IActionResult VerifyDoctorCodes()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyDoctorCodes(string doctorCode, string managerCode)
        {
            var savedDoctorCode = TempData["DoctorVerificationCode"] as string;
            var savedManagerCode = TempData["ManagerVerificationCodeDoctor"] as string;
            var codeGeneratedTime = TempData["CodeGeneratedTimeDoctor"] as DateTime?;

            if (string.IsNullOrEmpty(savedDoctorCode) || string.IsNullOrEmpty(savedManagerCode) || !codeGeneratedTime.HasValue)
            {
                return Json(new { success = false, message = "Verification code has expired or is invalid." });
            }

            var timeElapsed = DateTime.UtcNow - codeGeneratedTime.Value;
            if (timeElapsed > CodeValidityDuration)
            {
                return Json(new { success = false, message = "Verification code has expired." });
            }

            if (doctorCode == savedDoctorCode && managerCode == savedManagerCode)
            {
                return Json(new
                {
                    success = true,
                    message = "Verification successful.",
                    redirectUrl = Url.Action("RegisterDoctor", "ManegmentDoctor")
                });
            }
            else
            {
                return Json(new { success = false, message = "Invalid verification code(s)." });
            }
        }
        [HttpGet]
        public IActionResult RegisterDoctor()
        {
            ViewBag.Specialty = Enum.GetValues(typeof(DoctorSpecialty))
                                       .Cast<DoctorSpecialty>()
                                       .ToList();

            var email = HttpContext.Session.GetString("DoctorEmail");

            

            ViewBag.email = email;
            
            
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Hospital Manager")]
        public async Task<IActionResult> RegisterDoctor(DoctorRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {


                var user = new Doctor()
                {
                    Name = model.Name,
                    UserName = model.Name.Replace(" ", ""),
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Specialty = model.Specialty,
                    Salery = model.Salery,
                    UserType = model.UserType,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {


                    string roleName = "Doctor";

                    if (!await _roleManager.RoleExistsAsync(roleName))
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

            HttpContext.Session.SetString("DoctorEmail", model.Email);

            ViewBag.email = model.Email;
            return View(model);

        }


        [HttpGet]
        [Authorize(Roles = "Hospital Manager")]

        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id);
            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }

            return View(doctor);
        }


        [HttpGet]
        [Authorize(Roles = "Hospital Manager")]
        public async Task<IActionResult> EditDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        [HttpPost]
        [Authorize(Roles = "Hospital Manager")]
        public async Task<IActionResult> EditDoctor(int id, DoctorRegisterViewModel EditeDoctor)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (!ModelState.IsValid)
            {
                doctor.Name = EditeDoctor.Name;
                doctor.Gender = EditeDoctor.Gender;
                doctor.Specialty = EditeDoctor.Specialty;
                doctor.Email = EditeDoctor.Email;
                doctor.Salery = EditeDoctor.Salery;
                doctor.PhoneNumber = EditeDoctor.PhoneNumber;
                doctor.UserType = "Doctor";

                _context.Update(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "UserManegment");

            }
            return View(EditeDoctor);
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
