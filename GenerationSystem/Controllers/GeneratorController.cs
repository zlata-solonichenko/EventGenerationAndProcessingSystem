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
    public async Task<IActionResult> GenerateManualEvent()
    {
        try
        {
            await _eventGeneratorService.SendEventManually();
            return Ok("Event generated and sent to processor.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error generating event: {ex.Message}");
        }
    }
}