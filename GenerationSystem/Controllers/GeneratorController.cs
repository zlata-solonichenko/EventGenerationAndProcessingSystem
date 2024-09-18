using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

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

        if (getEvent.Incident == null)
        {
            return BadRequest("Incident data is null");
        }
        
        try
        {
            // Создание события
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Type = getEvent.Type,
                Time = DateTime.UtcNow,
                IncidentId = getEvent.Incident.Id,
                Incident = new Incident
                {
                Id = getEvent.Incident.Id,
                Type = getEvent.Incident.Type,
                Time = getEvent.Incident.Time,
                Events = new List<Event>()  // Пустой список событий
                } 
            };
            var json = JsonConvert.SerializeObject(newEvent, Formatting.Indented);
            
            // Отправка события по HTTP
            var response = await _httpClient.PostAsJsonAsync("http://localhost:7297/api/Generator/generate", json);
            if (response.IsSuccessStatusCode)
            {
                return Ok(newEvent);  // Возвращаем успешный результат
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Failed to send event");
            }
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
}