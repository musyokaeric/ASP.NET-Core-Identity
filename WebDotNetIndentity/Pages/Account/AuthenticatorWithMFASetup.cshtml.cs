using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebDotNetIndentity.Data.Account;

namespace WebDotNetIndentity.Pages.Account
{
    [Authorize] // The user has to login first to set up the MFA.
    public class AuthenticatorWithMFASetupModel : PageModel
    {
        private readonly UserManager<User> userManager;

        [BindProperty]
        public SetupMFAViewModel ViewModel { get; set; }

        [BindProperty]
        public bool Succeeded { get; set; }

        public AuthenticatorWithMFASetupModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
            ViewModel = new SetupMFAViewModel();
            Succeeded = false;
        }

        public async Task OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                await userManager.ResetAuthenticatorKeyAsync(user);
                var key = await userManager.GetAuthenticatorKeyAsync(user);
                ViewModel.Key = key ?? string.Empty;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                // If MFA is successfull
                if (await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, ViewModel.SecurityCode))
                {
                    await userManager.SetTwoFactorEnabledAsync(user, true);
                    Succeeded = true;
                }
                else
                {
                    ModelState.AddModelError("AuthenticatorSetup", "Something went wrong with the authenticator setup.");
                }
            }
            return Page();
        }
    }

    public class SetupMFAViewModel
    {
        public string? Key { get; set; }

        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; } = string.Empty;
    }
}
