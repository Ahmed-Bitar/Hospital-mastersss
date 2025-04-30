using MedicalPark.Dbcontext;
using MedicalPark.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<HospitalDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(90);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<EmailVerificationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
    var context = services.GetRequiredService<HospitalDbContext>();

    await context.SeedManagerRoleAndUser(roleManager, userManager);
    await context.DoctorRole(roleManager, userManager);
    await context.DoctorRole2(roleManager, userManager);
    await context.DoctorRole3(roleManager, userManager);
    await context.patientRole(roleManager, userManager);
    await context.patientRole2(roleManager, userManager);
    await context.patientRole3(roleManager, userManager);
    await context.AdminRole(roleManager, userManager);
    await context.NurseRole(roleManager, userManager);

}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=IndexLoginSigin}/{id?}");

app.Run();
