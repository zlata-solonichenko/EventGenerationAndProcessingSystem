using Microsoft.EntityFrameworkCore;

namespace EventGenerationAndProcessingSystem;

public class EventProcessorService
{
    private readonly IncidentDbContext _dbContext;

    public EventProcessorService(IncidentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ProcessEventAsync(Event newEvent)
    {
        Incident incident = null;

        // Простая проверка по шаблону 1
        if (newEvent.Type == Event.EventTypeEnum.Type1)
        {
            incident = new Incident
            {
                Id = Guid.NewGuid(),
                Type = Incident.IncidentTypeEnum.Type1,
                Time = DateTime.UtcNow
            };
        }

        // Логика составного шаблона
        else if (newEvent.Type == Event.EventTypeEnum.Type2)
        {
            var previousEvent = await _dbContext.Events
                .Where(e => e.Type == Event.EventTypeEnum.Type1 && e.Time >= DateTime.UtcNow.AddSeconds(-20))
                .OrderByDescending(e => e.Time)
                .FirstOrDefaultAsync();

            if (previousEvent != null)
            {
                incident = new Incident
                {
                    Id = Guid.NewGuid(),
                    Type = Incident.IncidentTypeEnum.Type2,
                    Time = DateTime.UtcNow
                };
            }
        }

        if (incident != null)
        {
            _dbContext.Incidents.Add(incident);
            await _dbContext.SaveChangesAsync();
        }
    }
}