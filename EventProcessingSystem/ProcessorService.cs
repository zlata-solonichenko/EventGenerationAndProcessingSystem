using System.Threading.Channels;
using CommonModels;
using Microsoft.EntityFrameworkCore;

namespace EventGenerationAndProcessingSystem;

public class ProcessorService : BackgroundService
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
        await ExecuteAsync(stoppingToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //список для хранения инцидентов
        var incidents = new List<Incident>();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            //получение события
            var unProcessedEvents = _dbContext.Events
                .Where(e => !e.IsProcessed && ((DateTime.UtcNow - e.Time).TotalSeconds) >= 20)
                .OrderBy(e => e.Time)
                .ToList();

            var type1Events = unProcessedEvents.Where(e=> e.Type == 1).ToList();
            var type2Events = unProcessedEvents.Where(e => e.Type == 2).ToList();
            
            foreach (var processingEvent in unProcessedEvents)
            {
                if (processingEvent.IsProcessed)
                {
                    continue;
                }
                Incident incident = null;
                
                if (processingEvent.Type == 2)
                {
                    var nextEvent = unProcessedEvents.FirstOrDefault(e=> e.Time <= processingEvent.Time.AddSeconds(20));
                    if (nextEvent == null)
                    {
                        //создать инцидент типа 1
                        incident = new Incident
                        {
                            Id = Guid.NewGuid(),
                            Type = (int)IncidentTypeEnum.Type1,
                            Time = DateTime.UtcNow,
                            Events = new List<SomeEvent> { processingEvent }
                        };
                        //ProcessedEvent обработан
                        processingEvent.IsProcessed = true;
                    }
                    else if(nextEvent.Type == 1)
                    {
                        //то создать инцидент типа 2
                        incident = new Incident
                        {
                            Id = Guid.NewGuid(),
                            Type = (int)IncidentTypeEnum.Type2,
                            Time = DateTime.UtcNow,
                            Events = new List<SomeEvent> {processingEvent, nextEvent}
                        };
                        // nextEvent тоже prosecced
                        nextEvent.IsProcessed = true;
                        //ProcessedEvent обработан 
                        processingEvent.IsProcessed = true;
                    }
                    continue;
                }
                else
                {
                    //создать инцидент 1 типа
                    incident = new Incident
                    {
                        Id = Guid.NewGuid(),
                        Type = (int)IncidentTypeEnum.Type1,
                        Time = DateTime.UtcNow,
                        Events = new List<SomeEvent> { processingEvent }
                    };
                    //ProcessedEvent обработан
                    processingEvent.IsProcessed = true;
                }
                
                if (incident != null)
                {
                    incidents.Add(incident); // Добавляем инцидент в список
                }
            }
            
            //все в итоге сохранять в бд
            _dbContext.Incidents.AddRange(incidents);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Создано инцидентов: {0}", incidents.Count);
        }
    }
}