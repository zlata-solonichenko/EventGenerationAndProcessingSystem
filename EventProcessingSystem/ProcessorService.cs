using System.Threading.Channels;
using CommonModels;
using Microsoft.EntityFrameworkCore;

namespace EventGenerationAndProcessingSystem;

public class ProcessorService : BackgroundService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ProcessorService> _logger;
    private readonly Channel<SomeEvent> _eventChannel; //создание канала 

    public ProcessorService(ApplicationDbContext dbContext, ILogger<ProcessorService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
        _eventChannel = Channel.CreateUnbounded<SomeEvent>();
    }

    public async Task ProcessEvent(SomeEvent inputEvent)
    {
        //отправка события
        await _eventChannel.Writer.WriteAsync(inputEvent);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            //получение события
            var inputEvent = await _eventChannel.Reader.ReadAsync(stoppingToken);
            await CreateIncident(inputEvent);
        }
    }

    private async Task CreateIncident(SomeEvent inputEvent)
    {
        Incident incident = null;

        // Простой шаблон
        if (inputEvent.Type == (int)EventTypeEnum.Type1)
        {
            incident = new Incident
            {
                Id = Guid.NewGuid(),
                Type = (int)IncidentTypeEnum.Type1,
                Time = DateTime.UtcNow,
                Events = new List<SomeEvent> { inputEvent }
            };
        }
        // Составной шаблон
        else if (inputEvent.Type == (int)EventTypeEnum.Type2)
        {
            var relatedEvent = await _dbContext.Events
                .Where(e => e.Type == (int)EventTypeEnum.Type1 &&
                             e.Time > inputEvent.Time.AddSeconds(-20) &&
                             e.Time <= inputEvent.Time)
                .FirstOrDefaultAsync();

            if (relatedEvent != null)
            {
                incident = new Incident
                {
                    Id = Guid.NewGuid(),
                    Type = (int)IncidentTypeEnum.Type2,
                    Time = DateTime.UtcNow,
                    Events = new List<SomeEvent> { inputEvent, relatedEvent }
                };
            }
        }
        // Если инцидент был создан, сохраняем его в БД
        if (incident != null)
        {
            _dbContext.Incidents.Add(incident);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Инцидент создан: {0}", incident.Id);
        }
    }
}