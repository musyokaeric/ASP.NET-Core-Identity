using Microsoft.AspNetCore.Identity;

namespace WebDotNetIndentity.Data.Account
{
    public class User : IdentityUser // Inherits user detail properties (eg. phone number, email, country etc.) 
    {
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }
}
