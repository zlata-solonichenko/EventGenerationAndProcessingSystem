namespace CommonModels;

public class EventDto
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
}