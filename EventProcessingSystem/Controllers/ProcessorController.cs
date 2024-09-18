using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventGenerationAndProcessingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessorController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private static readonly List<Incident> Incidents = new List<Incident>();
    private readonly EventProcessorService _eventProcessorService;
    private readonly ILogger<ProcessorController> _logger;

    public ProcessorController(ILogger<ProcessorController> logger, ApplicationDbContext dbContext, EventProcessorService eventProcessorService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _eventProcessorService = eventProcessorService;
    }

    /// <summary>
    /// Получение события и создания инцидента
    /// </summary>
    /// <param name="полученное событие"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ProcessEvent([FromBody] Event receivedEvent)
    {
        if (receivedEvent == null)
        {
            return BadRequest("Событие равно нулю.");
        }
        
        Incident incident = await _eventProcessorService.CreateIncidentBasedOnEvent(receivedEvent);
        
        if (incident != null)
        {
            _dbContext.Incidents.Add(incident);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Создано событие: {incident.Id}, Тип: {incident.Type}");
            return Ok("Событие обработано и инцидент создан.");
        }

        return BadRequest("Событие не соответствует никакому шаблону.");
    }
    
    // Метод для получения списка инцидентов с возможностью сортировки и пагинации
    [HttpGet("incidents")]
    public async Task<IActionResult> GetIncidents([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string sortBy = "Time", [FromQuery] bool ascending = true)
    {
        var incidents = await _dbContext.Incidents
            .Include(i => i.Events) // Включаем связанные события
            .ToListAsync();

        var sortedIncidents = SortAndPaginateIncidents(incidents, page, pageSize, sortBy, ascending);
        return Ok(sortedIncidents);
    }
    

    private IEnumerable<Incident> SortAndPaginateIncidents(IEnumerable<Incident> incidents, int page, int pageSize, string sortBy, bool ascending)
    {
        var query = incidents.AsQueryable();

        // Сортировка
        switch (sortBy.ToLower())
        {
            case "time":
                query = ascending ? query.OrderBy(i => i.Time) : query.OrderByDescending(i => i.Time);
                break;
            case "type":
                query = ascending ? query.OrderBy(i => i.Type) : query.OrderByDescending(i => i.Type);
                break;
            default:
                query = ascending ? query.OrderBy(i => i.Time) : query.OrderByDescending(i => i.Time);
                break;
        }

        // Пагинация
        var paginatedIncidents = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return paginatedIncidents;
    }
}