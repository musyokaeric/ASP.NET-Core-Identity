using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebDotNetIndentity.Data.Account;

namespace WebDotNetIndentity.Pages.Account
{
    public class UserProfileModel : PageModel
    {
        private readonly UserManager<User> userManager;

        [BindProperty]
        public UserProfileViewModel UserProfile { get; set; }

        [BindProperty]
        public string? SuccessMessage { get; set; } // the ? next to the data type makes the property as not required 

        public UserProfileModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
            UserProfile = new UserProfileViewModel();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            SuccessMessage = string.Empty;

            var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();

            if (user != null)
            {
                UserProfile.EmailAddress = User.Identity?.Name ?? string.Empty;
                UserProfile.Department = departmentClaim?.Value ?? string.Empty;
                UserProfile.Position = positionClaim?.Value ?? string.Empty;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();

            var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();

            try
            {
                if (user != null && departmentClaim != null)
                    await userManager.ReplaceClaimAsync(user, departmentClaim, new Claim(departmentClaim.Type, UserProfile.Department));
                if (user != null && positionClaim != null)
                    await userManager.ReplaceClaimAsync(user, positionClaim, new Claim(positionClaim.Type, UserProfile.Position));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UserProfile", "Error occured during updating user profile.");
            }

            SuccessMessage = "The user profile is updated successfully.";

            return Page();
        }

        private async Task<(User? user, Claim? departmentClaim, Claim? positionClaim)> GetUserInfoAsync()
        {
            var user = await userManager.FindByNameAsync(User.Identity?.Name ?? "");
            if (user != null)
            {
                var claims = await userManager.GetClaimsAsync(user);
                var departmentClaim = claims.FirstOrDefault(c => c.Type == "Department");
                var positionClaim = claims.FirstOrDefault(c => c.Type == "Position");

                return (user, departmentClaim, positionClaim);
            }
            else return (null, null, null);
        }
    }

    public class UserProfileViewModel
    {
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        [Required]
        public string Position { get; set; } = string.Empty;
    }
}
