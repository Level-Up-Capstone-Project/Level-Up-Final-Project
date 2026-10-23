using CapstoneProjectBE.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Data;

/// <summary>
/// A deterministic starter catalogue used while automated job and resource ingestion is unavailable.
/// Job rows deliberately link to live searches instead of pretending that a seeded vacancy is current.
/// </summary>
internal static class MvpCatalogueSeed
{
    private const string Education = "Relevant qualification, portfolio work, or equivalent practical experience.";
    private static readonly DateTime SeededAt = new(2026, 10, 23, 0, 0, 0, DateTimeKind.Utc);

    internal static void Configure(ModelBuilder modelBuilder)
    {
        var careers = CreateCareers();
        var skills = CreateSkills();
        var careerSkills = CreateCareerSkills();
        var resources = CreateResources();
        var jobs = CreateJobs();
        var jobSkills = CreateJobSkills(jobs, careerSkills);

        modelBuilder.Entity<Career>().HasData(careers);
        modelBuilder.Entity<Skill>().HasData(skills);
        modelBuilder.Entity<CareerSkill>().HasData(careerSkills);
        modelBuilder.Entity<LearningResource>().HasData(resources);
        modelBuilder.Entity<Job>().HasData(jobs);
        modelBuilder.Entity<JobSkill>().HasData(jobSkills);
    }

    private static Career[] CreateCareers() =>
    [
        Career(1, "Software Developer", "Designs, builds, tests and maintains software applications.", "Software Engineering"),
        Career(2, "Frontend Developer", "Builds accessible, responsive and interactive web user interfaces.", "Web Development"),
        Career(3, "Backend Developer", "Builds secure APIs, business services and database-backed applications.", "Software Engineering"),
        Career(4, "Full Stack Developer", "Develops complete web products across browser, server and database layers.", "Web Development"),
        Career(5, "Data Analyst", "Transforms data into reliable analysis, dashboards and business insights.", "Data and Analytics"),
        Career(6, "DevOps Engineer", "Automates software delivery and operates reliable cloud infrastructure.", "Cloud and Infrastructure")
    ];

    private static Skill[] CreateSkills() =>
    [
        Skill(1, "C#", "Programming Language", "Object-oriented programming with C# and the .NET ecosystem."),
        Skill(2, "SQL", "Database", "Querying, joining and managing data in relational databases."),
        Skill(3, "REST APIs", "Web Development", "Designing, consuming and testing HTTP-based RESTful APIs."),
        Skill(4, "Java", "Programming Language", "Object-oriented application development with Java."),
        Skill(5, "Python", "Programming Language", "Programming and automation using Python."),
        Skill(6, "PostgreSQL", "Database", "Designing and operating PostgreSQL relational databases."),
        Skill(7, "React", "Frontend", "Building component-based web interfaces with React."),
        Skill(8, "HTML", "Frontend", "Structuring accessible web content with semantic HTML."),
        Skill(9, "CSS", "Frontend", "Styling responsive web interfaces with modern CSS."),
        Skill(10, "JavaScript", "Programming Language", "Programming interactive web applications with JavaScript."),
        Skill(11, "TypeScript", "Programming Language", "Building type-safe JavaScript applications with TypeScript."),
        Skill(12, "Git", "Developer Tools", "Using distributed version control for collaborative software development."),
        Skill(13, ".NET", "Backend", "Building applications and services on the .NET platform."),
        Skill(14, "Node.js", "Backend", "Building server-side JavaScript applications with Node.js."),
        Skill(15, "Docker", "DevOps", "Packaging and running applications in containers."),
        Skill(16, "Azure", "Cloud", "Building and operating workloads on Microsoft Azure."),
        Skill(17, "Testing", "Software Quality", "Designing automated tests that protect software behaviour."),
        Skill(18, "Linux", "Operating Systems", "Working confidently with Linux systems and command-line tools."),
        Skill(19, "CI/CD", "DevOps", "Automating builds, tests and deployments through delivery pipelines."),
        Skill(20, "Excel", "Data and Analytics", "Analysing and presenting data with Microsoft Excel."),
        Skill(21, "Power BI", "Data and Analytics", "Creating data models and interactive reports with Power BI."),
        Skill(22, "Data Visualization", "Data and Analytics", "Communicating patterns and insights through effective visualisations.")
    ];

    private static CareerSkill[] CreateCareerSkills()
    {
        var specs = new[]
        {
            // Software Developer: includes the technologies requested for the first MVP career path.
            CS(1, 1, 5, true), CS(1, 2, 5, true), CS(1, 3, 5, true), CS(1, 4, 3),
            CS(1, 5, 3), CS(1, 6, 4), CS(1, 7, 3), CS(1, 8, 3), CS(1, 9, 3),
            CS(1, 10, 4), CS(1, 12, 4, true), CS(1, 13, 4), CS(1, 17, 4, true),

            CS(2, 8, 5, true), CS(2, 9, 5, true), CS(2, 10, 5, true), CS(2, 11, 4),
            CS(2, 7, 5, true), CS(2, 3, 4), CS(2, 12, 4), CS(2, 17, 4),

            CS(3, 1, 5, true), CS(3, 4, 4), CS(3, 5, 4), CS(3, 2, 5, true),
            CS(3, 6, 5, true), CS(3, 3, 5, true), CS(3, 13, 5), CS(3, 14, 4),
            CS(3, 12, 4), CS(3, 15, 3), CS(3, 17, 4),

            CS(4, 1, 4), CS(4, 2, 5, true), CS(4, 3, 5, true), CS(4, 5, 3),
            CS(4, 6, 5), CS(4, 7, 5, true), CS(4, 8, 5, true), CS(4, 9, 5, true),
            CS(4, 10, 5, true), CS(4, 11, 4), CS(4, 12, 4), CS(4, 13, 4),
            CS(4, 14, 4), CS(4, 15, 3), CS(4, 17, 4),

            CS(5, 2, 5, true), CS(5, 5, 5, true), CS(5, 6, 4), CS(5, 20, 5, true),
            CS(5, 21, 5, true), CS(5, 22, 5, true), CS(5, 12, 2),

            CS(6, 5, 4), CS(6, 6, 3), CS(6, 12, 5, true), CS(6, 15, 5, true),
            CS(6, 16, 5, true), CS(6, 17, 3), CS(6, 18, 5, true), CS(6, 19, 5, true)
        };

        return specs.Select((spec, index) => new CareerSkill
        {
            CareerSkillID = Id(3, index + 1),
            CareerID = Id(1, spec.Career),
            SkillID = Id(2, spec.Skill),
            ImportanceLevel = spec.Importance,
            IsCoreSkill = spec.Core
        }).ToArray();
    }

    private static LearningResource[] CreateResources()
    {
        var specs = new[]
        {
            R(1, "Get started with C#", "Learning Path", "https://learn.microsoft.com/en-us/training/paths/get-started-c-sharp-part-1/", "Microsoft Learn", "Beginner", "4 hours", "Learn C# syntax and build small console programs."),
            R(1, "C# practice exercises", "Practice", "https://exercism.org/tracks/csharp", "Exercism", "Beginner", "Self-paced", "Practise C# with mentored coding exercises."),
            R(2, "PostgreSQL SQL tutorial", "Tutorial", "https://www.postgresql.org/docs/current/tutorial-sql.html", "PostgreSQL", "Beginner", "3 hours", "Learn relational querying directly from the PostgreSQL documentation."),
            R(2, "Interactive SQL lessons", "Practice", "https://sqlbolt.com/", "SQLBolt", "Beginner", "3 hours", "Complete browser-based SQL lessons and exercises."),
            R(3, "Overview of HTTP", "Guide", "https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Overview", "MDN", "Beginner", "1 hour", "Understand requests, responses, methods and status codes."),
            R(3, "Build a web API", "Project", "https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api", "Microsoft Learn", "Intermediate", "3 hours", "Build and test a REST API with ASP.NET Core."),
            R(4, "Learn Java", "Tutorial", "https://dev.java/learn/", "Dev.java", "Beginner", "6 hours", "Learn Java language basics and core APIs."),
            R(4, "Java practice exercises", "Practice", "https://exercism.org/tracks/java", "Exercism", "Beginner", "Self-paced", "Practise Java through focused coding exercises."),
            R(5, "The Python tutorial", "Tutorial", "https://docs.python.org/3/tutorial/", "Python", "Beginner", "6 hours", "Learn Python syntax, data structures and modules."),
            R(5, "Python practice exercises", "Practice", "https://exercism.org/tracks/python", "Exercism", "Beginner", "Self-paced", "Practise Python with progressive coding exercises."),
            R(6, "PostgreSQL tutorial", "Tutorial", "https://www.postgresql.org/docs/current/tutorial.html", "PostgreSQL", "Beginner", "5 hours", "Learn PostgreSQL concepts, SQL and database features."),
            R(6, "PostgreSQL exercises", "Practice", "https://pgexercises.com/", "PGExercises", "Intermediate", "4 hours", "Solve hands-on PostgreSQL query exercises."),
            R(7, "Learn React", "Tutorial", "https://react.dev/learn", "React", "Beginner", "5 hours", "Learn components, state, events and data flow."),
            R(7, "React Tic-Tac-Toe tutorial", "Project", "https://react.dev/learn/tutorial-tic-tac-toe", "React", "Beginner", "2 hours", "Build an interactive React application."),
            R(8, "Structuring content with HTML", "Learning Path", "https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/Structuring_content", "MDN", "Beginner", "5 hours", "Learn semantic and accessible HTML structure."),
            R(8, "HTML text basics challenge", "Practice", "https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/Structuring_content/Test_your_skills/HTML_text_basics", "MDN", "Beginner", "1 hour", "Test core HTML text and structure skills."),
            R(9, "CSS styling basics", "Learning Path", "https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/Styling_basics", "MDN", "Beginner", "6 hours", "Learn selectors, the box model and common styling techniques."),
            R(9, "CSS layout", "Tutorial", "https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/CSS_layout", "MDN", "Intermediate", "5 hours", "Practise responsive layouts with Flexbox and Grid."),
            R(10, "JavaScript guide", "Guide", "https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide", "MDN", "Beginner", "8 hours", "Learn JavaScript syntax, functions, objects and asynchronous code."),
            R(10, "JavaScript exercises", "Practice", "https://javascript.info/", "JavaScript.info", "Beginner", "Self-paced", "Learn and practise modern JavaScript by topic."),
            R(11, "TypeScript handbook", "Guide", "https://www.typescriptlang.org/docs/handbook/intro.html", "TypeScript", "Beginner", "5 hours", "Learn TypeScript's type system and everyday patterns."),
            R(11, "TypeScript playground", "Practice", "https://www.typescriptlang.org/play", "TypeScript", "Beginner", "Self-paced", "Experiment with types and compiler feedback in the browser."),
            R(12, "Pro Git book", "Book", "https://git-scm.com/book/en/v2", "Git", "Beginner", "8 hours", "Learn Git fundamentals, branching and collaboration."),
            R(12, "Introduction to GitHub", "Project", "https://github.com/skills/introduction-to-github", "GitHub Skills", "Beginner", "1 hour", "Practise repositories, branches, commits and pull requests."),
            R(13, "Get started with .NET", "Tutorial", "https://learn.microsoft.com/en-us/dotnet/core/get-started", "Microsoft Learn", "Beginner", "2 hours", "Create and run a first .NET application."),
            R(13, "Build .NET applications with C#", "Learning Path", "https://learn.microsoft.com/en-us/training/paths/build-dotnet-applications-csharp/", "Microsoft Learn", "Intermediate", "6 hours", "Build practical applications on the .NET platform."),
            R(14, "Introduction to Node.js", "Guide", "https://nodejs.org/learn/getting-started/introduction-to-nodejs", "Node.js", "Beginner", "2 hours", "Learn the Node.js runtime and server-side JavaScript basics."),
            R(14, "Node.js test runner", "Practice", "https://nodejs.org/api/test.html", "Node.js", "Intermediate", "2 hours", "Write and run automated tests using the built-in test runner."),
            R(15, "Get started with Docker", "Tutorial", "https://docs.docker.com/get-started/", "Docker", "Beginner", "3 hours", "Learn images, containers and Docker fundamentals."),
            R(15, "Docker workshop", "Project", "https://docs.docker.com/get-started/workshop/", "Docker", "Beginner", "3 hours", "Containerise and update a sample application."),
            R(16, "Azure fundamentals", "Learning Path", "https://learn.microsoft.com/en-us/training/paths/microsoft-azure-fundamentals-describe-cloud-concepts/", "Microsoft Learn", "Beginner", "1 hour", "Learn foundational cloud and Azure concepts."),
            R(16, "Azure training", "Practice", "https://learn.microsoft.com/en-us/training/azure/", "Microsoft Learn", "Beginner", "Self-paced", "Choose hands-on Azure modules for your target role."),
            R(17, "Unit testing with dotnet test", "Tutorial", "https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test", "Microsoft Learn", "Intermediate", "2 hours", "Create, run and maintain automated .NET unit tests."),
            R(17, "Jest getting started", "Tutorial", "https://jestjs.io/docs/getting-started", "Jest", "Beginner", "2 hours", "Write JavaScript and TypeScript unit tests."),
            R(18, "Linux Journey", "Learning Path", "https://linuxjourney.com/", "Linux Journey", "Beginner", "8 hours", "Learn Linux commands, permissions, processes and networking."),
            R(18, "Bandit command-line challenges", "Practice", "https://overthewire.org/wargames/bandit/", "OverTheWire", "Beginner", "4 hours", "Practise Linux command-line problem solving."),
            R(19, "GitHub Actions quickstart", "Tutorial", "https://docs.github.com/en/actions/get-started/quickstart", "GitHub Docs", "Beginner", "1 hour", "Create a workflow that builds and tests code automatically."),
            R(19, "Hello GitHub Actions", "Project", "https://github.com/skills/hello-github-actions", "GitHub Skills", "Beginner", "1 hour", "Build a working CI workflow in a practice repository."),
            R(20, "Excel help and learning", "Learning Path", "https://support.microsoft.com/en-us/excel", "Microsoft", "Beginner", "5 hours", "Learn formulas, tables, charts and data analysis features."),
            R(20, "Excel formulas tutorial", "Practice", "https://support.microsoft.com/en-us/office/overview-of-formulas-in-excel-ecfdc708-9162-49e8-b993-c311f47ca173", "Microsoft", "Beginner", "3 hours", "Practise common formulas with guided examples and a tutorial workbook."),
            R(21, "Microsoft Power BI training", "Learning Path", "https://learn.microsoft.com/en-us/training/powerplatform/power-bi", "Microsoft Learn", "Beginner", "8 hours", "Learn to connect, model, visualise and share data."),
            R(21, "Create a Power BI report", "Project", "https://learn.microsoft.com/en-us/power-bi/create-reports/desktop-excel-stunning-report", "Microsoft Learn", "Beginner", "2 hours", "Build a report from an Excel workbook."),
            R(22, "Data visualisation in Power BI", "Learning Path", "https://learn.microsoft.com/en-us/training/paths/visualize-data-power-bi/", "Microsoft Learn", "Beginner", "5 hours", "Choose visuals and build clear interactive reports."),
            R(22, "Tableau free training", "Practice", "https://www.tableau.com/learn/training", "Tableau", "Beginner", "Self-paced", "Practise visual analytics with guided training videos.")
        };

        return specs.Select((spec, index) => new LearningResource
        {
            ResourceID = Id(4, index + 1),
            SkillID = Id(2, spec.Skill),
            ResourceName = spec.Name,
            ResourceType = spec.Type,
            Url = spec.Url,
            Provider = spec.Provider,
            DifficultyLevel = spec.Level,
            CostType = "Free",
            EstDuration = spec.Duration,
            Description = spec.Description,
            SourceType = "Curated",
            IsActive = true
        }).ToArray();
    }

    private static Job[] CreateJobs()
    {
        var specs = new[]
        {
            J(1, "Software Developer", "software+developer"),
            J(2, "Frontend Developer", "frontend+developer"),
            J(3, "Backend Developer", "backend+developer"),
            J(4, "Full Stack Developer", "full+stack+developer"),
            J(5, "Data Analyst", "data+analyst"),
            J(6, "DevOps Engineer", "devops+engineer")
        };

        var jobs = new List<Job>();
        var index = 1;
        foreach (var spec in specs)
        {
            jobs.Add(Job(index++, spec.Career, spec.Title, "LinkedIn Jobs",
                $"https://www.linkedin.com/jobs/search/?keywords={spec.Search.Replace("+", "%20")}&location=South%20Africa", "linkedin"));
            jobs.Add(Job(index++, spec.Career, spec.Title, "Indeed South Africa",
                $"https://za.indeed.com/jobs?q={spec.Search}", "indeed"));
        }

        return jobs.ToArray();
    }

    private static JobSkill[] CreateJobSkills(IReadOnlyList<Job> jobs, IReadOnlyList<CareerSkill> careerSkills)
    {
        var links = new List<JobSkill>();
        var index = 1;
        foreach (var job in jobs)
        {
            foreach (var skill in careerSkills.Where(item => item.CareerID == job.MappedCareerID))
            {
                links.Add(new JobSkill
                {
                    JobSkillID = Id(6, index++),
                    JobID = job.JobID,
                    SkillID = skill.SkillID,
                    RequirementType = skill.IsCoreSkill ? "Required" : "Preferred",
                    ImportanceLevel = skill.ImportanceLevel
                });
            }
        }

        return links.ToArray();
    }

    private static Career Career(int index, string name, string description, string industry) => new()
    {
        CareerID = Id(1, index),
        CareerName = name,
        Description = description,
        Industry = industry,
        EducationRequirements = Education
    };

    private static Skill Skill(int index, string name, string category, string description) => new()
    {
        SkillID = Id(2, index),
        SkillName = name,
        Category = category,
        Description = description
    };

    private static Job Job(int index, int career, string title, string provider, string url, string source) => new()
    {
        JobID = Id(5, index),
        MappedCareerID = Id(1, career),
        JobTitle = $"{title} opportunities",
        CompanyName = provider,
        Location = "South Africa / Remote",
        Description = $"A curated search for current {title} vacancies. Open the listing link to review live roles, employers and closing dates on {provider}.",
        QualificationRequirement = Education,
        ExperienceMin = 0,
        ExperienceMax = null,
        JobType = "Curated search",
        ApplicationUrl = url,
        SourceName = "LevelUpCatalogue",
        SourceListingID = $"{source}-{title.ToLowerInvariant().Replace(' ', '-')}",
        PostedAt = SeededAt,
        RetrievedAt = SeededAt
    };

    private static Guid Id(int group, int index) =>
        Guid.Parse($"{group}0000000-0000-0000-0000-{index:D12}");

    private static CareerSkillSpec CS(int career, int skill, int importance, bool core = false) =>
        new(career, skill, importance, core);

    private static ResourceSpec R(int skill, string name, string type, string url, string provider,
        string level, string duration, string description) =>
        new(skill, name, type, url, provider, level, duration, description);

    private static JobSpec J(int career, string title, string search) => new(career, title, search);

    private sealed record CareerSkillSpec(int Career, int Skill, int Importance, bool Core);
    private sealed record ResourceSpec(int Skill, string Name, string Type, string Url, string Provider,
        string Level, string Duration, string Description);
    private sealed record JobSpec(int Career, string Title, string Search);
}
