namespace EventGenerationAndProcessingSystem;

public class EventGeneratorService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EventGeneratorService> _logger;
    private static readonly Random _random = new Random();

    public EventGeneratorService(IHttpClientFactory httpClientFactory, ILogger<EventGeneratorService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Генерация случайного времени
            var delay = _random.Next(500, 2000); // интервал от 0,5 до 2 секунд
            await Task.Delay(delay, stoppingToken);

            // Генерация события
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Type = (Event.EventTypeEnum)_random.Next(1, 5),
                Time = DateTime.UtcNow
            };

            // Логирование события
            _logger.LogInformation($"Сгенерировано событие: {newEvent.Type} в {newEvent.Time}");

            // Отправка события процессору
            await SendEventToProcessor(newEvent);
        }
    }

    private async Task SendEventToProcessor(Event newEvent)
    {
        var client = _httpClientFactory.CreateClient("EventProcessor");

        var response = await client.PostAsJsonAsync("/api/events", newEvent);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Событие успешно отправлено процессору.");
        }
        else
        {
            _logger.LogError("Ошибка при отправке события процессору.");
        }
    }
}