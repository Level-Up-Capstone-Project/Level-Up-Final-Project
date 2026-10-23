using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapstoneProjectBE.Migrations
{
    /// <inheritdoc />
    public partial class BackfillRoadmapItemResources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO roadmap_item_resources
                    (roadmap_item_resource_id, roadmap_item_id, resource_id, is_completed, completed_at)
                SELECT
                    md5('levelup-resource-backfill:' || item.roadmap_item_id::text || ':' || resource.resource_id::text)::uuid,
                    item.roadmap_item_id,
                    resource.resource_id,
                    FALSE,
                    NULL
                FROM roadmap_items AS item
                JOIN LATERAL (
                    SELECT candidate.resource_id
                    FROM learning_resources AS candidate
                    WHERE candidate.skill_id = item.skill_id
                      AND candidate.is_active = TRUE
                    ORDER BY candidate.resource_name, candidate.resource_id
                    LIMIT 2
                ) AS resource ON TRUE
                ON CONFLICT (roadmap_item_id, resource_id) DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM roadmap_item_resources AS link
                USING roadmap_items AS item, learning_resources AS resource
                WHERE link.roadmap_item_id = item.roadmap_item_id
                  AND link.resource_id = resource.resource_id
                  AND link.roadmap_item_resource_id =
                      md5('levelup-resource-backfill:' || item.roadmap_item_id::text || ':' || resource.resource_id::text)::uuid;
                """);
        }
    }
}
