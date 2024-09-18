using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventGenerationAndProcessingSystem;

public class ApplicationDbContext : DbContext, IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public ApplicationDbContext(){}
    
    public DbSet<Incident> Incidents { get; set; }
    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Incident>()
            .HasMany(i => i.Events)
            .WithOne(e=>e.Incident)
            .HasForeignKey(e=>e.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Incident)
            .WithMany(i => i.Events)
            .HasForeignKey(e => e.IncidentId);
    }

    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=processorDb;Username=postgres;Password=postgres");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}