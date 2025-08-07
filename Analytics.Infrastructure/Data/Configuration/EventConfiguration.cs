using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Analytics.Domain.Entities;

namespace Analytics.Infrastructure.Data.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            // Nome da tabela
            builder.ToTable("Events");

            // Chave primária
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            // EventType - obrigatório
            builder.Property(e => e.EventType)
                .HasMaxLength(100)
                .IsRequired();

            // Timestamp - obrigatório
            builder.Property(e => e.Timestamp)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            // UserId - obrigatório
            builder.Property(e => e.UserId)
                .HasMaxLength(255)
                .IsRequired();

            // SessionId - opcional
            builder.Property(e => e.SessionId)
                .HasMaxLength(255)
                .IsRequired(false);

            // Payload - JSON
            builder.Property(e => e.Payload)
                .HasColumnType("jsonb")
                .IsRequired(false);

            // TenantId - obrigatório para multi-tenancy
            builder.Property(e => e.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // CreatedAt - timestamp automático
            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("NOW()");

            // Índices para performance
            builder.HasIndex(e => e.EventType)
                .HasDatabaseName("IX_Events_EventType");

            builder.HasIndex(e => e.Timestamp)
                .HasDatabaseName("IX_Events_Timestamp");

            builder.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_Events_UserId");

            builder.HasIndex(e => e.SessionId)
                .HasDatabaseName("IX_Events_SessionId");

            builder.HasIndex(e => e.TenantId)
                .HasDatabaseName("IX_Events_TenantId");

            // Índices compostos para consultas otimizadas
            builder.HasIndex(e => new { e.TenantId, e.EventType, e.Timestamp })
                .HasDatabaseName("IX_Events_TenantId_EventType_Timestamp");

            builder.HasIndex(e => new { e.TenantId, e.UserId, e.Timestamp })
                .HasDatabaseName("IX_Events_TenantId_UserId_Timestamp");

            builder.HasIndex(e => new { e.TenantId, e.SessionId })
                .HasDatabaseName("IX_Events_TenantId_SessionId");
        }
    }
}