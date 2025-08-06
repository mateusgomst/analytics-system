using Analytics.Domain.Entities;
using Analytics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<AggregatedMetric> AggregatedMetrics { get; set; }
        public DbSet<AlertRule> AlertRules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EventConfiguration());
        
            
            base.OnModelCreating(modelBuilder);
        }
    }
}