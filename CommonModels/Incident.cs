using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EventGenerationAndProcessingSystem;

/// <summary>
/// Представляет собой сущность инцидента
/// </summary>
public class Incident
{
    [Key]
    /// <summary>
    /// Id инцидента
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Тип инцидента
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// Время инцидента
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    /// Лист событий
    /// </summary>
    public List<Event> Events { get; set; } = new List<Event>();
}

/// <summary>
/// Перечисление событий
/// </summary>
public enum IncidentTypeEnum
{
    Type1 = 1,
    Type2 = 2
}