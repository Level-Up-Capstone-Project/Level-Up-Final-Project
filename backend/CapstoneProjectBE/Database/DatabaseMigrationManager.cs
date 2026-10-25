using System.Data;
using CapstoneProjectBE.Data;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Database;

/// <summary>
/// Provides Database Migration Manager database configuration and lifecycle behavior.
/// </summary>
public static class DatabaseMigrationManager
{
    private static readonly string[] RequiredTables =
    {
        "careers", "skills", "career_skills", "users", "user_profiles", "email_verifications", "jobs",
        "job_skills", "saved_jobs", "job_matches", "learning_resources", "learning_roadmaps", "roadmap_items",
        "roadmap_item_resources", "user_skills", "cvs", "cv_analyses", "cv_analysis_skills", "saved_resources",
        "skill_assessments", "assessment_questions", "assessment_responses"
    };

    public static async Task MigrateAsync(AppDbContext database, CancellationToken cancellationToken = default)
    {
        await database.Database.MigrateAsync(cancellationToken);
    }

    public static async Task AdoptExistingSchemaAsync(AppDbContext database, CancellationToken cancellationToken = default)
    {
        var migrations = database.Database.GetMigrations().ToArray();
        if (migrations.Length == 0) throw new InvalidOperationException("Schema adoption requires an EF baseline migration.");
        string baseline = migrations[0];
        if ((await database.Database.GetAppliedMigrationsAsync(cancellationToken)).Contains(baseline, StringComparer.Ordinal))
            throw new InvalidOperationException("The current database baseline has already been adopted.");

        await using var connection = database.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open) await connection.OpenAsync(cancellationToken);
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT COUNT(*) FROM unnest(ARRAY[" +
                string.Join(',', RequiredTables.Select((_, index) => $"@p{index}")) +
                "]::text[]) AS expected(name) WHERE to_regclass('public.' || expected.name) IS NOT NULL";
            for (var index = 0; index < RequiredTables.Length; index++)
            {
                var parameter = command.CreateParameter(); parameter.ParameterName = $"p{index}"; parameter.Value = RequiredTables[index]; command.Parameters.Add(parameter);
            }
            var present = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
            if (present != RequiredTables.Length)
                throw new InvalidOperationException($"Refusing to adopt an incomplete schema: found {present} of {RequiredTables.Length} required tables.");
        }

        await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);
        await database.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS external_logins (
                provider varchar(40) NOT NULL, subject varchar(255) NOT NULL,
                user_id uuid NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
                PRIMARY KEY (provider, subject), UNIQUE (user_id, provider));
            CREATE TABLE IF NOT EXISTS transcript_analyses (
                analysis_id uuid PRIMARY KEY, user_id uuid NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
                analysed_at timestamptz NOT NULL, result_json jsonb NOT NULL);
            ALTER TABLE assessment_questions ADD COLUMN IF NOT EXISTS choices text[] NOT NULL DEFAULT ARRAY[]::text[];
            ALTER TABLE assessment_questions ALTER COLUMN choices DROP DEFAULT;
            CREATE INDEX IF NOT EXISTS ix_transcript_analyses_user_date ON transcript_analyses(user_id, analysed_at DESC);
            CREATE UNIQUE INDEX IF NOT EXISTS saved_resources_user_resource_unique ON saved_resources(user_id, resource_id);
            CREATE INDEX IF NOT EXISTS saved_resources_resource_id_idx ON saved_resources(resource_id);
            CREATE UNIQUE INDEX IF NOT EXISTS skill_assessments_user_skill_attempt_unique ON skill_assessments(user_id, skill_id, attempt_number) WHERE attempt_number IS NOT NULL;
            CREATE INDEX IF NOT EXISTS skill_assessments_user_skill_idx ON skill_assessments(user_id, skill_id);
            CREATE UNIQUE INDEX IF NOT EXISTS assessment_questions_assessment_order_unique ON assessment_questions(skill_assessment_id, question_order);
            CREATE UNIQUE INDEX IF NOT EXISTS assessment_responses_question_id_unique ON assessment_responses(question_id);
            ALTER TABLE users ENABLE ROW LEVEL SECURITY;
            ALTER TABLE user_profiles ENABLE ROW LEVEL SECURITY;
            ALTER TABLE email_verifications ENABLE ROW LEVEL SECURITY;
            ALTER TABLE external_logins ENABLE ROW LEVEL SECURITY;
            ALTER TABLE transcript_analyses ENABLE ROW LEVEL SECURITY;
            ALTER TABLE cvs ENABLE ROW LEVEL SECURITY;
            ALTER TABLE cv_analyses ENABLE ROW LEVEL SECURITY;
            ALTER TABLE cv_analysis_skills ENABLE ROW LEVEL SECURITY;
            ALTER TABLE saved_jobs ENABLE ROW LEVEL SECURITY;
            ALTER TABLE job_matches ENABLE ROW LEVEL SECURITY;
            ALTER TABLE saved_resources ENABLE ROW LEVEL SECURITY;
            ALTER TABLE user_skills ENABLE ROW LEVEL SECURITY;
            ALTER TABLE learning_roadmaps ENABLE ROW LEVEL SECURITY;
            ALTER TABLE roadmap_items ENABLE ROW LEVEL SECURITY;
            ALTER TABLE roadmap_item_resources ENABLE ROW LEVEL SECURITY;
            ALTER TABLE skill_assessments ENABLE ROW LEVEL SECURITY;
            ALTER TABLE assessment_questions ENABLE ROW LEVEL SECURITY;
            ALTER TABLE assessment_responses ENABLE ROW LEVEL SECURITY;
            CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                "MigrationId" character varying(150) NOT NULL,
                "ProductVersion" character varying(32) NOT NULL,
                CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId"));
            DO $block$
            BEGIN
                IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'anon') THEN
                    REVOKE ALL ON careers, skills, career_skills, users, user_profiles, email_verifications,
                        jobs, job_skills, saved_jobs, job_matches, learning_resources, learning_roadmaps,
                        roadmap_items, roadmap_item_resources, user_skills, cvs, cv_analyses,
                        cv_analysis_skills, saved_resources, skill_assessments, assessment_questions,
                        assessment_responses, external_logins, transcript_analyses FROM anon;
                END IF;
                IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'authenticated') THEN
                    REVOKE ALL ON careers, skills, career_skills, users, user_profiles, email_verifications,
                        jobs, job_skills, saved_jobs, job_matches, learning_resources, learning_roadmaps,
                        roadmap_items, roadmap_item_resources, user_skills, cvs, cv_analyses,
                        cv_analysis_skills, saved_resources, skill_assessments, assessment_questions,
                        assessment_responses, external_logins, transcript_analyses FROM authenticated;
                END IF;
            END
            $block$;
            """, cancellationToken);

        await database.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") VALUES ({baseline}, {"8.0.31"}) ON CONFLICT (\"MigrationId\") DO NOTHING",
            cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await MigrateAsync(database, cancellationToken);
    }
}
