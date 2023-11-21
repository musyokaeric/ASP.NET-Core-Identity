using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        public IActionResult Authenticate([FromBody]Credential credential)
        {
            // Verification is successful
            if (credential.UserName == "admin" && credential.Password == "password")
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

                var expiresAt = DateTime.UtcNow.AddMinutes(10);

                return Ok(new
                {
                    access_token = CreateToken(claims, expiresAt), // Go to https://jwt.io/ to decode the jwt token
                    expires_at = expiresAt,
                });
            }

            // Verification failed
            ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint.");
            return Unauthorized(ModelState);
        }

        // Private method to create the access token
        private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt)
        {
            // Get SecretKey from configuration file (appsettings.json)
            var secretKey = Encoding.ASCII.GetBytes(configuration.GetValue<string>("SecretKey") ?? "");

            // Generate the JSON Web Token (JWT)
            // -- Install the following nuget packages: System.IdentityModel.Tokens.Jwt, Microsoft.AspNetCore.Authentication.JwtBearer
            var jwt = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expireAt,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature));

            // Return a string instead of a byte array object
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }

    public class Credential
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
