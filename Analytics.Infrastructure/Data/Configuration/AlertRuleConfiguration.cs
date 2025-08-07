using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Analytics.Domain.Entities;

namespace Analytics.Infrastructure.Data.Configurations
{
    public class AlertRuleConfiguration : IEntityTypeConfiguration<AlertRule>
    {
        public void Configure(EntityTypeBuilder<AlertRule> builder)
        {
            // Nome da tabela
            builder.ToTable("AlertRules");

            // Chave primária
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            // Name - nome da regra
            builder.Property(a => a.Name)
                .HasMaxLength(255)
                .IsRequired();

            // MetricName - métrica a ser monitorada
            builder.Property(a => a.MetricName)
                .HasMaxLength(255)
                .IsRequired();

            // Condition - condição em JSON
            builder.Property(a => a.Condition)
                .HasColumnType("jsonb")
                .IsRequired();

            // NotificationUrl - webhook para notificações
            builder.Property(a => a.NotificationUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            // IsActive - status da regra
            builder.Property(a => a.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // TenantId - multi-tenancy
            builder.Property(a => a.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // CreatedAt - timestamp de criação
            builder.Property(a => a.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("NOW()");

            // Índices
            builder.HasIndex(a => a.Name)
                .HasDatabaseName("IX_AlertRules_Name");

            builder.HasIndex(a => a.MetricName)
                .HasDatabaseName("IX_AlertRules_MetricName");

            builder.HasIndex(a => a.TenantId)
                .HasDatabaseName("IX_AlertRules_TenantId");

            builder.HasIndex(a => a.IsActive)
                .HasDatabaseName("IX_AlertRules_IsActive");

            // Índices compostos
            builder.HasIndex(a => new { a.TenantId, a.IsActive })
                .HasDatabaseName("IX_AlertRules_TenantId_IsActive");

            builder.HasIndex(a => new { a.TenantId, a.Name })
                .HasDatabaseName("IX_AlertRules_TenantId_Name")
                .IsUnique(); // Nome único por tenant

            builder.HasIndex(a => new { a.MetricName, a.IsActive })
                .HasDatabaseName("IX_AlertRules_MetricName_IsActive");
        }
    }
}