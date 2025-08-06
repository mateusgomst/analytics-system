using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using Analytics.Domain.Entities;

namespace Analytics.Infrastructure.Data
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.EventType)
                   .IsRequired()
                   .HasMaxLength(50);
                   
            builder.Property(e => e.UserId)
                   .IsRequired()
                   .HasMaxLength(100);
                   
            builder.Property(e => e.Timestamp)
                   .IsRequired();
                   
            builder.Property(e => e.Payload)
                   .HasConversion(
                       v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                       v => string.IsNullOrEmpty(v) 
                           ? new Dictionary<string, object>() 
                           : JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null)
                             ?? new Dictionary<string, object>()
                   )
                   .HasColumnType("jsonb")
                   .HasColumnName("payload");
                   
            builder.HasIndex(e => e.EventType);
            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => e.Timestamp);
            
            builder.ToTable("Events");
        }
    }
}