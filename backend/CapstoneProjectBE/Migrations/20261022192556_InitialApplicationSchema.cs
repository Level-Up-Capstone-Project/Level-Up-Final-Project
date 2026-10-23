using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CapstoneProjectBE.Migrations
{
    /// <inheritdoc />
    public partial class InitialApplicationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "careers",
                columns: table => new
                {
                    career_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    career_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    industry = table.Column<string>(type: "text", nullable: false),
                    education_requirements = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("careers_pkey", x => x.career_id);
                });

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    skill_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    category = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("skills_pkey", x => x.skill_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "jobs",
                columns: table => new
                {
                    job_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    mapped_career_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    company_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    qualification_requirement = table.Column<string>(type: "text", nullable: true),
                    experience_min = table.Column<int>(type: "integer", nullable: true),
                    experience_max = table.Column<int>(type: "integer", nullable: true),
                    job_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    application_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    source_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    source_listing_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    posted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    retrieved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("jobs_pkey", x => x.job_id);
                    table.ForeignKey(
                        name: "jobs_mapped_career_id_fkey",
                        column: x => x.mapped_career_id,
                        principalTable: "careers",
                        principalColumn: "career_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "career_skills",
                columns: table => new
                {
                    career_skill_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    career_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    importance_level = table.Column<int>(type: "integer", nullable: false),
                    is_core_skill = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("career_skills_pkey", x => x.career_skill_id);
                    table.ForeignKey(
                        name: "career_skills_career_id_fkey",
                        column: x => x.career_id,
                        principalTable: "careers",
                        principalColumn: "career_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "career_skills_skill_id_fkey",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "skill_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "learning_resources",
                columns: table => new
                {
                    resource_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    resource_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    resource_type = table.Column<string>(type: "text", nullable: false),
                    url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    provider = table.Column<string>(type: "text", nullable: false),
                    difficulty_level = table.Column<string>(type: "text", nullable: false),
                    cost_type = table.Column<string>(type: "text", nullable: false),
                    est_duration = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    source_type = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_learning_resources", x => x.resource_id);
                    table.ForeignKey(
                        name: "FK_learning_resources_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "skill_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cvs",
                columns: table => new
                {
                    cv_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    file_location = table.Column<string>(type: "text", nullable: false),
                    file_type = table.Column<string>(type: "text", nullable: false),
                    is_current = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("cvs_pkey", x => x.cv_id);
                    table.ForeignKey(
                        name: "cvs_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "email_verifications",
                columns: table => new
                {
                    email_verification_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    failed_attempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("email_verifications_pkey", x => x.email_verification_id);
                    table.ForeignKey(
                        name: "email_verifications_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "external_logins",
                columns: table => new
                {
                    provider = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_external_logins", x => new { x.provider, x.subject });
                    table.ForeignKey(
                        name: "FK_external_logins_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "learning_roadmaps",
                columns: table => new
                {
                    roadmap_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    career_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    progress_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_learning_roadmaps", x => x.roadmap_id);
                    table.ForeignKey(
                        name: "FK_learning_roadmaps_careers_career_id",
                        column: x => x.career_id,
                        principalTable: "careers",
                        principalColumn: "career_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "learning_roadmaps_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transcript_analyses",
                columns: table => new
                {
                    analysis_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    analysed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    result_json = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transcript_analyses", x => x.analysis_id);
                    table.ForeignKey(
                        name: "FK_transcript_analyses_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_career_id = table.Column<Guid>(type: "uuid", nullable: true),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    qualification = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    field_of_study = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    experience_level = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    interests = table.Column<string>(type: "text", nullable: true),
                    profile_completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_profiles_pkey", x => x.profile_id);
                    table.ForeignKey(
                        name: "user_profiles_target_career_id_fkey",
                        column: x => x.target_career_id,
                        principalTable: "careers",
                        principalColumn: "career_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "user_profiles_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_skills",
                columns: table => new
                {
                    user_skill_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "missing"),
                    is_self_reported = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_cv_detected = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_knowledge_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_skills_pkey", x => x.user_skill_id);
                    table.ForeignKey(
                        name: "user_skills_skill_id_fkey",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "skill_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "user_skills_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "job_matches",
                columns: table => new
                {
                    job_match_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_match_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    education_match_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    experience_match_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    career_alignment_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    overall_match_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    calculated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("job_matches_pkey", x => x.job_match_id);
                    table.ForeignKey(
                        name: "job_matches_job_id_fkey",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "job_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "job_matches_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "job_skills",
                columns: table => new
                {
                    job_skill_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requirement_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    importance_level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("job_skills_pkey", x => x.job_skill_id);
                    table.ForeignKey(
                        name: "job_skills_job_id_fkey",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "job_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "job_skills_skill_id_fkey",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "skill_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "saved_jobs",
                columns: table => new
                {
                    saved_job_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    saved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("saved_jobs_pkey", x => x.saved_job_id);
                    table.ForeignKey(
                        name: "saved_jobs_job_id_fkey",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "job_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "saved_jobs_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "saved_resources",
                columns: table => new
                {
                    saved_resource_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: true),
                    saved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("saved_resources_pkey", x => x.saved_resource_id);
                    table.ForeignKey(
                        name: "saved_resources_resource_id_fkey",
                        column: x => x.resource_id,
                        principalTable: "learning_resources",
                        principalColumn: "resource_id");
                    table.ForeignKey(
                        name: "saved_resources_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "cv_analyses",
                columns: table => new
                {
                    analysis_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    cv_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quality_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ats_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    formatting_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    strengths = table.Column<string>(type: "text", nullable: false),
                    weaknesses = table.Column<string>(type: "text", nullable: false),
                    recommendations = table.Column<string>(type: "text", nullable: false),
                    analysed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("cv_analyses_pkey", x => x.analysis_id);
                    table.ForeignKey(
                        name: "cv_analyses_cv_id_fkey",
                        column: x => x.cv_id,
                        principalTable: "cvs",
                        principalColumn: "cv_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "roadmap_items",
                columns: table => new
                {
                    roadmap_item_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    roadmap_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    step_order = table.Column<int>(type: "integer", nullable: false),
                    priority = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    target_date = table.Column<DateOnly>(type: "date", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roadmap_items", x => x.roadmap_item_id);
                    table.ForeignKey(
                        name: "FK_roadmap_items_learning_roadmaps_roadmap_id",
                        column: x => x.roadmap_id,
                        principalTable: "learning_roadmaps",
                        principalColumn: "roadmap_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_roadmap_items_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "skill_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cv_analysis_skills",
                columns: table => new
                {
                    cv_analysis_skill_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    analysis_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("cv_analysis_skills_pkey", x => x.cv_analysis_skill_id);
                    table.ForeignKey(
                        name: "cv_analysis_skills_analysis_id_fkey",
                        column: x => x.analysis_id,
                        principalTable: "cv_analyses",
                        principalColumn: "analysis_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "cv_analysis_skills_skill_id_fkey",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "skill_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "roadmap_item_resources",
                columns: table => new
                {
                    roadmap_item_resource_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    roadmap_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roadmap_item_resources", x => x.roadmap_item_resource_id);
                    table.ForeignKey(
                        name: "FK_roadmap_item_resources_learning_resources_resource_id",
                        column: x => x.resource_id,
                        principalTable: "learning_resources",
                        principalColumn: "resource_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_roadmap_item_resources_roadmap_items_roadmap_item_id",
                        column: x => x.roadmap_item_id,
                        principalTable: "roadmap_items",
                        principalColumn: "roadmap_item_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "skill_assessments",
                columns: table => new
                {
                    skill_assessment_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: true),
                    roadmap_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    attempt_number = table.Column<int>(type: "integer", nullable: true),
                    score_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    perfect_score = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    generated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("skill_assessments_pkey", x => x.skill_assessment_id);
                    table.ForeignKey(
                        name: "skill_assessments_roadmap_item_id_fkey",
                        column: x => x.roadmap_item_id,
                        principalTable: "roadmap_items",
                        principalColumn: "roadmap_item_id");
                    table.ForeignKey(
                        name: "skill_assessments_skill_id_fkey",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "skill_id");
                    table.ForeignKey(
                        name: "skill_assessments_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "assessment_questions",
                columns: table => new
                {
                    question_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    skill_assessment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_text = table.Column<string>(type: "text", nullable: false),
                    choices = table.Column<string[]>(type: "text[]", nullable: false),
                    correct_answer = table.Column<string>(type: "text", nullable: false),
                    question_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("assessment_questions_pkey", x => x.question_id);
                    table.ForeignKey(
                        name: "assessment_questions_skill_assessment_id_fkey",
                        column: x => x.skill_assessment_id,
                        principalTable: "skill_assessments",
                        principalColumn: "skill_assessment_id");
                });

            migrationBuilder.CreateTable(
                name: "assessment_responses",
                columns: table => new
                {
                    response_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_answer = table.Column<string>(type: "text", nullable: true),
                    is_correct = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("assessment_responses_pkey", x => x.response_id);
                    table.ForeignKey(
                        name: "assessment_responses_question_id_fkey",
                        column: x => x.question_id,
                        principalTable: "assessment_questions",
                        principalColumn: "question_id");
                });

            migrationBuilder.InsertData(
                table: "careers",
                columns: new[] { "career_id", "career_name", "description", "education_requirements", "industry" },
                values: new object[] { new Guid("10000000-0000-0000-0000-000000000001"), "Software Developer", "Designs, develops and maintains software applications.", "Relevant qualification or equivalent practical experience.", "Information Technology" });

            migrationBuilder.InsertData(
                table: "skills",
                columns: new[] { "skill_id", "category", "description", "skill_name" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "Technical", "Programming using the C# language.", "C#" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "Technical", "Querying and managing relational databases.", "SQL" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "Technical", "Designing and consuming RESTful APIs.", "REST APIs" }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "is_core_skill", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001"), 5, true, new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("30000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000001"), 4, true, new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("30000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000001"), 4, true, new Guid("20000000-0000-0000-0000-000000000003") }
                });

            migrationBuilder.CreateIndex(
                name: "assessment_questions_assessment_order_unique",
                table: "assessment_questions",
                columns: new[] { "skill_assessment_id", "question_order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "assessment_responses_question_id_unique",
                table: "assessment_responses",
                column: "question_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "career_skills_career_id_skill_id_key",
                table: "career_skills",
                columns: new[] { "career_id", "skill_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "career_skills_skill_id_idx",
                table: "career_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "careers_career_name_key",
                table: "careers",
                column: "career_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "cv_analyses_cv_id_idx",
                table: "cv_analyses",
                column: "cv_id");

            migrationBuilder.CreateIndex(
                name: "cv_analysis_skills_analysis_skill_unique",
                table: "cv_analysis_skills",
                columns: new[] { "analysis_id", "skill_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "cv_analysis_skills_skill_id_idx",
                table: "cv_analysis_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_cvs_user_id",
                table: "cvs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "email_verifications_user_id_key",
                table: "email_verifications",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_external_logins_user_id_provider",
                table: "external_logins",
                columns: new[] { "user_id", "provider" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_job_matches_job_id",
                table: "job_matches",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "job_matches_user_id_job_id_key",
                table: "job_matches",
                columns: new[] { "user_id", "job_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "job_matches_user_score_idx",
                table: "job_matches",
                columns: new[] { "user_id", "overall_match_score" });

            migrationBuilder.CreateIndex(
                name: "job_skills_job_id_skill_id_key",
                table: "job_skills",
                columns: new[] { "job_id", "skill_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "job_skills_skill_id_idx",
                table: "job_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "jobs_expires_at_idx",
                table: "jobs",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "jobs_mapped_career_id_idx",
                table: "jobs",
                column: "mapped_career_id");

            migrationBuilder.CreateIndex(
                name: "jobs_source_name_source_listing_id_key",
                table: "jobs",
                columns: new[] { "source_name", "source_listing_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_learning_resources_skill_id",
                table: "learning_resources",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_learning_roadmaps_career_id",
                table: "learning_roadmaps",
                column: "career_id");

            migrationBuilder.CreateIndex(
                name: "IX_learning_roadmaps_user_id",
                table: "learning_roadmaps",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_item_resources_resource_id",
                table: "roadmap_item_resources",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_item_resources_roadmap_item_id_resource_id",
                table: "roadmap_item_resources",
                columns: new[] { "roadmap_item_id", "resource_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_items_roadmap_id_step_order",
                table: "roadmap_items",
                columns: new[] { "roadmap_id", "step_order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_items_skill_id",
                table: "roadmap_items",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "saved_jobs_job_id_idx",
                table: "saved_jobs",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "saved_jobs_user_id_job_id_key",
                table: "saved_jobs",
                columns: new[] { "user_id", "job_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "saved_resources_resource_id_idx",
                table: "saved_resources",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "saved_resources_user_resource_unique",
                table: "saved_resources",
                columns: new[] { "user_id", "resource_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_skill_assessments_roadmap_item_id",
                table: "skill_assessments",
                column: "roadmap_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_skill_assessments_skill_id",
                table: "skill_assessments",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "skill_assessments_user_skill_attempt_unique",
                table: "skill_assessments",
                columns: new[] { "user_id", "skill_id", "attempt_number" },
                unique: true,
                filter: "attempt_number IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "skill_assessments_user_skill_idx",
                table: "skill_assessments",
                columns: new[] { "user_id", "skill_id" });

            migrationBuilder.CreateIndex(
                name: "skills_skill_name_key",
                table: "skills",
                column: "skill_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transcript_analyses_user_id_analysed_at",
                table: "transcript_analyses",
                columns: new[] { "user_id", "analysed_at" });

            migrationBuilder.CreateIndex(
                name: "user_profiles_target_career_id_idx",
                table: "user_profiles",
                column: "target_career_id");

            migrationBuilder.CreateIndex(
                name: "user_profiles_user_id_key",
                table: "user_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "user_skills_skill_id_idx",
                table: "user_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "user_skills_user_skill_unique",
                table: "user_skills",
                columns: new[] { "user_id", "skill_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_email_key",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.Sql("""
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
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assessment_responses");

            migrationBuilder.DropTable(
                name: "career_skills");

            migrationBuilder.DropTable(
                name: "cv_analysis_skills");

            migrationBuilder.DropTable(
                name: "email_verifications");

            migrationBuilder.DropTable(
                name: "external_logins");

            migrationBuilder.DropTable(
                name: "job_matches");

            migrationBuilder.DropTable(
                name: "job_skills");

            migrationBuilder.DropTable(
                name: "roadmap_item_resources");

            migrationBuilder.DropTable(
                name: "saved_jobs");

            migrationBuilder.DropTable(
                name: "saved_resources");

            migrationBuilder.DropTable(
                name: "transcript_analyses");

            migrationBuilder.DropTable(
                name: "user_profiles");

            migrationBuilder.DropTable(
                name: "user_skills");

            migrationBuilder.DropTable(
                name: "assessment_questions");

            migrationBuilder.DropTable(
                name: "cv_analyses");

            migrationBuilder.DropTable(
                name: "jobs");

            migrationBuilder.DropTable(
                name: "learning_resources");

            migrationBuilder.DropTable(
                name: "skill_assessments");

            migrationBuilder.DropTable(
                name: "cvs");

            migrationBuilder.DropTable(
                name: "roadmap_items");

            migrationBuilder.DropTable(
                name: "learning_roadmaps");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropTable(
                name: "careers");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
