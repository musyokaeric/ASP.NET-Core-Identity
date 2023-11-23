using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebDotNetIndentity.Data;
using WebDotNetIndentity.Data.Account;
using WebDotNetIndentity.Services;
using WebDotNetIndentity.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Injected DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

// Configure Application Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Password Policy
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>() // Specifies how the identity service connects to the database
    .AddDefaultTokenProviders(); // Generates token for email confirmation

// Configure Cookie Behavior
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Configure app settings
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SMTP"));
builder.Services.AddSingleton<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
