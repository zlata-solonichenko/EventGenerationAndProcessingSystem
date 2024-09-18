namespace EventGenerationAndProcessingSystem;

/// <summary>
/// Представляет собой сущность инцидента
/// </summary>
public class Incident
{
    /// <summary>
    /// Id инцидента
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Тип инцидента
    /// </summary>
    public IncidentTypeEnum Type { get; set; }
    
    /// <summary>
    /// Время инцидента
    /// </summary>
    public DateTime Time { get; set; }
    
    /// <summary>
    /// Лист событий
    /// </summary>
    public List<Event> Events { get; set; } = new List<Event>();
    
    /// <summary>
    /// Перечисление событий
    /// </summary>
    public enum IncidentTypeEnum
    {
        Type1 = 1,
        Type2 = 2
    }

}