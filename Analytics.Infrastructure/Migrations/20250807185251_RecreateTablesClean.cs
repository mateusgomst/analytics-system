using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Analytics.Infrastructure.Migrations
{
    /// <summary>
    /// Analytics System - Clean Tables Recreation
    /// Date: 2025-08-07 18:52:15 UTC  
    /// User: mateusgomst
    /// Action: Drop and recreate all tables without sample data
    /// </summary>
    public partial class RecreateTablesClean : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. DROP ALL EXISTING TABLES
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    -- Drop tables in correct order to avoid FK constraints
                    DROP TABLE IF EXISTS ""AlertRules"" CASCADE;
                    DROP TABLE IF EXISTS ""AggregatedMetrics"" CASCADE;
                    DROP TABLE IF EXISTS ""Events"" CASCADE;
                    
                    RAISE NOTICE 'All existing tables dropped successfully at %', NOW();
                END $$;
            ");

            // 2. CREATE EVENTS TABLE
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EventType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SessionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Payload = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            // 3. CREATE AGGREGATED METRICS TABLE
            migrationBuilder.CreateTable(
                name: "AggregatedMetrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    MetricName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    Dimensions = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedMetrics", x => x.Id);
                    table.CheckConstraint("CK_AggregatedMetrics_Value", "\"Value\" >= 0");
                });

            // 4. CREATE ALERT RULES TABLE
            migrationBuilder.CreateTable(
                name: "AlertRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MetricName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Condition = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    NotificationUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertRules", x => x.Id);
                });

            // 5. CREATE INDEXES FOR EVENTS
            migrationBuilder.CreateIndex(name: "IX_Events_EventType", table: "Events", column: "EventType");
            migrationBuilder.CreateIndex(name: "IX_Events_SessionId", table: "Events", column: "SessionId");
            migrationBuilder.CreateIndex(name: "IX_Events_TenantId", table: "Events", column: "TenantId");
            migrationBuilder.CreateIndex(name: "IX_Events_Timestamp", table: "Events", column: "Timestamp");
            migrationBuilder.CreateIndex(name: "IX_Events_UserId", table: "Events", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_Events_TenantId_EventType_Timestamp", table: "Events", columns: new[] { "TenantId", "EventType", "Timestamp" });
            migrationBuilder.CreateIndex(name: "IX_Events_TenantId_SessionId", table: "Events", columns: new[] { "TenantId", "SessionId" });
            migrationBuilder.CreateIndex(name: "IX_Events_TenantId_UserId_Timestamp", table: "Events", columns: new[] { "TenantId", "UserId", "Timestamp" });

            // 6. CREATE INDEXES FOR AGGREGATED METRICS
            migrationBuilder.CreateIndex(name: "IX_AggregatedMetrics_Date", table: "AggregatedMetrics", column: "Date");
            migrationBuilder.CreateIndex(name: "IX_AggregatedMetrics_MetricName", table: "AggregatedMetrics", column: "MetricName");
            migrationBuilder.CreateIndex(name: "IX_AggregatedMetrics_TenantId", table: "AggregatedMetrics", column: "TenantId");
            migrationBuilder.CreateIndex(name: "IX_AggregatedMetrics_MetricName_Date", table: "AggregatedMetrics", columns: new[] { "MetricName", "Date" });
            migrationBuilder.CreateIndex(name: "IX_AggregatedMetrics_TenantId_Date", table: "AggregatedMetrics", columns: new[] { "TenantId", "Date" });
            migrationBuilder.CreateIndex(name: "IX_AggregatedMetrics_TenantId_MetricName_Date", table: "AggregatedMetrics", columns: new[] { "TenantId", "MetricName", "Date" }, unique: true);

            // 7. CREATE INDEXES FOR ALERT RULES
            migrationBuilder.CreateIndex(name: "IX_AlertRules_IsActive", table: "AlertRules", column: "IsActive");
            migrationBuilder.CreateIndex(name: "IX_AlertRules_MetricName", table: "AlertRules", column: "MetricName");
            migrationBuilder.CreateIndex(name: "IX_AlertRules_Name", table: "AlertRules", column: "Name");
            migrationBuilder.CreateIndex(name: "IX_AlertRules_TenantId", table: "AlertRules", column: "TenantId");
            migrationBuilder.CreateIndex(name: "IX_AlertRules_MetricName_IsActive", table: "AlertRules", columns: new[] { "MetricName", "IsActive" });
            migrationBuilder.CreateIndex(name: "IX_AlertRules_TenantId_IsActive", table: "AlertRules", columns: new[] { "TenantId", "IsActive" });
            migrationBuilder.CreateIndex(name: "IX_AlertRules_TenantId_Name", table: "AlertRules", columns: new[] { "TenantId", "Name" }, unique: true);

            // 8. SUCCESS MESSAGE
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    RAISE NOTICE 'SUCCESS: Analytics System tables recreated successfully';
                    RAISE NOTICE 'Tables: Events, AggregatedMetrics, AlertRules';
                    RAISE NOTICE 'Ready for data insertion via API';
                    RAISE NOTICE 'Created at: 2025-08-07 18:52:15 UTC by mateusgomst';
                END $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Complete cleanup - drop all tables
            migrationBuilder.DropTable(name: "AlertRules");
            migrationBuilder.DropTable(name: "AggregatedMetrics");
            migrationBuilder.DropTable(name: "Events");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    RAISE NOTICE 'Analytics System tables removed successfully';
                END $$;
            ");
        }
    }
}