using CapstoneProjectBE.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapstoneProjectBE.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261023010000_ArchiveLegacyPublicTables")]
public partial class ArchiveLegacyPublicTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE SCHEMA IF NOT EXISTS legacy;
            REVOKE ALL ON SCHEMA legacy FROM PUBLIC;

            DO $block$
            BEGIN
                IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'anon') THEN
                    REVOKE ALL ON TABLE public."__EFMigrationsHistory" FROM anon;
                    REVOKE ALL ON SCHEMA legacy FROM anon;
                END IF;
                IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'authenticated') THEN
                    REVOKE ALL ON TABLE public."__EFMigrationsHistory" FROM authenticated;
                    REVOKE ALL ON SCHEMA legacy FROM authenticated;
                END IF;

                IF to_regclass('public."CareerSkills"') IS NOT NULL AND to_regclass('legacy."CareerSkills"') IS NULL THEN
                    ALTER TABLE public."CareerSkills" SET SCHEMA legacy;
                END IF;
                IF to_regclass('public."Careers"') IS NOT NULL AND to_regclass('legacy."Careers"') IS NULL THEN
                    ALTER TABLE public."Careers" SET SCHEMA legacy;
                END IF;
                IF to_regclass('public."Skills"') IS NOT NULL AND to_regclass('legacy."Skills"') IS NULL THEN
                    ALTER TABLE public."Skills" SET SCHEMA legacy;
                END IF;
                IF to_regclass('public.cv_analysis') IS NOT NULL AND to_regclass('legacy.cv_analysis') IS NULL THEN
                    ALTER TABLE public.cv_analysis SET SCHEMA legacy;
                END IF;

                IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'anon') THEN
                    REVOKE ALL ON ALL TABLES IN SCHEMA legacy FROM anon;
                END IF;
                IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'authenticated') THEN
                    REVOKE ALL ON ALL TABLES IN SCHEMA legacy FROM authenticated;
                END IF;
            END
            $block$;

            REVOKE ALL ON ALL TABLES IN SCHEMA legacy FROM PUBLIC;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DO $block$
            BEGIN
                IF to_regclass('legacy."Careers"') IS NOT NULL AND to_regclass('public."Careers"') IS NULL THEN
                    ALTER TABLE legacy."Careers" SET SCHEMA public;
                END IF;
                IF to_regclass('legacy."Skills"') IS NOT NULL AND to_regclass('public."Skills"') IS NULL THEN
                    ALTER TABLE legacy."Skills" SET SCHEMA public;
                END IF;
                IF to_regclass('legacy."CareerSkills"') IS NOT NULL AND to_regclass('public."CareerSkills"') IS NULL THEN
                    ALTER TABLE legacy."CareerSkills" SET SCHEMA public;
                END IF;
                IF to_regclass('legacy.cv_analysis') IS NOT NULL AND to_regclass('public.cv_analysis') IS NULL THEN
                    ALTER TABLE legacy.cv_analysis SET SCHEMA public;
                END IF;
            END
            $block$;
            """);
    }
}
