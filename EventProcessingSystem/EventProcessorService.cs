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

    public EventProcessorService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ProcessEventAsync(Event newEvent)
    {
        Incident incident = null;

        // Проверка по шаблону 1
        if (newEvent.Type == EventTypeEnum.Type1)
        {
            incident = new Incident
            {
                Id = Guid.NewGuid(),
                Type = IncidentTypeEnum.Type1,
                Time = DateTime.UtcNow
            };
        }

        // Логика составного шаблона
        else if (newEvent.Type == EventTypeEnum.Type2)
        {
            var previousEvent = await _dbContext.Events
                .Where(e => e.Type == EventTypeEnum.Type1 && e.Time >= DateTime.UtcNow.AddSeconds(-20))
                .OrderByDescending(e => e.Time)
                .FirstOrDefaultAsync();

            if (previousEvent != null)
            {
                incident = new Incident
                {
                    Id = Guid.NewGuid(),
                    Type = IncidentTypeEnum.Type2,
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