using Ev_Rent.Areas.Identity;
using EvRent.Data;
using EvRent.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Globalization;
using Microsoft.AspNetCore.Localization;


var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
var culture = new CultureInfo("en-US");

StripeConfiguration.ApiKey =
    builder.Configuration["Stripe:SecretKey"];


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddHttpClient<ChargingStationService>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
})
.AddErrorDescriber<BulgarianIdentityErrorDescriber>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()   
.AddDefaultTokenProviders();

builder.Services.AddScoped<RangeCalculatorService>();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(culture);
    options.SupportedCultures = new[] { culture };
    options.SupportedUICultures = new[] { culture };
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

var app = builder.Build();
app.UseRequestLocalization();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    var adminEmail = "iordanpenkov8@gmail.com"; 
    var admin = await userManager.FindByEmailAsync(adminEmail);

    if (admin != null && !await userManager.IsInRoleAsync(admin, "Admin"))
    {
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();