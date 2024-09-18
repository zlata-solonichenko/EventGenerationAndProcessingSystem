using Microsoft.EntityFrameworkCore;

namespace EventGenerationAndProcessingSystem;

public class IncidentDbContext : DbContext
{
    public DbSet<Event> Events { get; set; }
    public DbSet<Incident> Incidents { get; set; }

    public IncidentDbContext(DbContextOptions<IncidentDbContext> options) : base(options)
    {
        
    }
}