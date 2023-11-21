using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.Metrics;
using UnderTheHood.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add authentication service
builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";

    // Specify where the login page is
    // options.LoginPath = "/Account/Login";

    // Cookie lifetime and browser session
    options.ExpireTimeSpan = TimeSpan.FromSeconds(300);
});

// To add policy based authorization, we will need to configure the authorization middleware
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
    options.AddPolicy("HRDepartment", policy => policy.RequireClaim("Department", "HR"));
    options.AddPolicy("HRManagerOnly", policy => policy
        .RequireClaim("Department", "HR")
        .RequireClaim("Manager") // claims chaining
        .Requirements.Add(new HRManagerProbationRequirement(3))); // HR Manager is granted access after the probation period has passed(3 months)
});

// Register the requirement handler for the custom policy based authorization
builder.Services.AddSingleton<IAuthorizationHandler, HRManagerProbationRequirementHandler>();

// ===================
// Web API Injections:
// ===================

// Register the HttpClient extension : calls the http client factory which is used to trigger the web API endpoints
// See how IHttpClientFactory is injected and used on the HRManager Page
builder.Services.AddHttpClient("OurWebAPI", options =>
{
    options.BaseAddress = new Uri("https://localhost:7009/");
});

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

// Add authentication middleware
app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
