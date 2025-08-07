using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Analytics.Domain.Entities;

namespace Analytics.Infrastructure.Data.Configurations
{
    public class AggregatedMetricConfiguration : IEntityTypeConfiguration<AggregatedMetric>
    {
        public void Configure(EntityTypeBuilder<AggregatedMetric> builder)
        {
            // Nome da tabela
            builder.ToTable("AggregatedMetrics");

            // Chave primária
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            // MetricName - obrigatório
            builder.Property(a => a.MetricName)
                .HasMaxLength(255)
                .IsRequired();

            // Date - data da agregação
            builder.Property(a => a.Date)
                .IsRequired()
                .HasColumnType("date");

            // Value - valor da métrica
            builder.Property(a => a.Value)
                .IsRequired()
                .HasColumnType("double precision");

            // Dimensions - dados adicionais em JSON
            builder.Property(a => a.Dimensions)
                .HasColumnType("jsonb")
                .IsRequired(false);

            // TenantId - multi-tenancy
            builder.Property(a => a.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // UpdatedAt - timestamp de atualização
            builder.Property(a => a.UpdatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("NOW()");

            // Índices para consultas rápidas
            builder.HasIndex(a => a.MetricName)
                .HasDatabaseName("IX_AggregatedMetrics_MetricName");

            builder.HasIndex(a => a.Date)
                .HasDatabaseName("IX_AggregatedMetrics_Date");

            builder.HasIndex(a => a.TenantId)
                .HasDatabaseName("IX_AggregatedMetrics_TenantId");

            // Índices compostos essenciais
            builder.HasIndex(a => new { a.TenantId, a.MetricName, a.Date })
                .HasDatabaseName("IX_AggregatedMetrics_TenantId_MetricName_Date")
                .IsUnique(); // Evita duplicatas

            builder.HasIndex(a => new { a.TenantId, a.Date })
                .HasDatabaseName("IX_AggregatedMetrics_TenantId_Date");

            builder.HasIndex(a => new { a.MetricName, a.Date })
                .HasDatabaseName("IX_AggregatedMetrics_MetricName_Date");

            // Constraint para evitar valores negativos em certas métricas
            builder.HasCheckConstraint("CK_AggregatedMetrics_Value", "\"Value\" >= 0");
        }
    }
}