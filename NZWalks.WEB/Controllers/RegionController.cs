using Microsoft.AspNetCore.Mvc;
using NZWalks.WEB.Models;
using NZWalks.WEB.Models.DTO;
using System.Text;
using System.Text.Json;

namespace NZWalks.WEB.Controllers
{
    public class RegionController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<RegionController> _logger;
        public RegionController(IHttpClientFactory httpClientFactory, ILogger<RegionController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<RegionDto> response = new List<RegionDto>();
            try
            {
                var client = _httpClientFactory.CreateClient();

                // normally the URL comes from appsettings.json
                var httpResponseMessage = await client.GetAsync("https://localhost:7049/api/region");

                httpResponseMessage.EnsureSuccessStatusCode();

                response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());


            }
            catch (Exception ex)
            {
                // log the exception
            }


            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Add(AddRegionViewModel addRegionViewModel)
        {
            var client = _httpClientFactory.CreateClient();

            var httpResponseMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7049/api/region"),
                Content = new StringContent(JsonSerializer.Serialize(addRegionViewModel), Encoding.UTF8, "application/json")

            };

            var response = await client.SendAsync(httpResponseMessage);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadFromJsonAsync<RegionDto>();

            if (responseString != null)
            {
                return RedirectToAction("Index", "RegionController");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetFromJsonAsync<RegionDto>($"https://localhost:7049/api/region/{id.ToString()}");

            if (response != null)
            {
                return View(response);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RegionDto regionDto)
        {
            var client = _httpClientFactory.CreateClient();
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"https://localhost:7049/api/region/{regionDto.Id}"),
                Content = new StringContent(JsonSerializer.Serialize(regionDto), Encoding.UTF8, "application/json")
            };

            // Vor dem Senden des PUT-Requests
            _logger.LogInformation("Sending PUT request to update region with ID {RegionId}", regionDto.Id);

            var response = await client.SendAsync(httpRequestMessage);

            // Nach Erhalt der Antwort
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Region with ID {RegionId} updated successfully", regionDto.Id);
                var responseString = await response.Content.ReadFromJsonAsync<RegionDto>();
                return RedirectToAction("Edit", "Region");
            }
            else
            {
                _logger.LogWarning("Failed to update region with ID {RegionId}. Status Code: {StatusCode}", regionDto.Id, response.StatusCode);
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Response content: {ResponseContent}", responseContent);

                // Zeige eine benutzerfreundliche Fehlerseite oder gib das regionDto zurück, um Fehler auf der Formularseite anzuzeigen
                return View(regionDto);
            }


        }

        [HttpPost]
        public async Task<IActionResult> Delete(RegionDto request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var httpResponseMessage = await client.DeleteAsync(client.BaseAddress + $"https://localhost:7049/api/region/{request.Id}");

                httpResponseMessage.EnsureSuccessStatusCode();
                
                return RedirectToAction("Index", "Region");
            }
            catch (Exception ex)
            {
                // log the exception
            }

            return View("Edit");

        }

    }
}
