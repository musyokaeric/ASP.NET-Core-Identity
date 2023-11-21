using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace UnderTheHood.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; } = new Credential();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Verification is successful
            if (Credential.UserName == "admin" && Credential.Password == "password")
            {
                // Create the security context
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
                    new Claim("Department", "HR"), // access HumanResource page
                    new Claim("Manager", "true"), // access HRManager page
                    new Claim("Admin", "true"), // access Settings page

                    // custom policy based authorization
                    // HR Manager is granted access after the probation period has passed (3 months)
                    new Claim("EmploymentDate", "2023-08-16")
                };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                var authenticationProperties = new AuthenticationProperties
                {
                    // persistent cookie survives the closing of the browser session. however, the
                    // cookie is still subject to the timespan set on program.cs
                    IsPersistent = Credential.RememberMe
                };

                // Serialize the ClaimsPrinciple into a string, encrypt the string, and
                // save it as a cookie in the HttpContext object
                await HttpContext.SignInAsync("MyCookieAuth", principal, authenticationProperties);

                return RedirectToPage("/Index");
            }

            // Verification failed
            return Page();
        }
    }

    public class Credential
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name ="Remember Me")]
        public bool RememberMe { get; set; }
    }
}
