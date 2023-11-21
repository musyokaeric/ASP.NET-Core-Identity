using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using UnderTheHood.Authorization;
using UnderTheHood.DTO;
using UnderTheHood.Pages.Account;

namespace UnderTheHood.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;

        // Inject the Http client factory because we want to use it to trigger
        // the web API endpoints
        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public List<WeatherForecastDTO> WeatherForecastItems { get; set; } = new List<WeatherForecastDTO>();

        public async Task OnGet()
        {
            // Get token from session
            JwtToken token = new JwtToken();
            var jwt = HttpContext.Session.GetString("access_token");

            if (string.IsNullOrEmpty(jwt))
            {
                // Authenticate the web API
                token = await Authenticate();
                
            }
            else
            {
                token = JsonConvert.DeserializeObject<JwtToken>(jwt) ?? new JwtToken();
            }

            // There's a possibility that the token is still null or has expired
            if (token == null || string.IsNullOrEmpty(token.AccessToken) || token.ExpiresAt <= DateTime.UtcNow)
            {
                // Authenticate the web API
                token = await Authenticate();
            }

            // Consume web API endpoint
            var httpClient = httpClientFactory.CreateClient("OurWebAPI");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty);
            WeatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast") ?? new List<WeatherForecastDTO>();
        }

        private async Task<JwtToken> Authenticate()
        {
            var httpClient = httpClientFactory.CreateClient("OurWebAPI");
            
            var response = await httpClient.PostAsJsonAsync("auth", new Credential { UserName = "admin", Password = "password" });
            response.EnsureSuccessStatusCode();
            string jwt = await response.Content.ReadAsStringAsync();

            // Store the token in the session
            HttpContext.Session.SetString("access_token", jwt);

            return JsonConvert.DeserializeObject<JwtToken>(jwt) ?? new JwtToken();
        }
    }
}
