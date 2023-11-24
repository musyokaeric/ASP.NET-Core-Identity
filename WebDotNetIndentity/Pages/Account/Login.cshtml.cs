using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebDotNetIndentity.Data.Account;

namespace WebDotNetIndentity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> signInManager;

        public LoginModel(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
        }

        [BindProperty]
        public CredentialViewModel Credential { get; set; } = new CredentialViewModel();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await signInManager.PasswordSignInAsync(
                Credential.EmailAddress, // string userName
                Credential.Password, // string password
                Credential.RememberMe, // bool isPersistent
                false); // bool lockoutOnFailure

            if (result.Succeeded) return RedirectToPage("/Index");
            else
            {
                if(result.RequiresTwoFactor)
                {
                    return RedirectToPage("/Account/LoginTwoFactor",
                        new
                        {
                            EmailAddress = Credential.EmailAddress,
                            RememberMe = Credential.RememberMe
                        });
                }

                if (result.IsLockedOut)
                    ModelState.AddModelError("Login", "You are locked out.");
                else
                    ModelState.AddModelError("Login", "Failed to login.");
            }

            return Page();
        }
    }

    public class CredentialViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
