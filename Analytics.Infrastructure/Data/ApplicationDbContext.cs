using Analytics.Domain.Entities;
using Analytics.Infrastructure.Data.Configurations;
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
            modelBuilder.ApplyConfiguration(new AggregatedMetricConfiguration());
            modelBuilder.ApplyConfiguration(new AlertRuleConfiguration());
            
            base.OnModelCreating(modelBuilder);
        }
    }
}