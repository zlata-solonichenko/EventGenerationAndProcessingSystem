using Microsoft.EntityFrameworkCore;

namespace EventGenerationAndProcessingSystem;

///2. Процессор
/// - имеет REST API (swagger) через которое принимает события и создает на их основе инциденты
/// - имеет БД в которую сохраняет созданные инциденты
/// - инциденты создаются на основе 2-х шаблонов: простого и составного
/// - получая события, проверяет их на соответствие шаблонам
/// - если событие соответствует шаблону - создается инцидент и записывается в БД
/// - имеет в REST API (swagger) метод для получения списка созданных инцидентов (sorting и pagination - опциональны, но будет плюсом)
/// - при этом, в списке сгенерированных инцидентов для каждого инцидента должен выводится список событий на основе которых он был создан
/// - Шаблон
/// 1. Шаблон №1 (простой): если получено событие с Event.Type = 1 то создать инцидент 1 типа
/// 2. Шаблон №2 (составной): если получено событие с Event.Type = 2, а затем в течении 20 секунд (и не позже!) получено событие с Event.Type = 1, то создать инцидент с Incident.Type = 2, иначе создать инцидент Incident.Type = 1 на основе события с с Event.Type = 1
/// 2.1 составной шаблон описывает инцидент в котором участвуют несколько событий, он имеет временную границу
/// 2.2 составной шаблон имеет приоритет, если событие соответствует составному шаблону, оно не участвует в простом шаблоне №1
/// 
/// <summary>
/// Сущность процессора
/// </summary>
public class EventProcessorService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<EventProcessorService> _logger;

    public EventProcessorService(ApplicationDbContext dbContext, ILogger<EventProcessorService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Шаблоны
    /// </summary>
    /// <param name="newEvent"></param>
    public async Task<Incident> CreateIncidentBasedOnEvent(Event newEvent)
    {
        Incident incident = null;

        // Проверка на соответствие шаблону 2 (составной)
        if (newEvent.Type == EventTypeEnum.Type2)
        {
            var relatedEvent = await _dbContext.Events
                .Where(e => e.Type == EventTypeEnum.Type1 && e.Time > newEvent.Time.AddSeconds(-20) &&
                            e.Time <= newEvent.Time)
                .OrderBy(e => e.Time)
                .FirstOrDefaultAsync();

            if (relatedEvent != null)
            {
                return new Incident
                {
                    Id = Guid.NewGuid(),
                    Type = IncidentTypeEnum.Type2,
                    Time = DateTime.UtcNow,
                    Events = new List<Event> { newEvent, relatedEvent }
                };
            }
        }

        // Проверка на соответствие шаблону 1 (простой)
        if (newEvent.Type == EventTypeEnum.Type1)
        {
            return new Incident
            {
                Id = Guid.NewGuid(),
                Type = IncidentTypeEnum.Type1,
                Time = DateTime.UtcNow,
                Events = new List<Event> { newEvent }
            };
        }

        if (incident != null)
        {
            _dbContext.Incidents.Add(incident);
            await _dbContext.SaveChangesAsync();
        }

        return null;
    }
}