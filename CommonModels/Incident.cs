using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CommonModels;

namespace EventGenerationAndProcessingSystem;

/// <summary>
/// Представляет собой сущность инцидента
/// </summary>
public class Incident
{
    /// <summary>
    /// Id инцидента
    /// </summary>
    [Key]
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
    public List<SomeEvent> Events { get; set; } = new List<SomeEvent>();
}

/// <summary>
/// Перечисление событий
/// </summary>
public enum IncidentTypeEnum
{
    Type1 = 1,
    Type2 = 2
}