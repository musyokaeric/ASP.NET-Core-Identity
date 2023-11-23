using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using WebDotNetIndentity.Data.Account;
using WebDotNetIndentity.Services;

namespace WebDotNetIndentity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;

        public RegisterModel(UserManager<User> userManager, IEmailService emailService)
        {
            this.userManager = userManager;
            this.emailService = emailService;
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
            var user = new User
            {
                Email = RegisterViewModel.EmailAddress,
                UserName = RegisterViewModel.EmailAddress,
                Department = RegisterViewModel.Department,
                Position = RegisterViewModel.Position
            };

            var result = await userManager.CreateAsync(user, RegisterViewModel.Password);

            if (result.Succeeded)
            {
                // Generate the confirmation token
                var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

                return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail",
                    values: new { userId = user.Id, token = confirmationToken }) ?? "");

                //////////////////////////////////////////////////////////////
                // To trigger the email confirmation flow, use the code below
                //////////////////////////////////////////////////////////////

                //var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail",
                //    values: new { userId = user.Id, token = confirmationToken });

                // Send Email
                //await emailService.SendAsync("musyokaer@gmail.com", user.Email,
                //    "Please confirm your email",
                //    $"Please click this link to confirm your email address: {confirmationLink}");

                //return RedirectToPage("/Account/Login");
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

        [Required]
        public string Department { get; set; } = string.Empty;

        [Required]
        public string Position { get; set; } = string.Empty;
    }
}
