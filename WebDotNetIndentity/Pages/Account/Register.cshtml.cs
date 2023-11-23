using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace WebDotNetIndentity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;

        public RegisterModel(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; } = new RegisterViewModel();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();

            // Create user
            var user = new IdentityUser
            {
                Email = RegisterViewModel.EmailAddress,
                UserName = RegisterViewModel.EmailAddress
            };

            var result = await userManager.CreateAsync(user, RegisterViewModel.Password);

            if (result.Succeeded)
            {
                // Generate the confirmation token
                var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail",
                    values: new { userId = user.Id, token = confirmationToken });

                // Send Email
                var message = new MailMessage("musyokaer@gmail.com", user.Email,
                    "Please confirm your email",
                    $"Please click this link to confirm your email address: {confirmationLink}");

                using (var emailClient = new SmtpClient("smtp-relay.brevo.com", 587))
                {
                    emailClient.Credentials = new NetworkCredential("musyokaer@gmail.com", "EDRbS90UOB3fsHt8"); // brevo.com SMTP & API Settings
                    await emailClient.SendMailAsync(message);
                }

                return RedirectToPage("/Account/Login");
            }
            else
                foreach (var error in result.Errors)
                    ModelState.AddModelError("Register", error.Description);

            return Page();
        }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
