using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebDotNetIndentity.Data.Account;
using WebDotNetIndentity.Services;

namespace WebDotNetIndentity.Pages.Account
{
    public class LoginTwoFactorModel : PageModel
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;

        [BindProperty]
        public EmailMFA EmailMFA { get; set; }

        public LoginTwoFactorModel(SignInManager<User> signInManager, UserManager<User> userManager, IEmailService emailService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.emailService = emailService;
            EmailMFA = new EmailMFA();
        }

        public async Task OnGetAsync(string emailAddress, bool rememberMe)
        {
            var user = await userManager.FindByEmailAsync(emailAddress);

            EmailMFA.SecurityCode = string.Empty;
            EmailMFA.RememberMe = rememberMe;

            // Generate the security code
            var securityCode = await userManager.GenerateTwoFactorTokenAsync(user, "Email");

            //Send the code to the user
            await emailService.SendAsync("musyokaer@gmail.com", emailAddress,
                "My Web App's OTP",
                $"Please use this code as the OTP: {securityCode}");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await signInManager.TwoFactorSignInAsync(
                "Email", // string provider
                EmailMFA.SecurityCode, // security code
                EmailMFA.RememberMe, // persistent
                false); // false means that a 2-factor authentication is always required after every login

            if (result.Succeeded) return RedirectToPage("/Index");
            else
            {
                if (result.IsLockedOut)
                    ModelState.AddModelError("LoginTwoFactor", "You are locked out.");
                else
                    ModelState.AddModelError("LoginTwoFactor", "Failed to login.");
            }

            return Page();
        }
    }

    public class EmailMFA
    {
        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
