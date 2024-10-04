using CommonModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventGenerationAndProcessingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessorController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private static readonly List<Incident> Incidents = new List<Incident>();
    private readonly ILogger<ProcessorController> _logger;
    private readonly ProcessorService _processorService;

    public ProcessorController(ILogger<ProcessorController> logger, ApplicationDbContext dbContext, ProcessorService processorController)    
    {
        _logger = logger;
        _dbContext = dbContext;
        _processorService = processorController;
    }

    /// <summary>
    /// Получение события и создания инцидента
    /// </summary>
    /// <param name="полученное событие"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> PostEvent([FromBody] EventDto inputEvent, CancellationToken stoppingToken)
    {
        if (inputEvent == null)
        {
            return BadRequest("Event data is null");
        }

        var someEvent = new SomeEvent
        {
            Id = inputEvent.Id,
            Type = inputEvent.Type,
            Time = inputEvent.Time,
        };
        
        await _processorService.ProcessEvent(someEvent, stoppingToken);
        return Ok("Событие успешно принято для обработки.");
        
    }
    
    /// <summary>
    /// Метод для получения списка инцидентов с возможностью сортировки и пагинации
    /// </summary>
    /// <returns></returns>
    [HttpGet("incidents")]
    public async Task<IActionResult> GetIncidents([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string sortBy = "Time", [FromQuery] bool ascending = true)
    {
        var incidents = await _dbContext.Incidents
            .Include(i => i.Events) // Включаем связанные события
            .ToListAsync();

        var sortedIncidents = SortAndPaginateIncidents(incidents, page, pageSize, sortBy, ascending);
        return Ok(sortedIncidents);
    }

    /// <summary>
    /// Метод выполняет сортировку и пагинацию списка инцидентов
    /// </summary>
    /// <param name="incidents">список инцидентов</param>
    /// <param name="page">номер страницы, которую нужно вернуть</param>
    /// <param name="pageSize">количество элементов на одной странице</param>
    /// <param name="sortBy">критерий сортировки</param>
    /// <param name="ascending">флаг, указывающий на порядок сортировки</param>
    /// <returns></returns>
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