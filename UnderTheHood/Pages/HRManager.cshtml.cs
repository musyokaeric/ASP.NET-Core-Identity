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
        // the web API
        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public List<WeatherForecastDTO> WeatherForecastItems { get; set; } = new List<WeatherForecastDTO>();

        public async Task OnGet()
        {
            var httpClient = httpClientFactory.CreateClient("OurWebAPI");

            // Authenticate the web API
            var response = await httpClient.PostAsJsonAsync("auth", new Credential { UserName = "admin", Password = "password" });
            response.EnsureSuccessStatusCode();

            string jwt = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<JwtToken>(jwt);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty);

            // Consume web API endpoint
            WeatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast") ?? new List<WeatherForecastDTO>();
        }
    }
}
