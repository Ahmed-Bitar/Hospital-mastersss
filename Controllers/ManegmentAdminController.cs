using System.Net.Mail;
using MedicalPark.Dbcontext;
using MedicalPark.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Umbraco.Core.Models;
using static MedicalPark.Models.Doctor;
namespace MedicalPark.Controllers
{
    [Authorize(Roles = "Hospital Manager")]
    [ValidateAntiForgeryToken]

    public class ManegmentAdminController : Controller
    {
        private readonly EmailVerificationService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly HospitalDbContext _context;
        private static readonly TimeSpan CodeValidityDuration = TimeSpan.FromMinutes(1.5);

        public ManegmentAdminController(
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
        public IActionResult SendVerificationCodeForAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendVerificationCodeForAdmin(string email)
        {
            HttpContext.Session.SetString("AdminEmail", email);

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
            

            var AdmineCode = GenerateVerificationCodeEmployee();
            var managerCode = GenerateVerificationCodeManager();
            var codeGeneratedTime = DateTime.UtcNow;

            bool AdmineEmailSent = await _emailService.SendVerificationEmail(email, AdmineCode);
            bool managerEmailSent = await _emailService.SendVerificationEmail("ahmad.w.bitar@gmail.com", managerCode);

            if (AdmineEmailSent && managerEmailSent)
            {
                TempData["AdmineVerificationCode"] = AdmineCode;
                TempData["ManagerVerificationCodeAdmine"] = managerCode;
                TempData["CodeGeneratedTimeAdmine"] = codeGeneratedTime;

                return Json(new
                {
                    success = true,
                    message = "Verification codes sent successfully.",
                    redirectUrl = Url.Action("VerifyAdminCodes", "ManegmentAdmin")

                });
            }

            return Json(new { success = false, message = "Failed to send verification code." });
        }




        [HttpGet]
        public IActionResult VerifyAdminCodes()
        {
            return View();
        }
        [HttpPost]
        public IActionResult VerifyAdminCodes(string AdmineCode, string managerCode)
        {
            var savedAdmineCode = TempData["AdmineVerificationCode"] as string;
            var savedManagerCode = TempData["ManagerVerificationCodeAdmine"] as string;
            var codeGeneratedTime = TempData["CodeGeneratedTimeAdmine"] as DateTime?;

            if (string.IsNullOrEmpty(savedAdmineCode) || string.IsNullOrEmpty(savedManagerCode) || !codeGeneratedTime.HasValue)
            {
                return RedirectToAction("SendVerificationCodeForAdmin", "ManegmentAdmin");


            }



            if (AdmineCode == savedAdmineCode && managerCode == savedManagerCode)
            {
                return Json(new
                {
                    success = true,
                    message = "Verification successful.",
                    redirectUrl = Url.Action("RegisterAdmin")

                });
            }
            return View();
           
        }
        [HttpGet]
        public async Task<IActionResult> RegisterAdmin()
        {
          
            LoadViewBags();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Hospital Manager")]

        public async Task<IActionResult> RegisterAdmin(AdminRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = new Admin()
                {
                    Name = model.FullName,
                    UserName = model.FullName.Replace(" ", ""),
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserType = model.UserType,
                    Gender = model.Gender,
                    Type = model.Type,
                    Department = model.Department,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {


                    string roleName = "Admin";

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
            HttpContext.Session.SetString("AdminEmail", model.Email);

            ViewBag.email = model.Email;

            return View(model);
        }
        [Authorize(Roles = "Hospital Manager")]

        [HttpGet]
        public async Task<IActionResult> EditAdmin(int id)
        {
            var manegmaent = await _context.Managements.FindAsync(id);
            if (manegmaent == null)
            {
                return NotFound("Admin not found.");
            }

            LoadViewBags();
            return View(manegmaent);
        }
        [Authorize(Roles = "Hospital Manager")]

        [HttpPost]
        public async Task<IActionResult> EditAdmin(int id, AdminRegisterViewModel adminRegisterViewModel)
        {
            if (ModelState.IsValid)
            {
                var manegment = await _context.Managements.FindAsync(id);
                if (manegment == null)
                {
                    return NotFound("Admin not found.");
                }

                manegment.Name = adminRegisterViewModel.FullName;
                manegment.Gender = adminRegisterViewModel.Gender;
                manegment.Type = adminRegisterViewModel.Type;
                manegment.Department = adminRegisterViewModel.Department;
                manegment.Email = adminRegisterViewModel.Email;
                manegment.PhoneNumber = adminRegisterViewModel.PhoneNumber;
                manegment.UserName = adminRegisterViewModel.FullName.Replace(" ", "");
                manegment.UserType= "Admin";

                _context.Update(manegment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "UserManegmentController");

            }

            LoadViewBags();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var manegment = await _context.Managements.FindAsync(id);
            if (manegment == null)
            {
                return NotFound("Management not found.");
            }

            return View(manegment);
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
        private void LoadViewBags()
        {
            var email = HttpContext.Session.GetString("AdminEmail");

            ViewBag.Departments = Enum.GetValues(typeof(Department)).Cast<Department>();
            ViewBag.ManegmentTypes = Enum.GetValues(typeof(ManegmentType)).Cast<ManegmentType>();
            ViewBag.Gender = new List<string> { "Female", "Male" };
            ViewBag.email = email;

        }
    }
    
}
