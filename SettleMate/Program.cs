using SettleMate.Models.Email;
using SettleMate.S_services.Email;

var builder = WebApplication.CreateBuilder(args);


// ============================================
// MVC
// ============================================

builder.Services.AddControllersWithViews();


// ============================================
// Database
// ============================================

string connectionString =
    builder.Configuration
        .GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "DefaultConnection is not configured.");


// ============================================
// Email Settings
// ============================================

builder.Services.Configure<EmailSettings>(
    builder.Configuration
        .GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailService, EmailService>();


// ============================================
// Session
// ============================================

int sessionTimeout =
    builder.Configuration
        .GetValue<int>(
            "SessionSettings:IdleTimeoutMinutes");

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout =
        TimeSpan.FromMinutes(sessionTimeout);

    options.Cookie.HttpOnly = true;

    options.Cookie.IsEssential = true;
});


var app = builder.Build();


// ============================================
// Middleware
// ============================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


// ============================================
// Session
// ============================================

app.UseSession();


// ============================================
// Authorization
// ============================================

app.UseAuthorization();


// ============================================
// Routing
// ============================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();