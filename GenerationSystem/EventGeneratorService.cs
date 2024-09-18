namespace EventGenerationAndProcessingSystem;

///1. Генератор
/// -  постоянно генерирует события в случайное время в 2-секундном (например) интервале
/// (т.е. в случайный промежуток времени - [время предыдущего события; время предыдущего события + 2 сек] - должно быть сгенерировано новое событие)
/// - имеет REST API (swagger) в котором в любой момент можно сгенерировать событие вручную 
/// - каждый раз, когда генерируется новое событие, он отсылает его через HTTP request Процессору


/// <summary>
/// Сущность генератора
/// </summary>
public class EventGeneratorService : BackgroundService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EventGeneratorService> _logger;
    private readonly Random _random = new Random();

    public EventGeneratorService(HttpClient httpClient, ILogger<EventGeneratorService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Асинхронное выполнение
    /// </summary>
    /// <param name="stoppingToken"></param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Генерация случайного времени в пределах 2 секунд
            var delay = _random.Next(0, 2000); 
            await Task.Delay(delay, stoppingToken);

            // Генерация события
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Type = (EventTypeEnum)_random.Next(0, Enum.GetValues(typeof(EventTypeEnum)).Length),
                Time = DateTime.UtcNow
            };
            
            // Генерация нового события
            var generatedEvent = GenerateEvent();

            await SendEventToProcessor(generatedEvent);
            
            _logger.LogInformation("Было сгенерировано новое событие");

        }
    }

    /// <summary>
    /// Генерация события
    /// </summary>
    /// <returns></returns>
    private Event GenerateEvent()
    {
        return new Event
        {
            Id = Guid.NewGuid(),
            Type = (EventTypeEnum)_random.Next(0, Enum.GetValues(typeof(EventTypeEnum)).Length),
            Time = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Отправка события процессору через HTTP-запрос
    /// </summary>
    /// <param name="generatedEvent"></param>
    private async Task SendEventToProcessor(Event generatedEvent)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5001/api/processor", generatedEvent);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Событие успешно отправлено процессору.");
            }
            else
            {
                _logger.LogError("Не удалось отправить событие процессору. Status code: " + response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка отправки события процессору: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Отправка события вручную (я не понимаю куда её ставить)
    /// </summary>
    public async Task SendEventManually()
    {
        var newGeneratedEvent = GenerateEvent();
    }
    
}