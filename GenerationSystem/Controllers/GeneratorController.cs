using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace EventGenerationAndProcessingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneratorController : ControllerBase
{

    private readonly HttpClient _httpClient;
    public GeneratorController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Генерация события вручную
    /// </summary>
    /// <returns></returns>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateManualEvent([FromBody] Event getEvent)
    {
        if (getEvent == null)
        {
            return BadRequest("Event data is null");
        }

        try
        {
            // Создание события
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Type = getEvent.Type,
                Time = DateTime.UtcNow,
                IncidentId = Guid.NewGuid()
            };

            // Отправка события по HTTP
            var response = await _httpClient.PostAsJsonAsync("http://localhost:7297/api/Generator/generate", newEvent);
            response.EnsureSuccessStatusCode();
            return Ok(newEvent);
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}