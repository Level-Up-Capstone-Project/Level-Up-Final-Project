using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CapstoneProjectBE.Migrations
{
    /// <inheritdoc />
    public partial class SeedMvpCareerCatalogue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                column: "importance_level",
                value: 5);

            migrationBuilder.UpdateData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                column: "importance_level",
                value: 5);

            migrationBuilder.UpdateData(
                table: "careers",
                keyColumn: "career_id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "description", "education_requirements", "industry" },
                values: new object[] { "Designs, builds, tests and maintains software applications.", "Relevant qualification, portfolio work, or equivalent practical experience.", "Software Engineering" });

            migrationBuilder.InsertData(
                table: "careers",
                columns: new[] { "career_id", "career_name", "description", "education_requirements", "industry" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Frontend Developer", "Builds accessible, responsive and interactive web user interfaces.", "Relevant qualification, portfolio work, or equivalent practical experience.", "Web Development" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Backend Developer", "Builds secure APIs, business services and database-backed applications.", "Relevant qualification, portfolio work, or equivalent practical experience.", "Software Engineering" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "Full Stack Developer", "Develops complete web products across browser, server and database layers.", "Relevant qualification, portfolio work, or equivalent practical experience.", "Web Development" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "Data Analyst", "Transforms data into reliable analysis, dashboards and business insights.", "Relevant qualification, portfolio work, or equivalent practical experience.", "Data and Analytics" },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "DevOps Engineer", "Automates software delivery and operates reliable cloud infrastructure.", "Relevant qualification, portfolio work, or equivalent practical experience.", "Cloud and Infrastructure" }
                });

            migrationBuilder.InsertData(
                table: "jobs",
                columns: new[] { "job_id", "application_url", "company_name", "description", "experience_max", "experience_min", "expires_at", "job_title", "job_type", "location", "mapped_career_id", "posted_at", "qualification_requirement", "retrieved_at", "source_listing_id", "source_name" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), "https://www.linkedin.com/jobs/search/?keywords=software%20developer&location=South%20Africa", "LinkedIn Jobs", "A curated search for current Software Developer vacancies. Open the listing link to review live roles, employers and closing dates on LinkedIn Jobs.", null, 0, null, "Software Developer opportunities", "Curated search", "South Africa / Remote", new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Relevant qualification, portfolio work, or equivalent practical experience.", new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "linkedin-software-developer", "LevelUpCatalogue" },
                    { new Guid("50000000-0000-0000-0000-000000000002"), "https://za.indeed.com/jobs?q=software+developer", "Indeed South Africa", "A curated search for current Software Developer vacancies. Open the listing link to review live roles, employers and closing dates on Indeed South Africa.", null, 0, null, "Software Developer opportunities", "Curated search", "South Africa / Remote", new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Relevant qualification, portfolio work, or equivalent practical experience.", new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "indeed-software-developer", "LevelUpCatalogue" }
                });

            migrationBuilder.InsertData(
                table: "learning_resources",
                columns: new[] { "resource_id", "cost_type", "description", "difficulty_level", "est_duration", "is_active", "provider", "resource_name", "resource_type", "skill_id", "source_type", "url" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), "Free", "Learn C# syntax and build small console programs.", "Beginner", "4 hours", true, "Microsoft Learn", "Get started with C#", "Learning Path", new Guid("20000000-0000-0000-0000-000000000001"), "Curated", "https://learn.microsoft.com/en-us/training/paths/get-started-c-sharp-part-1/" },
                    { new Guid("40000000-0000-0000-0000-000000000002"), "Free", "Practise C# with mentored coding exercises.", "Beginner", "Self-paced", true, "Exercism", "C# practice exercises", "Practice", new Guid("20000000-0000-0000-0000-000000000001"), "Curated", "https://exercism.org/tracks/csharp" },
                    { new Guid("40000000-0000-0000-0000-000000000003"), "Free", "Learn relational querying directly from the PostgreSQL documentation.", "Beginner", "3 hours", true, "PostgreSQL", "PostgreSQL SQL tutorial", "Tutorial", new Guid("20000000-0000-0000-0000-000000000002"), "Curated", "https://www.postgresql.org/docs/current/tutorial-sql.html" },
                    { new Guid("40000000-0000-0000-0000-000000000004"), "Free", "Complete browser-based SQL lessons and exercises.", "Beginner", "3 hours", true, "SQLBolt", "Interactive SQL lessons", "Practice", new Guid("20000000-0000-0000-0000-000000000002"), "Curated", "https://sqlbolt.com/" },
                    { new Guid("40000000-0000-0000-0000-000000000005"), "Free", "Understand requests, responses, methods and status codes.", "Beginner", "1 hour", true, "MDN", "Overview of HTTP", "Guide", new Guid("20000000-0000-0000-0000-000000000003"), "Curated", "https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Overview" },
                    { new Guid("40000000-0000-0000-0000-000000000006"), "Free", "Build and test a REST API with ASP.NET Core.", "Intermediate", "3 hours", true, "Microsoft Learn", "Build a web API", "Project", new Guid("20000000-0000-0000-0000-000000000003"), "Curated", "https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api" }
                });

            migrationBuilder.UpdateData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                columns: new[] { "category", "description" },
                values: new object[] { "Programming Language", "Object-oriented programming with C# and the .NET ecosystem." });

            migrationBuilder.UpdateData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                columns: new[] { "category", "description" },
                values: new object[] { "Database", "Querying, joining and managing data in relational databases." });

            migrationBuilder.UpdateData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"),
                columns: new[] { "category", "description" },
                values: new object[] { "Web Development", "Designing, consuming and testing HTTP-based RESTful APIs." });

            migrationBuilder.InsertData(
                table: "skills",
                columns: new[] { "skill_id", "category", "description", "skill_name" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000004"), "Programming Language", "Object-oriented application development with Java.", "Java" },
                    { new Guid("20000000-0000-0000-0000-000000000005"), "Programming Language", "Programming and automation using Python.", "Python" },
                    { new Guid("20000000-0000-0000-0000-000000000006"), "Database", "Designing and operating PostgreSQL relational databases.", "PostgreSQL" },
                    { new Guid("20000000-0000-0000-0000-000000000007"), "Frontend", "Building component-based web interfaces with React.", "React" },
                    { new Guid("20000000-0000-0000-0000-000000000008"), "Frontend", "Structuring accessible web content with semantic HTML.", "HTML" },
                    { new Guid("20000000-0000-0000-0000-000000000009"), "Frontend", "Styling responsive web interfaces with modern CSS.", "CSS" },
                    { new Guid("20000000-0000-0000-0000-000000000010"), "Programming Language", "Programming interactive web applications with JavaScript.", "JavaScript" },
                    { new Guid("20000000-0000-0000-0000-000000000011"), "Programming Language", "Building type-safe JavaScript applications with TypeScript.", "TypeScript" },
                    { new Guid("20000000-0000-0000-0000-000000000012"), "Developer Tools", "Using distributed version control for collaborative software development.", "Git" },
                    { new Guid("20000000-0000-0000-0000-000000000013"), "Backend", "Building applications and services on the .NET platform.", ".NET" },
                    { new Guid("20000000-0000-0000-0000-000000000014"), "Backend", "Building server-side JavaScript applications with Node.js.", "Node.js" },
                    { new Guid("20000000-0000-0000-0000-000000000015"), "DevOps", "Packaging and running applications in containers.", "Docker" },
                    { new Guid("20000000-0000-0000-0000-000000000016"), "Cloud", "Building and operating workloads on Microsoft Azure.", "Azure" },
                    { new Guid("20000000-0000-0000-0000-000000000017"), "Software Quality", "Designing automated tests that protect software behaviour.", "Testing" },
                    { new Guid("20000000-0000-0000-0000-000000000018"), "Operating Systems", "Working confidently with Linux systems and command-line tools.", "Linux" },
                    { new Guid("20000000-0000-0000-0000-000000000019"), "DevOps", "Automating builds, tests and deployments through delivery pipelines.", "CI/CD" },
                    { new Guid("20000000-0000-0000-0000-000000000020"), "Data and Analytics", "Analysing and presenting data with Microsoft Excel.", "Excel" },
                    { new Guid("20000000-0000-0000-0000-000000000021"), "Data and Analytics", "Creating data models and interactive reports with Power BI.", "Power BI" },
                    { new Guid("20000000-0000-0000-0000-000000000022"), "Data and Analytics", "Communicating patterns and insights through effective visualisations.", "Data Visualization" }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000001"), 3, new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("30000000-0000-0000-0000-000000000005"), new Guid("10000000-0000-0000-0000-000000000001"), 3, new Guid("20000000-0000-0000-0000-000000000005") },
                    { new Guid("30000000-0000-0000-0000-000000000006"), new Guid("10000000-0000-0000-0000-000000000001"), 4, new Guid("20000000-0000-0000-0000-000000000006") },
                    { new Guid("30000000-0000-0000-0000-000000000007"), new Guid("10000000-0000-0000-0000-000000000001"), 3, new Guid("20000000-0000-0000-0000-000000000007") },
                    { new Guid("30000000-0000-0000-0000-000000000008"), new Guid("10000000-0000-0000-0000-000000000001"), 3, new Guid("20000000-0000-0000-0000-000000000008") },
                    { new Guid("30000000-0000-0000-0000-000000000009"), new Guid("10000000-0000-0000-0000-000000000001"), 3, new Guid("20000000-0000-0000-0000-000000000009") },
                    { new Guid("30000000-0000-0000-0000-000000000010"), new Guid("10000000-0000-0000-0000-000000000001"), 4, new Guid("20000000-0000-0000-0000-000000000010") }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "is_core_skill", "skill_id" },
                values: new object[] { new Guid("30000000-0000-0000-0000-000000000011"), new Guid("10000000-0000-0000-0000-000000000001"), 4, true, new Guid("20000000-0000-0000-0000-000000000012") });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "skill_id" },
                values: new object[] { new Guid("30000000-0000-0000-0000-000000000012"), new Guid("10000000-0000-0000-0000-000000000001"), 4, new Guid("20000000-0000-0000-0000-000000000013") });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "is_core_skill", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000013"), new Guid("10000000-0000-0000-0000-000000000001"), 4, true, new Guid("20000000-0000-0000-0000-000000000017") },
                    { new Guid("30000000-0000-0000-0000-000000000014"), new Guid("10000000-0000-0000-0000-000000000002"), 5, true, new Guid("20000000-0000-0000-0000-000000000008") },
                    { new Guid("30000000-0000-0000-0000-000000000015"), new Guid("10000000-0000-0000-0000-000000000002"), 5, true, new Guid("20000000-0000-0000-0000-000000000009") },
                    { new Guid("30000000-0000-0000-0000-000000000016"), new Guid("10000000-0000-0000-0000-000000000002"), 5, true, new Guid("20000000-0000-0000-0000-000000000010") }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "skill_id" },
                values: new object[] { new Guid("30000000-0000-0000-0000-000000000017"), new Guid("10000000-0000-0000-0000-000000000002"), 4, new Guid("20000000-0000-0000-0000-000000000011") });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "is_core_skill", "skill_id" },
                values: new object[] { new Guid("30000000-0000-0000-0000-000000000018"), new Guid("10000000-0000-0000-0000-000000000002"), 5, true, new Guid("20000000-0000-0000-0000-000000000007") });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000019"), new Guid("10000000-0000-0000-0000-000000000002"), 4, new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("30000000-0000-0000-0000-000000000020"), new Guid("10000000-0000-0000-0000-000000000002"), 4, new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("30000000-0000-0000-0000-000000000021"), new Guid("10000000-0000-0000-0000-000000000002"), 4, new Guid("20000000-0000-0000-0000-000000000017") }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "is_core_skill", "skill_id" },
                values: new object[] { new Guid("30000000-0000-0000-0000-000000000022"), new Guid("10000000-0000-0000-0000-000000000003"), 5, true, new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000023"), new Guid("10000000-0000-0000-0000-000000000003"), 4, new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("30000000-0000-0000-0000-000000000024"), new Guid("10000000-0000-0000-0000-000000000003"), 4, new Guid("20000000-0000-0000-0000-000000000005") }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "is_core_skill", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000025"), new Guid("10000000-0000-0000-0000-000000000003"), 5, true, new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("30000000-0000-0000-0000-000000000026"), new Guid("10000000-0000-0000-0000-000000000003"), 5, true, new Guid("20000000-0000-0000-0000-000000000006") },
                    { new Guid("30000000-0000-0000-0000-000000000027"), new Guid("10000000-0000-0000-0000-000000000003"), 5, true, new Guid("20000000-0000-0000-0000-000000000003") }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000028"), new Guid("10000000-0000-0000-0000-000000000003"), 5, new Guid("20000000-0000-0000-0000-000000000013") },
                    { new Guid("30000000-0000-0000-0000-000000000029"), new Guid("10000000-0000-0000-0000-000000000003"), 4, new Guid("20000000-0000-0000-0000-000000000014") },
                    { new Guid("30000000-0000-0000-0000-000000000030"), new Guid("10000000-0000-0000-0000-000000000003"), 4, new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("30000000-0000-0000-0000-000000000031"), new Guid("10000000-0000-0000-0000-000000000003"), 3, new Guid("20000000-0000-0000-0000-000000000015") },
                    { new Guid("30000000-0000-0000-0000-000000000032"), new Guid("10000000-0000-0000-0000-000000000003"), 4, new Guid("20000000-0000-0000-0000-000000000017") },
                    { new Guid("30000000-0000-0000-0000-000000000033"), new Guid("10000000-0000-0000-0000-000000000004"), 4, new Guid("20000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "is_core_skill", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000034"), new Guid("10000000-0000-0000-0000-000000000004"), 5, true, new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("30000000-0000-0000-0000-000000000035"), new Guid("10000000-0000-0000-0000-000000000004"), 5, true, new Guid("20000000-0000-0000-0000-000000000003") }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000036"), new Guid("10000000-0000-0000-0000-000000000004"), 3, new Guid("20000000-0000-0000-0000-000000000005") },
                    { new Guid("30000000-0000-0000-0000-000000000037"), new Guid("10000000-0000-0000-0000-000000000004"), 5, new Guid("20000000-0000-0000-0000-000000000006") }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "is_core_skill", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000038"), new Guid("10000000-0000-0000-0000-000000000004"), 5, true, new Guid("20000000-0000-0000-0000-000000000007") },
                    { new Guid("30000000-0000-0000-0000-000000000039"), new Guid("10000000-0000-0000-0000-000000000004"), 5, true, new Guid("20000000-0000-0000-0000-000000000008") },
                    { new Guid("30000000-0000-0000-0000-000000000040"), new Guid("10000000-0000-0000-0000-000000000004"), 5, true, new Guid("20000000-0000-0000-0000-000000000009") },
                    { new Guid("30000000-0000-0000-0000-000000000041"), new Guid("10000000-0000-0000-0000-000000000004"), 5, true, new Guid("20000000-0000-0000-0000-000000000010") }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000042"), new Guid("10000000-0000-0000-0000-000000000004"), 4, new Guid("20000000-0000-0000-0000-000000000011") },
                    { new Guid("30000000-0000-0000-0000-000000000043"), new Guid("10000000-0000-0000-0000-000000000004"), 4, new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("30000000-0000-0000-0000-000000000044"), new Guid("10000000-0000-0000-0000-000000000004"), 4, new Guid("20000000-0000-0000-0000-000000000013") },
                    { new Guid("30000000-0000-0000-0000-000000000045"), new Guid("10000000-0000-0000-0000-000000000004"), 4, new Guid("20000000-0000-0000-0000-000000000014") },
                    { new Guid("30000000-0000-0000-0000-000000000046"), new Guid("10000000-0000-0000-0000-000000000004"), 3, new Guid("20000000-0000-0000-0000-000000000015") },
                    { new Guid("30000000-0000-0000-0000-000000000047"), new Guid("10000000-0000-0000-0000-000000000004"), 4, new Guid("20000000-0000-0000-0000-000000000017") }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "is_core_skill", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000048"), new Guid("10000000-0000-0000-0000-000000000005"), 5, true, new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("30000000-0000-0000-0000-000000000049"), new Guid("10000000-0000-0000-0000-000000000005"), 5, true, new Guid("20000000-0000-0000-0000-000000000005") }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "skill_id" },
                values: new object[] { new Guid("30000000-0000-0000-0000-000000000050"), new Guid("10000000-0000-0000-0000-000000000005"), 4, new Guid("20000000-0000-0000-0000-000000000006") });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "is_core_skill", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000051"), new Guid("10000000-0000-0000-0000-000000000005"), 5, true, new Guid("20000000-0000-0000-0000-000000000020") },
                    { new Guid("30000000-0000-0000-0000-000000000052"), new Guid("10000000-0000-0000-0000-000000000005"), 5, true, new Guid("20000000-0000-0000-0000-000000000021") },
                    { new Guid("30000000-0000-0000-0000-000000000053"), new Guid("10000000-0000-0000-0000-000000000005"), 5, true, new Guid("20000000-0000-0000-0000-000000000022") }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000054"), new Guid("10000000-0000-0000-0000-000000000005"), 2, new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("30000000-0000-0000-0000-000000000055"), new Guid("10000000-0000-0000-0000-000000000006"), 4, new Guid("20000000-0000-0000-0000-000000000005") },
                    { new Guid("30000000-0000-0000-0000-000000000056"), new Guid("10000000-0000-0000-0000-000000000006"), 3, new Guid("20000000-0000-0000-0000-000000000006") }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "is_core_skill", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000057"), new Guid("10000000-0000-0000-0000-000000000006"), 5, true, new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("30000000-0000-0000-0000-000000000058"), new Guid("10000000-0000-0000-0000-000000000006"), 5, true, new Guid("20000000-0000-0000-0000-000000000015") },
                    { new Guid("30000000-0000-0000-0000-000000000059"), new Guid("10000000-0000-0000-0000-000000000006"), 5, true, new Guid("20000000-0000-0000-0000-000000000016") }
                });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "skill_id" },
                values: new object[] { new Guid("30000000-0000-0000-0000-000000000060"), new Guid("10000000-0000-0000-0000-000000000006"), 3, new Guid("20000000-0000-0000-0000-000000000017") });

            migrationBuilder.InsertData(
                table: "career_skills",
                columns: new[] { "career_skill_id", "career_id", "importance_level", "is_core_skill", "skill_id" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000061"), new Guid("10000000-0000-0000-0000-000000000006"), 5, true, new Guid("20000000-0000-0000-0000-000000000018") },
                    { new Guid("30000000-0000-0000-0000-000000000062"), new Guid("10000000-0000-0000-0000-000000000006"), 5, true, new Guid("20000000-0000-0000-0000-000000000019") }
                });

            migrationBuilder.InsertData(
                table: "job_skills",
                columns: new[] { "job_skill_id", "importance_level", "job_id", "requirement_type", "skill_id" },
                values: new object[,]
                {
                    { new Guid("60000000-0000-0000-0000-000000000001"), 5, new Guid("50000000-0000-0000-0000-000000000001"), "Required", new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("60000000-0000-0000-0000-000000000002"), 5, new Guid("50000000-0000-0000-0000-000000000001"), "Required", new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("60000000-0000-0000-0000-000000000003"), 5, new Guid("50000000-0000-0000-0000-000000000001"), "Required", new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("60000000-0000-0000-0000-000000000004"), 3, new Guid("50000000-0000-0000-0000-000000000001"), "Preferred", new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("60000000-0000-0000-0000-000000000005"), 3, new Guid("50000000-0000-0000-0000-000000000001"), "Preferred", new Guid("20000000-0000-0000-0000-000000000005") },
                    { new Guid("60000000-0000-0000-0000-000000000006"), 4, new Guid("50000000-0000-0000-0000-000000000001"), "Preferred", new Guid("20000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000007"), 3, new Guid("50000000-0000-0000-0000-000000000001"), "Preferred", new Guid("20000000-0000-0000-0000-000000000007") },
                    { new Guid("60000000-0000-0000-0000-000000000008"), 3, new Guid("50000000-0000-0000-0000-000000000001"), "Preferred", new Guid("20000000-0000-0000-0000-000000000008") },
                    { new Guid("60000000-0000-0000-0000-000000000009"), 3, new Guid("50000000-0000-0000-0000-000000000001"), "Preferred", new Guid("20000000-0000-0000-0000-000000000009") },
                    { new Guid("60000000-0000-0000-0000-000000000010"), 4, new Guid("50000000-0000-0000-0000-000000000001"), "Preferred", new Guid("20000000-0000-0000-0000-000000000010") },
                    { new Guid("60000000-0000-0000-0000-000000000011"), 4, new Guid("50000000-0000-0000-0000-000000000001"), "Required", new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("60000000-0000-0000-0000-000000000012"), 4, new Guid("50000000-0000-0000-0000-000000000001"), "Preferred", new Guid("20000000-0000-0000-0000-000000000013") },
                    { new Guid("60000000-0000-0000-0000-000000000013"), 4, new Guid("50000000-0000-0000-0000-000000000001"), "Required", new Guid("20000000-0000-0000-0000-000000000017") },
                    { new Guid("60000000-0000-0000-0000-000000000014"), 5, new Guid("50000000-0000-0000-0000-000000000002"), "Required", new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("60000000-0000-0000-0000-000000000015"), 5, new Guid("50000000-0000-0000-0000-000000000002"), "Required", new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("60000000-0000-0000-0000-000000000016"), 5, new Guid("50000000-0000-0000-0000-000000000002"), "Required", new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("60000000-0000-0000-0000-000000000017"), 3, new Guid("50000000-0000-0000-0000-000000000002"), "Preferred", new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("60000000-0000-0000-0000-000000000018"), 3, new Guid("50000000-0000-0000-0000-000000000002"), "Preferred", new Guid("20000000-0000-0000-0000-000000000005") },
                    { new Guid("60000000-0000-0000-0000-000000000019"), 4, new Guid("50000000-0000-0000-0000-000000000002"), "Preferred", new Guid("20000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000020"), 3, new Guid("50000000-0000-0000-0000-000000000002"), "Preferred", new Guid("20000000-0000-0000-0000-000000000007") },
                    { new Guid("60000000-0000-0000-0000-000000000021"), 3, new Guid("50000000-0000-0000-0000-000000000002"), "Preferred", new Guid("20000000-0000-0000-0000-000000000008") },
                    { new Guid("60000000-0000-0000-0000-000000000022"), 3, new Guid("50000000-0000-0000-0000-000000000002"), "Preferred", new Guid("20000000-0000-0000-0000-000000000009") },
                    { new Guid("60000000-0000-0000-0000-000000000023"), 4, new Guid("50000000-0000-0000-0000-000000000002"), "Preferred", new Guid("20000000-0000-0000-0000-000000000010") },
                    { new Guid("60000000-0000-0000-0000-000000000024"), 4, new Guid("50000000-0000-0000-0000-000000000002"), "Required", new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("60000000-0000-0000-0000-000000000025"), 4, new Guid("50000000-0000-0000-0000-000000000002"), "Preferred", new Guid("20000000-0000-0000-0000-000000000013") },
                    { new Guid("60000000-0000-0000-0000-000000000026"), 4, new Guid("50000000-0000-0000-0000-000000000002"), "Required", new Guid("20000000-0000-0000-0000-000000000017") }
                });

            migrationBuilder.InsertData(
                table: "jobs",
                columns: new[] { "job_id", "application_url", "company_name", "description", "experience_max", "experience_min", "expires_at", "job_title", "job_type", "location", "mapped_career_id", "posted_at", "qualification_requirement", "retrieved_at", "source_listing_id", "source_name" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000003"), "https://www.linkedin.com/jobs/search/?keywords=frontend%20developer&location=South%20Africa", "LinkedIn Jobs", "A curated search for current Frontend Developer vacancies. Open the listing link to review live roles, employers and closing dates on LinkedIn Jobs.", null, 0, null, "Frontend Developer opportunities", "Curated search", "South Africa / Remote", new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Relevant qualification, portfolio work, or equivalent practical experience.", new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "linkedin-frontend-developer", "LevelUpCatalogue" },
                    { new Guid("50000000-0000-0000-0000-000000000004"), "https://za.indeed.com/jobs?q=frontend+developer", "Indeed South Africa", "A curated search for current Frontend Developer vacancies. Open the listing link to review live roles, employers and closing dates on Indeed South Africa.", null, 0, null, "Frontend Developer opportunities", "Curated search", "South Africa / Remote", new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Relevant qualification, portfolio work, or equivalent practical experience.", new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "indeed-frontend-developer", "LevelUpCatalogue" },
                    { new Guid("50000000-0000-0000-0000-000000000005"), "https://www.linkedin.com/jobs/search/?keywords=backend%20developer&location=South%20Africa", "LinkedIn Jobs", "A curated search for current Backend Developer vacancies. Open the listing link to review live roles, employers and closing dates on LinkedIn Jobs.", null, 0, null, "Backend Developer opportunities", "Curated search", "South Africa / Remote", new Guid("10000000-0000-0000-0000-000000000003"), new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Relevant qualification, portfolio work, or equivalent practical experience.", new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "linkedin-backend-developer", "LevelUpCatalogue" },
                    { new Guid("50000000-0000-0000-0000-000000000006"), "https://za.indeed.com/jobs?q=backend+developer", "Indeed South Africa", "A curated search for current Backend Developer vacancies. Open the listing link to review live roles, employers and closing dates on Indeed South Africa.", null, 0, null, "Backend Developer opportunities", "Curated search", "South Africa / Remote", new Guid("10000000-0000-0000-0000-000000000003"), new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Relevant qualification, portfolio work, or equivalent practical experience.", new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "indeed-backend-developer", "LevelUpCatalogue" },
                    { new Guid("50000000-0000-0000-0000-000000000007"), "https://www.linkedin.com/jobs/search/?keywords=full%20stack%20developer&location=South%20Africa", "LinkedIn Jobs", "A curated search for current Full Stack Developer vacancies. Open the listing link to review live roles, employers and closing dates on LinkedIn Jobs.", null, 0, null, "Full Stack Developer opportunities", "Curated search", "South Africa / Remote", new Guid("10000000-0000-0000-0000-000000000004"), new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Relevant qualification, portfolio work, or equivalent practical experience.", new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "linkedin-full-stack-developer", "LevelUpCatalogue" },
                    { new Guid("50000000-0000-0000-0000-000000000008"), "https://za.indeed.com/jobs?q=full+stack+developer", "Indeed South Africa", "A curated search for current Full Stack Developer vacancies. Open the listing link to review live roles, employers and closing dates on Indeed South Africa.", null, 0, null, "Full Stack Developer opportunities", "Curated search", "South Africa / Remote", new Guid("10000000-0000-0000-0000-000000000004"), new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Relevant qualification, portfolio work, or equivalent practical experience.", new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "indeed-full-stack-developer", "LevelUpCatalogue" },
                    { new Guid("50000000-0000-0000-0000-000000000009"), "https://www.linkedin.com/jobs/search/?keywords=data%20analyst&location=South%20Africa", "LinkedIn Jobs", "A curated search for current Data Analyst vacancies. Open the listing link to review live roles, employers and closing dates on LinkedIn Jobs.", null, 0, null, "Data Analyst opportunities", "Curated search", "South Africa / Remote", new Guid("10000000-0000-0000-0000-000000000005"), new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Relevant qualification, portfolio work, or equivalent practical experience.", new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "linkedin-data-analyst", "LevelUpCatalogue" },
                    { new Guid("50000000-0000-0000-0000-000000000010"), "https://za.indeed.com/jobs?q=data+analyst", "Indeed South Africa", "A curated search for current Data Analyst vacancies. Open the listing link to review live roles, employers and closing dates on Indeed South Africa.", null, 0, null, "Data Analyst opportunities", "Curated search", "South Africa / Remote", new Guid("10000000-0000-0000-0000-000000000005"), new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Relevant qualification, portfolio work, or equivalent practical experience.", new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "indeed-data-analyst", "LevelUpCatalogue" },
                    { new Guid("50000000-0000-0000-0000-000000000011"), "https://www.linkedin.com/jobs/search/?keywords=devops%20engineer&location=South%20Africa", "LinkedIn Jobs", "A curated search for current DevOps Engineer vacancies. Open the listing link to review live roles, employers and closing dates on LinkedIn Jobs.", null, 0, null, "DevOps Engineer opportunities", "Curated search", "South Africa / Remote", new Guid("10000000-0000-0000-0000-000000000006"), new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Relevant qualification, portfolio work, or equivalent practical experience.", new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "linkedin-devops-engineer", "LevelUpCatalogue" },
                    { new Guid("50000000-0000-0000-0000-000000000012"), "https://za.indeed.com/jobs?q=devops+engineer", "Indeed South Africa", "A curated search for current DevOps Engineer vacancies. Open the listing link to review live roles, employers and closing dates on Indeed South Africa.", null, 0, null, "DevOps Engineer opportunities", "Curated search", "South Africa / Remote", new Guid("10000000-0000-0000-0000-000000000006"), new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Relevant qualification, portfolio work, or equivalent practical experience.", new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "indeed-devops-engineer", "LevelUpCatalogue" }
                });

            migrationBuilder.InsertData(
                table: "learning_resources",
                columns: new[] { "resource_id", "cost_type", "description", "difficulty_level", "est_duration", "is_active", "provider", "resource_name", "resource_type", "skill_id", "source_type", "url" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000007"), "Free", "Learn Java language basics and core APIs.", "Beginner", "6 hours", true, "Dev.java", "Learn Java", "Tutorial", new Guid("20000000-0000-0000-0000-000000000004"), "Curated", "https://dev.java/learn/" },
                    { new Guid("40000000-0000-0000-0000-000000000008"), "Free", "Practise Java through focused coding exercises.", "Beginner", "Self-paced", true, "Exercism", "Java practice exercises", "Practice", new Guid("20000000-0000-0000-0000-000000000004"), "Curated", "https://exercism.org/tracks/java" },
                    { new Guid("40000000-0000-0000-0000-000000000009"), "Free", "Learn Python syntax, data structures and modules.", "Beginner", "6 hours", true, "Python", "The Python tutorial", "Tutorial", new Guid("20000000-0000-0000-0000-000000000005"), "Curated", "https://docs.python.org/3/tutorial/" },
                    { new Guid("40000000-0000-0000-0000-000000000010"), "Free", "Practise Python with progressive coding exercises.", "Beginner", "Self-paced", true, "Exercism", "Python practice exercises", "Practice", new Guid("20000000-0000-0000-0000-000000000005"), "Curated", "https://exercism.org/tracks/python" },
                    { new Guid("40000000-0000-0000-0000-000000000011"), "Free", "Learn PostgreSQL concepts, SQL and database features.", "Beginner", "5 hours", true, "PostgreSQL", "PostgreSQL tutorial", "Tutorial", new Guid("20000000-0000-0000-0000-000000000006"), "Curated", "https://www.postgresql.org/docs/current/tutorial.html" },
                    { new Guid("40000000-0000-0000-0000-000000000012"), "Free", "Solve hands-on PostgreSQL query exercises.", "Intermediate", "4 hours", true, "PGExercises", "PostgreSQL exercises", "Practice", new Guid("20000000-0000-0000-0000-000000000006"), "Curated", "https://pgexercises.com/" },
                    { new Guid("40000000-0000-0000-0000-000000000013"), "Free", "Learn components, state, events and data flow.", "Beginner", "5 hours", true, "React", "Learn React", "Tutorial", new Guid("20000000-0000-0000-0000-000000000007"), "Curated", "https://react.dev/learn" },
                    { new Guid("40000000-0000-0000-0000-000000000014"), "Free", "Build an interactive React application.", "Beginner", "2 hours", true, "React", "React Tic-Tac-Toe tutorial", "Project", new Guid("20000000-0000-0000-0000-000000000007"), "Curated", "https://react.dev/learn/tutorial-tic-tac-toe" },
                    { new Guid("40000000-0000-0000-0000-000000000015"), "Free", "Learn semantic and accessible HTML structure.", "Beginner", "5 hours", true, "MDN", "Structuring content with HTML", "Learning Path", new Guid("20000000-0000-0000-0000-000000000008"), "Curated", "https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/Structuring_content" },
                    { new Guid("40000000-0000-0000-0000-000000000016"), "Free", "Test core HTML text and structure skills.", "Beginner", "1 hour", true, "MDN", "HTML text basics challenge", "Practice", new Guid("20000000-0000-0000-0000-000000000008"), "Curated", "https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/Structuring_content/Test_your_skills/HTML_text_basics" },
                    { new Guid("40000000-0000-0000-0000-000000000017"), "Free", "Learn selectors, the box model and common styling techniques.", "Beginner", "6 hours", true, "MDN", "CSS styling basics", "Learning Path", new Guid("20000000-0000-0000-0000-000000000009"), "Curated", "https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/Styling_basics" },
                    { new Guid("40000000-0000-0000-0000-000000000018"), "Free", "Practise responsive layouts with Flexbox and Grid.", "Intermediate", "5 hours", true, "MDN", "CSS layout", "Tutorial", new Guid("20000000-0000-0000-0000-000000000009"), "Curated", "https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/CSS_layout" },
                    { new Guid("40000000-0000-0000-0000-000000000019"), "Free", "Learn JavaScript syntax, functions, objects and asynchronous code.", "Beginner", "8 hours", true, "MDN", "JavaScript guide", "Guide", new Guid("20000000-0000-0000-0000-000000000010"), "Curated", "https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide" },
                    { new Guid("40000000-0000-0000-0000-000000000020"), "Free", "Learn and practise modern JavaScript by topic.", "Beginner", "Self-paced", true, "JavaScript.info", "JavaScript exercises", "Practice", new Guid("20000000-0000-0000-0000-000000000010"), "Curated", "https://javascript.info/" },
                    { new Guid("40000000-0000-0000-0000-000000000021"), "Free", "Learn TypeScript's type system and everyday patterns.", "Beginner", "5 hours", true, "TypeScript", "TypeScript handbook", "Guide", new Guid("20000000-0000-0000-0000-000000000011"), "Curated", "https://www.typescriptlang.org/docs/handbook/intro.html" },
                    { new Guid("40000000-0000-0000-0000-000000000022"), "Free", "Experiment with types and compiler feedback in the browser.", "Beginner", "Self-paced", true, "TypeScript", "TypeScript playground", "Practice", new Guid("20000000-0000-0000-0000-000000000011"), "Curated", "https://www.typescriptlang.org/play" },
                    { new Guid("40000000-0000-0000-0000-000000000023"), "Free", "Learn Git fundamentals, branching and collaboration.", "Beginner", "8 hours", true, "Git", "Pro Git book", "Book", new Guid("20000000-0000-0000-0000-000000000012"), "Curated", "https://git-scm.com/book/en/v2" },
                    { new Guid("40000000-0000-0000-0000-000000000024"), "Free", "Practise repositories, branches, commits and pull requests.", "Beginner", "1 hour", true, "GitHub Skills", "Introduction to GitHub", "Project", new Guid("20000000-0000-0000-0000-000000000012"), "Curated", "https://github.com/skills/introduction-to-github" },
                    { new Guid("40000000-0000-0000-0000-000000000025"), "Free", "Create and run a first .NET application.", "Beginner", "2 hours", true, "Microsoft Learn", "Get started with .NET", "Tutorial", new Guid("20000000-0000-0000-0000-000000000013"), "Curated", "https://learn.microsoft.com/en-us/dotnet/core/get-started" },
                    { new Guid("40000000-0000-0000-0000-000000000026"), "Free", "Build practical applications on the .NET platform.", "Intermediate", "6 hours", true, "Microsoft Learn", "Build .NET applications with C#", "Learning Path", new Guid("20000000-0000-0000-0000-000000000013"), "Curated", "https://learn.microsoft.com/en-us/training/paths/build-dotnet-applications-csharp/" },
                    { new Guid("40000000-0000-0000-0000-000000000027"), "Free", "Learn the Node.js runtime and server-side JavaScript basics.", "Beginner", "2 hours", true, "Node.js", "Introduction to Node.js", "Guide", new Guid("20000000-0000-0000-0000-000000000014"), "Curated", "https://nodejs.org/learn/getting-started/introduction-to-nodejs" },
                    { new Guid("40000000-0000-0000-0000-000000000028"), "Free", "Write and run automated tests using the built-in test runner.", "Intermediate", "2 hours", true, "Node.js", "Node.js test runner", "Practice", new Guid("20000000-0000-0000-0000-000000000014"), "Curated", "https://nodejs.org/api/test.html" },
                    { new Guid("40000000-0000-0000-0000-000000000029"), "Free", "Learn images, containers and Docker fundamentals.", "Beginner", "3 hours", true, "Docker", "Get started with Docker", "Tutorial", new Guid("20000000-0000-0000-0000-000000000015"), "Curated", "https://docs.docker.com/get-started/" },
                    { new Guid("40000000-0000-0000-0000-000000000030"), "Free", "Containerise and update a sample application.", "Beginner", "3 hours", true, "Docker", "Docker workshop", "Project", new Guid("20000000-0000-0000-0000-000000000015"), "Curated", "https://docs.docker.com/get-started/workshop/" },
                    { new Guid("40000000-0000-0000-0000-000000000031"), "Free", "Learn foundational cloud and Azure concepts.", "Beginner", "1 hour", true, "Microsoft Learn", "Azure fundamentals", "Learning Path", new Guid("20000000-0000-0000-0000-000000000016"), "Curated", "https://learn.microsoft.com/en-us/training/paths/microsoft-azure-fundamentals-describe-cloud-concepts/" },
                    { new Guid("40000000-0000-0000-0000-000000000032"), "Free", "Choose hands-on Azure modules for your target role.", "Beginner", "Self-paced", true, "Microsoft Learn", "Azure training", "Practice", new Guid("20000000-0000-0000-0000-000000000016"), "Curated", "https://learn.microsoft.com/en-us/training/azure/" },
                    { new Guid("40000000-0000-0000-0000-000000000033"), "Free", "Create, run and maintain automated .NET unit tests.", "Intermediate", "2 hours", true, "Microsoft Learn", "Unit testing with dotnet test", "Tutorial", new Guid("20000000-0000-0000-0000-000000000017"), "Curated", "https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test" },
                    { new Guid("40000000-0000-0000-0000-000000000034"), "Free", "Write JavaScript and TypeScript unit tests.", "Beginner", "2 hours", true, "Jest", "Jest getting started", "Tutorial", new Guid("20000000-0000-0000-0000-000000000017"), "Curated", "https://jestjs.io/docs/getting-started" },
                    { new Guid("40000000-0000-0000-0000-000000000035"), "Free", "Learn Linux commands, permissions, processes and networking.", "Beginner", "8 hours", true, "Linux Journey", "Linux Journey", "Learning Path", new Guid("20000000-0000-0000-0000-000000000018"), "Curated", "https://linuxjourney.com/" },
                    { new Guid("40000000-0000-0000-0000-000000000036"), "Free", "Practise Linux command-line problem solving.", "Beginner", "4 hours", true, "OverTheWire", "Bandit command-line challenges", "Practice", new Guid("20000000-0000-0000-0000-000000000018"), "Curated", "https://overthewire.org/wargames/bandit/" },
                    { new Guid("40000000-0000-0000-0000-000000000037"), "Free", "Create a workflow that builds and tests code automatically.", "Beginner", "1 hour", true, "GitHub Docs", "GitHub Actions quickstart", "Tutorial", new Guid("20000000-0000-0000-0000-000000000019"), "Curated", "https://docs.github.com/en/actions/get-started/quickstart" },
                    { new Guid("40000000-0000-0000-0000-000000000038"), "Free", "Build a working CI workflow in a practice repository.", "Beginner", "1 hour", true, "GitHub Skills", "Hello GitHub Actions", "Project", new Guid("20000000-0000-0000-0000-000000000019"), "Curated", "https://github.com/skills/hello-github-actions" },
                    { new Guid("40000000-0000-0000-0000-000000000039"), "Free", "Learn formulas, tables, charts and data analysis features.", "Beginner", "5 hours", true, "Microsoft", "Excel help and learning", "Learning Path", new Guid("20000000-0000-0000-0000-000000000020"), "Curated", "https://support.microsoft.com/en-us/excel" },
                    { new Guid("40000000-0000-0000-0000-000000000040"), "Free", "Practise common formulas with guided examples and a tutorial workbook.", "Beginner", "3 hours", true, "Microsoft", "Excel formulas tutorial", "Practice", new Guid("20000000-0000-0000-0000-000000000020"), "Curated", "https://support.microsoft.com/en-us/office/overview-of-formulas-in-excel-ecfdc708-9162-49e8-b993-c311f47ca173" },
                    { new Guid("40000000-0000-0000-0000-000000000041"), "Free", "Learn to connect, model, visualise and share data.", "Beginner", "8 hours", true, "Microsoft Learn", "Microsoft Power BI training", "Learning Path", new Guid("20000000-0000-0000-0000-000000000021"), "Curated", "https://learn.microsoft.com/en-us/training/powerplatform/power-bi" },
                    { new Guid("40000000-0000-0000-0000-000000000042"), "Free", "Build a report from an Excel workbook.", "Beginner", "2 hours", true, "Microsoft Learn", "Create a Power BI report", "Project", new Guid("20000000-0000-0000-0000-000000000021"), "Curated", "https://learn.microsoft.com/en-us/power-bi/create-reports/desktop-excel-stunning-report" },
                    { new Guid("40000000-0000-0000-0000-000000000043"), "Free", "Choose visuals and build clear interactive reports.", "Beginner", "5 hours", true, "Microsoft Learn", "Data visualisation in Power BI", "Learning Path", new Guid("20000000-0000-0000-0000-000000000022"), "Curated", "https://learn.microsoft.com/en-us/training/paths/visualize-data-power-bi/" },
                    { new Guid("40000000-0000-0000-0000-000000000044"), "Free", "Practise visual analytics with guided training videos.", "Beginner", "Self-paced", true, "Tableau", "Tableau free training", "Practice", new Guid("20000000-0000-0000-0000-000000000022"), "Curated", "https://www.tableau.com/learn/training" }
                });

            migrationBuilder.InsertData(
                table: "job_skills",
                columns: new[] { "job_skill_id", "importance_level", "job_id", "requirement_type", "skill_id" },
                values: new object[,]
                {
                    { new Guid("60000000-0000-0000-0000-000000000027"), 5, new Guid("50000000-0000-0000-0000-000000000003"), "Required", new Guid("20000000-0000-0000-0000-000000000008") },
                    { new Guid("60000000-0000-0000-0000-000000000028"), 5, new Guid("50000000-0000-0000-0000-000000000003"), "Required", new Guid("20000000-0000-0000-0000-000000000009") },
                    { new Guid("60000000-0000-0000-0000-000000000029"), 5, new Guid("50000000-0000-0000-0000-000000000003"), "Required", new Guid("20000000-0000-0000-0000-000000000010") },
                    { new Guid("60000000-0000-0000-0000-000000000030"), 4, new Guid("50000000-0000-0000-0000-000000000003"), "Preferred", new Guid("20000000-0000-0000-0000-000000000011") },
                    { new Guid("60000000-0000-0000-0000-000000000031"), 5, new Guid("50000000-0000-0000-0000-000000000003"), "Required", new Guid("20000000-0000-0000-0000-000000000007") },
                    { new Guid("60000000-0000-0000-0000-000000000032"), 4, new Guid("50000000-0000-0000-0000-000000000003"), "Preferred", new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("60000000-0000-0000-0000-000000000033"), 4, new Guid("50000000-0000-0000-0000-000000000003"), "Preferred", new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("60000000-0000-0000-0000-000000000034"), 4, new Guid("50000000-0000-0000-0000-000000000003"), "Preferred", new Guid("20000000-0000-0000-0000-000000000017") },
                    { new Guid("60000000-0000-0000-0000-000000000035"), 5, new Guid("50000000-0000-0000-0000-000000000004"), "Required", new Guid("20000000-0000-0000-0000-000000000008") },
                    { new Guid("60000000-0000-0000-0000-000000000036"), 5, new Guid("50000000-0000-0000-0000-000000000004"), "Required", new Guid("20000000-0000-0000-0000-000000000009") },
                    { new Guid("60000000-0000-0000-0000-000000000037"), 5, new Guid("50000000-0000-0000-0000-000000000004"), "Required", new Guid("20000000-0000-0000-0000-000000000010") },
                    { new Guid("60000000-0000-0000-0000-000000000038"), 4, new Guid("50000000-0000-0000-0000-000000000004"), "Preferred", new Guid("20000000-0000-0000-0000-000000000011") },
                    { new Guid("60000000-0000-0000-0000-000000000039"), 5, new Guid("50000000-0000-0000-0000-000000000004"), "Required", new Guid("20000000-0000-0000-0000-000000000007") },
                    { new Guid("60000000-0000-0000-0000-000000000040"), 4, new Guid("50000000-0000-0000-0000-000000000004"), "Preferred", new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("60000000-0000-0000-0000-000000000041"), 4, new Guid("50000000-0000-0000-0000-000000000004"), "Preferred", new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("60000000-0000-0000-0000-000000000042"), 4, new Guid("50000000-0000-0000-0000-000000000004"), "Preferred", new Guid("20000000-0000-0000-0000-000000000017") },
                    { new Guid("60000000-0000-0000-0000-000000000043"), 5, new Guid("50000000-0000-0000-0000-000000000005"), "Required", new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("60000000-0000-0000-0000-000000000044"), 4, new Guid("50000000-0000-0000-0000-000000000005"), "Preferred", new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("60000000-0000-0000-0000-000000000045"), 4, new Guid("50000000-0000-0000-0000-000000000005"), "Preferred", new Guid("20000000-0000-0000-0000-000000000005") },
                    { new Guid("60000000-0000-0000-0000-000000000046"), 5, new Guid("50000000-0000-0000-0000-000000000005"), "Required", new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("60000000-0000-0000-0000-000000000047"), 5, new Guid("50000000-0000-0000-0000-000000000005"), "Required", new Guid("20000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000048"), 5, new Guid("50000000-0000-0000-0000-000000000005"), "Required", new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("60000000-0000-0000-0000-000000000049"), 5, new Guid("50000000-0000-0000-0000-000000000005"), "Preferred", new Guid("20000000-0000-0000-0000-000000000013") },
                    { new Guid("60000000-0000-0000-0000-000000000050"), 4, new Guid("50000000-0000-0000-0000-000000000005"), "Preferred", new Guid("20000000-0000-0000-0000-000000000014") },
                    { new Guid("60000000-0000-0000-0000-000000000051"), 4, new Guid("50000000-0000-0000-0000-000000000005"), "Preferred", new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("60000000-0000-0000-0000-000000000052"), 3, new Guid("50000000-0000-0000-0000-000000000005"), "Preferred", new Guid("20000000-0000-0000-0000-000000000015") },
                    { new Guid("60000000-0000-0000-0000-000000000053"), 4, new Guid("50000000-0000-0000-0000-000000000005"), "Preferred", new Guid("20000000-0000-0000-0000-000000000017") },
                    { new Guid("60000000-0000-0000-0000-000000000054"), 5, new Guid("50000000-0000-0000-0000-000000000006"), "Required", new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("60000000-0000-0000-0000-000000000055"), 4, new Guid("50000000-0000-0000-0000-000000000006"), "Preferred", new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("60000000-0000-0000-0000-000000000056"), 4, new Guid("50000000-0000-0000-0000-000000000006"), "Preferred", new Guid("20000000-0000-0000-0000-000000000005") },
                    { new Guid("60000000-0000-0000-0000-000000000057"), 5, new Guid("50000000-0000-0000-0000-000000000006"), "Required", new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("60000000-0000-0000-0000-000000000058"), 5, new Guid("50000000-0000-0000-0000-000000000006"), "Required", new Guid("20000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000059"), 5, new Guid("50000000-0000-0000-0000-000000000006"), "Required", new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("60000000-0000-0000-0000-000000000060"), 5, new Guid("50000000-0000-0000-0000-000000000006"), "Preferred", new Guid("20000000-0000-0000-0000-000000000013") },
                    { new Guid("60000000-0000-0000-0000-000000000061"), 4, new Guid("50000000-0000-0000-0000-000000000006"), "Preferred", new Guid("20000000-0000-0000-0000-000000000014") },
                    { new Guid("60000000-0000-0000-0000-000000000062"), 4, new Guid("50000000-0000-0000-0000-000000000006"), "Preferred", new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("60000000-0000-0000-0000-000000000063"), 3, new Guid("50000000-0000-0000-0000-000000000006"), "Preferred", new Guid("20000000-0000-0000-0000-000000000015") },
                    { new Guid("60000000-0000-0000-0000-000000000064"), 4, new Guid("50000000-0000-0000-0000-000000000006"), "Preferred", new Guid("20000000-0000-0000-0000-000000000017") },
                    { new Guid("60000000-0000-0000-0000-000000000065"), 4, new Guid("50000000-0000-0000-0000-000000000007"), "Preferred", new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("60000000-0000-0000-0000-000000000066"), 5, new Guid("50000000-0000-0000-0000-000000000007"), "Required", new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("60000000-0000-0000-0000-000000000067"), 5, new Guid("50000000-0000-0000-0000-000000000007"), "Required", new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("60000000-0000-0000-0000-000000000068"), 3, new Guid("50000000-0000-0000-0000-000000000007"), "Preferred", new Guid("20000000-0000-0000-0000-000000000005") },
                    { new Guid("60000000-0000-0000-0000-000000000069"), 5, new Guid("50000000-0000-0000-0000-000000000007"), "Preferred", new Guid("20000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000070"), 5, new Guid("50000000-0000-0000-0000-000000000007"), "Required", new Guid("20000000-0000-0000-0000-000000000007") },
                    { new Guid("60000000-0000-0000-0000-000000000071"), 5, new Guid("50000000-0000-0000-0000-000000000007"), "Required", new Guid("20000000-0000-0000-0000-000000000008") },
                    { new Guid("60000000-0000-0000-0000-000000000072"), 5, new Guid("50000000-0000-0000-0000-000000000007"), "Required", new Guid("20000000-0000-0000-0000-000000000009") },
                    { new Guid("60000000-0000-0000-0000-000000000073"), 5, new Guid("50000000-0000-0000-0000-000000000007"), "Required", new Guid("20000000-0000-0000-0000-000000000010") },
                    { new Guid("60000000-0000-0000-0000-000000000074"), 4, new Guid("50000000-0000-0000-0000-000000000007"), "Preferred", new Guid("20000000-0000-0000-0000-000000000011") },
                    { new Guid("60000000-0000-0000-0000-000000000075"), 4, new Guid("50000000-0000-0000-0000-000000000007"), "Preferred", new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("60000000-0000-0000-0000-000000000076"), 4, new Guid("50000000-0000-0000-0000-000000000007"), "Preferred", new Guid("20000000-0000-0000-0000-000000000013") },
                    { new Guid("60000000-0000-0000-0000-000000000077"), 4, new Guid("50000000-0000-0000-0000-000000000007"), "Preferred", new Guid("20000000-0000-0000-0000-000000000014") },
                    { new Guid("60000000-0000-0000-0000-000000000078"), 3, new Guid("50000000-0000-0000-0000-000000000007"), "Preferred", new Guid("20000000-0000-0000-0000-000000000015") },
                    { new Guid("60000000-0000-0000-0000-000000000079"), 4, new Guid("50000000-0000-0000-0000-000000000007"), "Preferred", new Guid("20000000-0000-0000-0000-000000000017") },
                    { new Guid("60000000-0000-0000-0000-000000000080"), 4, new Guid("50000000-0000-0000-0000-000000000008"), "Preferred", new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("60000000-0000-0000-0000-000000000081"), 5, new Guid("50000000-0000-0000-0000-000000000008"), "Required", new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("60000000-0000-0000-0000-000000000082"), 5, new Guid("50000000-0000-0000-0000-000000000008"), "Required", new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("60000000-0000-0000-0000-000000000083"), 3, new Guid("50000000-0000-0000-0000-000000000008"), "Preferred", new Guid("20000000-0000-0000-0000-000000000005") },
                    { new Guid("60000000-0000-0000-0000-000000000084"), 5, new Guid("50000000-0000-0000-0000-000000000008"), "Preferred", new Guid("20000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000085"), 5, new Guid("50000000-0000-0000-0000-000000000008"), "Required", new Guid("20000000-0000-0000-0000-000000000007") },
                    { new Guid("60000000-0000-0000-0000-000000000086"), 5, new Guid("50000000-0000-0000-0000-000000000008"), "Required", new Guid("20000000-0000-0000-0000-000000000008") },
                    { new Guid("60000000-0000-0000-0000-000000000087"), 5, new Guid("50000000-0000-0000-0000-000000000008"), "Required", new Guid("20000000-0000-0000-0000-000000000009") },
                    { new Guid("60000000-0000-0000-0000-000000000088"), 5, new Guid("50000000-0000-0000-0000-000000000008"), "Required", new Guid("20000000-0000-0000-0000-000000000010") },
                    { new Guid("60000000-0000-0000-0000-000000000089"), 4, new Guid("50000000-0000-0000-0000-000000000008"), "Preferred", new Guid("20000000-0000-0000-0000-000000000011") },
                    { new Guid("60000000-0000-0000-0000-000000000090"), 4, new Guid("50000000-0000-0000-0000-000000000008"), "Preferred", new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("60000000-0000-0000-0000-000000000091"), 4, new Guid("50000000-0000-0000-0000-000000000008"), "Preferred", new Guid("20000000-0000-0000-0000-000000000013") },
                    { new Guid("60000000-0000-0000-0000-000000000092"), 4, new Guid("50000000-0000-0000-0000-000000000008"), "Preferred", new Guid("20000000-0000-0000-0000-000000000014") },
                    { new Guid("60000000-0000-0000-0000-000000000093"), 3, new Guid("50000000-0000-0000-0000-000000000008"), "Preferred", new Guid("20000000-0000-0000-0000-000000000015") },
                    { new Guid("60000000-0000-0000-0000-000000000094"), 4, new Guid("50000000-0000-0000-0000-000000000008"), "Preferred", new Guid("20000000-0000-0000-0000-000000000017") },
                    { new Guid("60000000-0000-0000-0000-000000000095"), 5, new Guid("50000000-0000-0000-0000-000000000009"), "Required", new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("60000000-0000-0000-0000-000000000096"), 5, new Guid("50000000-0000-0000-0000-000000000009"), "Required", new Guid("20000000-0000-0000-0000-000000000005") },
                    { new Guid("60000000-0000-0000-0000-000000000097"), 4, new Guid("50000000-0000-0000-0000-000000000009"), "Preferred", new Guid("20000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000098"), 5, new Guid("50000000-0000-0000-0000-000000000009"), "Required", new Guid("20000000-0000-0000-0000-000000000020") },
                    { new Guid("60000000-0000-0000-0000-000000000099"), 5, new Guid("50000000-0000-0000-0000-000000000009"), "Required", new Guid("20000000-0000-0000-0000-000000000021") },
                    { new Guid("60000000-0000-0000-0000-000000000100"), 5, new Guid("50000000-0000-0000-0000-000000000009"), "Required", new Guid("20000000-0000-0000-0000-000000000022") },
                    { new Guid("60000000-0000-0000-0000-000000000101"), 2, new Guid("50000000-0000-0000-0000-000000000009"), "Preferred", new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("60000000-0000-0000-0000-000000000102"), 5, new Guid("50000000-0000-0000-0000-000000000010"), "Required", new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("60000000-0000-0000-0000-000000000103"), 5, new Guid("50000000-0000-0000-0000-000000000010"), "Required", new Guid("20000000-0000-0000-0000-000000000005") },
                    { new Guid("60000000-0000-0000-0000-000000000104"), 4, new Guid("50000000-0000-0000-0000-000000000010"), "Preferred", new Guid("20000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000105"), 5, new Guid("50000000-0000-0000-0000-000000000010"), "Required", new Guid("20000000-0000-0000-0000-000000000020") },
                    { new Guid("60000000-0000-0000-0000-000000000106"), 5, new Guid("50000000-0000-0000-0000-000000000010"), "Required", new Guid("20000000-0000-0000-0000-000000000021") },
                    { new Guid("60000000-0000-0000-0000-000000000107"), 5, new Guid("50000000-0000-0000-0000-000000000010"), "Required", new Guid("20000000-0000-0000-0000-000000000022") },
                    { new Guid("60000000-0000-0000-0000-000000000108"), 2, new Guid("50000000-0000-0000-0000-000000000010"), "Preferred", new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("60000000-0000-0000-0000-000000000109"), 4, new Guid("50000000-0000-0000-0000-000000000011"), "Preferred", new Guid("20000000-0000-0000-0000-000000000005") },
                    { new Guid("60000000-0000-0000-0000-000000000110"), 3, new Guid("50000000-0000-0000-0000-000000000011"), "Preferred", new Guid("20000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000111"), 5, new Guid("50000000-0000-0000-0000-000000000011"), "Required", new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("60000000-0000-0000-0000-000000000112"), 5, new Guid("50000000-0000-0000-0000-000000000011"), "Required", new Guid("20000000-0000-0000-0000-000000000015") },
                    { new Guid("60000000-0000-0000-0000-000000000113"), 5, new Guid("50000000-0000-0000-0000-000000000011"), "Required", new Guid("20000000-0000-0000-0000-000000000016") },
                    { new Guid("60000000-0000-0000-0000-000000000114"), 3, new Guid("50000000-0000-0000-0000-000000000011"), "Preferred", new Guid("20000000-0000-0000-0000-000000000017") },
                    { new Guid("60000000-0000-0000-0000-000000000115"), 5, new Guid("50000000-0000-0000-0000-000000000011"), "Required", new Guid("20000000-0000-0000-0000-000000000018") },
                    { new Guid("60000000-0000-0000-0000-000000000116"), 5, new Guid("50000000-0000-0000-0000-000000000011"), "Required", new Guid("20000000-0000-0000-0000-000000000019") },
                    { new Guid("60000000-0000-0000-0000-000000000117"), 4, new Guid("50000000-0000-0000-0000-000000000012"), "Preferred", new Guid("20000000-0000-0000-0000-000000000005") },
                    { new Guid("60000000-0000-0000-0000-000000000118"), 3, new Guid("50000000-0000-0000-0000-000000000012"), "Preferred", new Guid("20000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000119"), 5, new Guid("50000000-0000-0000-0000-000000000012"), "Required", new Guid("20000000-0000-0000-0000-000000000012") },
                    { new Guid("60000000-0000-0000-0000-000000000120"), 5, new Guid("50000000-0000-0000-0000-000000000012"), "Required", new Guid("20000000-0000-0000-0000-000000000015") },
                    { new Guid("60000000-0000-0000-0000-000000000121"), 5, new Guid("50000000-0000-0000-0000-000000000012"), "Required", new Guid("20000000-0000-0000-0000-000000000016") },
                    { new Guid("60000000-0000-0000-0000-000000000122"), 3, new Guid("50000000-0000-0000-0000-000000000012"), "Preferred", new Guid("20000000-0000-0000-0000-000000000017") },
                    { new Guid("60000000-0000-0000-0000-000000000123"), 5, new Guid("50000000-0000-0000-0000-000000000012"), "Required", new Guid("20000000-0000-0000-0000-000000000018") },
                    { new Guid("60000000-0000-0000-0000-000000000124"), 5, new Guid("50000000-0000-0000-0000-000000000012"), "Required", new Guid("20000000-0000-0000-0000-000000000019") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000023"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000024"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000025"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000026"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000027"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000028"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000029"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000030"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000031"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000032"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000033"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000034"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000035"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000036"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000037"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000038"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000039"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000040"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000041"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000042"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000043"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000044"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000045"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000046"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000047"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000048"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000049"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000050"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000051"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000052"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000053"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000054"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000055"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000056"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000057"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000058"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000059"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000060"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000061"));

            migrationBuilder.DeleteData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000062"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000023"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000024"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000025"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000026"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000027"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000028"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000029"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000030"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000031"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000032"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000033"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000034"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000035"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000036"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000037"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000038"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000039"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000040"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000041"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000042"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000043"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000044"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000045"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000046"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000047"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000048"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000049"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000050"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000051"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000052"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000053"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000054"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000055"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000056"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000057"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000058"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000059"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000060"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000061"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000062"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000063"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000064"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000065"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000066"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000067"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000068"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000069"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000070"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000071"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000072"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000073"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000074"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000075"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000076"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000077"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000078"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000079"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000080"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000081"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000082"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000083"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000084"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000085"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000086"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000087"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000088"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000089"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000090"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000091"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000092"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000093"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000094"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000095"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000096"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000097"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000098"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000099"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000100"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000101"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000102"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000103"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000104"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000105"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000106"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000107"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000108"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000109"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000110"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000111"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000112"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000113"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000114"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000115"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000116"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000117"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000118"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000119"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000120"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000121"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000122"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000123"));

            migrationBuilder.DeleteData(
                table: "job_skills",
                keyColumn: "job_skill_id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000124"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000023"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000024"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000025"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000026"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000027"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000028"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000029"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000030"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000031"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000032"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000033"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000034"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000035"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000036"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000037"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000038"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000039"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000040"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000041"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000042"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000043"));

            migrationBuilder.DeleteData(
                table: "learning_resources",
                keyColumn: "resource_id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000044"));

            migrationBuilder.DeleteData(
                table: "jobs",
                keyColumn: "job_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "jobs",
                keyColumn: "job_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "jobs",
                keyColumn: "job_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "jobs",
                keyColumn: "job_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "jobs",
                keyColumn: "job_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "jobs",
                keyColumn: "job_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "jobs",
                keyColumn: "job_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "jobs",
                keyColumn: "job_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "jobs",
                keyColumn: "job_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "jobs",
                keyColumn: "job_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "jobs",
                keyColumn: "job_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "jobs",
                keyColumn: "job_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                table: "careers",
                keyColumn: "career_id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "careers",
                keyColumn: "career_id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "careers",
                keyColumn: "career_id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "careers",
                keyColumn: "career_id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "careers",
                keyColumn: "career_id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"));

            migrationBuilder.UpdateData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                column: "importance_level",
                value: 4);

            migrationBuilder.UpdateData(
                table: "career_skills",
                keyColumn: "career_skill_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                column: "importance_level",
                value: 4);

            migrationBuilder.UpdateData(
                table: "careers",
                keyColumn: "career_id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "description", "education_requirements", "industry" },
                values: new object[] { "Designs, develops and maintains software applications.", "Relevant qualification or equivalent practical experience.", "Information Technology" });

            migrationBuilder.UpdateData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                columns: new[] { "category", "description" },
                values: new object[] { "Technical", "Programming using the C# language." });

            migrationBuilder.UpdateData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                columns: new[] { "category", "description" },
                values: new object[] { "Technical", "Querying and managing relational databases." });

            migrationBuilder.UpdateData(
                table: "skills",
                keyColumn: "skill_id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"),
                columns: new[] { "category", "description" },
                values: new object[] { "Technical", "Designing and consuming RESTful APIs." });
        }
    }
}
