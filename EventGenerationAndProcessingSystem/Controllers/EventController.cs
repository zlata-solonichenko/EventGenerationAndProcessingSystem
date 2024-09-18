using Microsoft.AspNetCore.Mvc;

namespace EventGenerationAndProcessingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly EventGeneratorService _eventService;

     public EventController(EventGeneratorService eventService)
     {
         _eventService = eventService;
     }

    [HttpPost]
    public IActionResult GenerateManualEvent()
    {
        //_eventService.GenerateManualEvent();
        return Ok("Событие сгенерировано вручную.");
    }
}