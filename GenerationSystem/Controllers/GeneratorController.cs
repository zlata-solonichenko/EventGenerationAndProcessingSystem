using Microsoft.AspNetCore.Mvc;

namespace EventGenerationAndProcessingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneratorController : ControllerBase
{
    private readonly EventGeneratorService _eventGeneratorService;

     public GeneratorController(EventGeneratorService eventService)
     {
         _eventGeneratorService = eventService;
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
                Time = DateTime.UtcNow
            };

            return Ok(newEvent);
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}