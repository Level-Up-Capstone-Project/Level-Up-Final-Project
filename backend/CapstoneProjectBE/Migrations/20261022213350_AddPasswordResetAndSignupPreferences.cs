using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapstoneProjectBE.Migrations
{
    /// <summary>
    /// Applies the password-reset and signup-preferences database migration.
    /// </summary>
    public partial class AddPasswordResetAndSignupPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "industry",
                table: "user_profiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "user_profiles",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "target_roles",
                table: "user_profiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "work_areas",
                table: "user_profiles",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "password_resets",
                columns: table => new
                {
                    password_reset_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    failed_attempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("password_resets_pkey", x => x.password_reset_id);
                    table.ForeignKey(
                        name: "password_resets_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "password_resets_user_id_key",
                table: "password_resets",
                column: "user_id",
                unique: true);

            migrationBuilder.Sql("""
                ALTER TABLE password_resets ENABLE ROW LEVEL SECURITY;
                DO $block$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'anon') THEN
                        REVOKE ALL ON password_resets FROM anon;
                    END IF;
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'authenticated') THEN
                        REVOKE ALL ON password_resets FROM authenticated;
                    END IF;
                END
                $block$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "password_resets");

            migrationBuilder.DropColumn(
                name: "industry",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "target_roles",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "work_areas",
                table: "user_profiles");
        }
    }
}
