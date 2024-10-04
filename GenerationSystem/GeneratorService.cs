using CommonModels;
using Newtonsoft.Json;

namespace EventGenerationAndProcessingSystem;

///1. Генератор
/// -  постоянно генерирует события в случайное время в 2-секундном (например) интервале
/// (т.е. в случайный промежуток времени - [время предыдущего события; время предыдущего события + 2 сек] - должно быть сгенерировано новое событие)
/// - имеет REST API (swagger) в котором в любой момент можно сгенерировать событие вручную 
/// - каждый раз, когда генерируется новое событие, он отсылает его через HTTP request Процессору


/// <summary>
/// Сущность генератора
/// </summary>
public class GeneratorService : BackgroundService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeneratorService> _logger;
    private readonly Random _random = new Random();
    private readonly string _processorUrl;

    public GeneratorService(HttpClient httpClient, ILogger<GeneratorService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _processorUrl = "http://localhost:5274/api/Processor";
    }

    /// <summary>
    /// Асинхронное выполнение отправки события (сгенерированного генератором) процессору
    /// </summary>
    /// <param name="stoppingToken"></param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Генерация случайного времени в пределах от 1 до 2 секунд
            var delay = _random.Next(1000, 2000); 
            await Task.Delay(delay, stoppingToken);
            
            // Генерация нового события
            var generatedEvent = GenerateEvent();

            await SendEventToProcessor(generatedEvent);
            
            _logger.LogInformation("Было сгенерировано новое событие");

        }
    }

    /// <summary>
    /// Генерация нового события
    /// </summary>
    /// <returns></returns>
    private SomeEvent GenerateEvent()
    {
        return new SomeEvent
        {
            Id = Guid.NewGuid(),
            Type = _random.Next(0, Enum.GetValues(typeof(EventTypeEnum)).Length),
            Time = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Отправка события процессору через HTTP-запрос
    /// </summary>
    /// <param name="generatedSomeEvent"></param>
    private async Task SendEventToProcessor(SomeEvent generatedSomeEvent)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_processorUrl, generatedSomeEvent);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Отправлено событие с идентификатором {eventId}", generatedSomeEvent.Id);
            
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
    
}