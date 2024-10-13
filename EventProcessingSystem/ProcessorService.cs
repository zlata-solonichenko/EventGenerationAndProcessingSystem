using System.Threading.Channels;
using CommonModels;
using Microsoft.EntityFrameworkCore;

namespace EventGenerationAndProcessingSystem;

public class ProcessorService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ProcessorService> _logger;

    public ProcessorService(ApplicationDbContext dbContext, ILogger<ProcessorService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task ProcessEvent(SomeEvent inputEvent, CancellationToken stoppingToken)
    {
        await ExecuteAsync(inputEvent, stoppingToken);
    }

    protected async Task ExecuteAsync(SomeEvent inputEvent, CancellationToken stoppingToken)
    {
        if (inputEvent.Type == 1)
        {
            //DOTO: не создается инцидент 2 типа, что-то со временем
            //получение события
            var eventType2Before = _dbContext.Events
                .FirstOrDefault(e => e.Type == 2 && !e.IsProcessed && ((DateTime.UtcNow - e.Time).TotalSeconds) <= 60);

            var incident = new Incident
            {
                Id = Guid.NewGuid(),
                Type = eventType2Before == null ? (int)IncidentTypeEnum.Type1 : (int)IncidentTypeEnum.Type2,
                Time = DateTime.UtcNow,
                Events = new List<SomeEvent> { inputEvent }
            };
            //inputEvent.Incident = incident;
            inputEvent.IsProcessed = true;
            
            if (eventType2Before != null)
            {
                eventType2Before.IsProcessed = true;
            }

            _dbContext.Incidents.Add(incident);

            _dbContext.Events.Add(inputEvent);
        }
        else if (inputEvent.Type == 2)
        {
            _dbContext.Events.Add(inputEvent);
        }

        await _dbContext.SaveChangesAsync(stoppingToken);
        _logger.LogInformation("Успешная обработка {inputEventId}", inputEvent.Id);
    }
}