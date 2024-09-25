using CommonModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GenerationSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneratorController : ControllerBase
{
    //private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeneratorController> _logger;

    //IHttpClientFactory httpClientFactory
    public GeneratorController(HttpClient httpClient, ILogger<GeneratorController> logger)
    {
        //_httpClientFactory = httpClientFactory;
        _httpClient = httpClient;
        _logger = logger;
    }
    
    /// <summary>
    /// Генерация события вручную
    /// </summary>
    /// <returns></returns>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateManualEvent([FromBody] EventDto inputEvent)
    {
        if (inputEvent == null)
        {
            return BadRequest("Event data is null");
        }
        
        try
        {
            // Создание события
            var newEvent = new SomeEvent
            {
                Id = Guid.NewGuid(),
                Type = inputEvent.Type,
                Time = DateTime.UtcNow
            };
            //var json = JsonConvert.SerializeObject(newEvent, Formatting.Indented);
            
            // Отправка события по HTTP
            //var httpClient = _httpClientFactory.CreateClient();
            
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5274/api/Processor", newEvent);
            if (response.IsSuccessStatusCode)
            {
                return Ok(newEvent);  // Возвращаем успешный результат
            }
            else
            {
                _logger.LogError($"Failed to send event: {response}");
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