using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapstoneProjectBE.Migrations
{
    /// <summary>
    /// Adds stable provider identities and canonical URLs for reusable AI-discovered learning resources.
    /// </summary>
    public partial class AddDiscoveredLearningResources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "canonical_url",
                table: "learning_resources",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "provider_key",
                table: "learning_resources",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "provider_resource_id",
                table: "learning_resources",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://learn.microsoft.com/en-us/training/paths/get-started-c-sharp-part-1", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://exercism.org/tracks/csharp", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://www.postgresql.org/docs/current/tutorial-sql.html", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000004"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://sqlbolt.com/", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000005"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Overview", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000006"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000007"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://dev.java/learn", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000008"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://exercism.org/tracks/java", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000009"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://docs.python.org/3/tutorial", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000010"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://exercism.org/tracks/python", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000011"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://www.postgresql.org/docs/current/tutorial.html", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000012"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://pgexercises.com/", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000013"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://react.dev/learn", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000014"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://react.dev/learn/tutorial-tic-tac-toe", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000015"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/Structuring_content", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000016"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/Structuring_content/Test_your_skills/HTML_text_basics", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000017"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/Styling_basics", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000018"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/CSS_layout", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000019"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000020"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://javascript.info/", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000021"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://www.typescriptlang.org/docs/handbook/intro.html", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000022"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://www.typescriptlang.org/play", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000023"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://git-scm.com/book/en/v2", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000024"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://github.com/skills/introduction-to-github", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000025"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://learn.microsoft.com/en-us/dotnet/core/get-started", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000026"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://learn.microsoft.com/en-us/training/paths/build-dotnet-applications-csharp", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000027"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://nodejs.org/learn/getting-started/introduction-to-nodejs", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000028"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://nodejs.org/api/test.html", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000029"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://docs.docker.com/get-started", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000030"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://docs.docker.com/get-started/workshop", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000031"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://learn.microsoft.com/en-us/training/paths/microsoft-azure-fundamentals-describe-cloud-concepts", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000032"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://learn.microsoft.com/en-us/training/azure", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000033"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000034"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://jestjs.io/docs/getting-started", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000035"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://linuxjourney.com/", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000036"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://overthewire.org/wargames/bandit", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000037"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://docs.github.com/en/actions/get-started/quickstart", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000038"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://github.com/skills/hello-github-actions", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000039"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://support.microsoft.com/en-us/excel", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000040"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://support.microsoft.com/en-us/office/overview-of-formulas-in-excel-ecfdc708-9162-49e8-b993-c311f47ca173", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000041"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://learn.microsoft.com/en-us/training/powerplatform/power-bi", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000042"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://learn.microsoft.com/en-us/power-bi/create-reports/desktop-excel-stunning-report", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000043"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://learn.microsoft.com/en-us/training/paths/visualize-data-power-bi", null, null });

            migrationBuilder.UpdateData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000044"),
                columns: new[] { "canonical_url", "provider_key", "provider_resource_id" },
                values: new object[] { "https://www.tableau.com/learn/training", null, null });

            migrationBuilder.Sql("""
                UPDATE learning_resources
                SET canonical_url = CASE
                    WHEN split_part(url, '#', 1) ~ '^https://[^/]+/$'
                        THEN split_part(url, '#', 1)
                    ELSE regexp_replace(split_part(url, '#', 1), '/+$', '')
                END
                WHERE canonical_url IS NULL OR btrim(canonical_url) = '';
                """);

            migrationBuilder.AlterColumn<string>(
                name: "canonical_url",
                table: "learning_resources",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "learning_resources_skill_canonical_url_key",
                table: "learning_resources",
                columns: new[] { "skill_id", "canonical_url" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "learning_resources_skill_provider_resource_key",
                table: "learning_resources",
                columns: new[] { "skill_id", "provider_key", "provider_resource_id" },
                unique: true,
                filter: "provider_key IS NOT NULL AND provider_resource_id IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "learning_resources_provider_identity_check",
                table: "learning_resources",
                sql: "provider_resource_id IS NULL OR provider_key IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "learning_resources_skill_canonical_url_key",
                table: "learning_resources");

            migrationBuilder.DropIndex(
                name: "learning_resources_skill_provider_resource_key",
                table: "learning_resources");

            migrationBuilder.DropCheckConstraint(
                name: "learning_resources_provider_identity_check",
                table: "learning_resources");

            migrationBuilder.DropColumn(
                name: "canonical_url",
                table: "learning_resources");

            migrationBuilder.DropColumn(
                name: "provider_key",
                table: "learning_resources");

            migrationBuilder.DropColumn(
                name: "provider_resource_id",
                table: "learning_resources");
        }
    }
}
