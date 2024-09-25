using EventGenerationAndProcessingSystem;

namespace CommonModels;

/// <summary>
/// Представляет собой сущность события
/// </summary>
public class SomeEvent
{
    /// <summary>
    /// Id события
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Тип события
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// Время события
    /// </summary>
    public DateTime Time { get; set; }
    
    /// <summary>
    /// Внешний ключ
    /// </summary>
    public Guid IncidentId { get; set; }  
    
    /// <summary>
    /// Навигационное свойство
    /// </summary>
    public Incident Incident { get; set; }
}

/// <summary>
/// Перечисление событий
/// </summary>
public enum EventTypeEnum
{
    Type1 = 1,
    Type2 = 2,
    Type3 = 3,
    Type4 = 4
}