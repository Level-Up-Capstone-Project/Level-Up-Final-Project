using System.Net;
using System.Text;
using System.Text.Json;
using CapstoneProjectBE.Data;
using CapstoneProjectBE.ExternalServices.Roadmaps;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

/// <summary>
/// Verifies persistent deduplication and reuse of AI-discovered learning resources.
/// </summary>
public sealed class LearningResourceDiscoveryServiceTests
{
    [Fact]
    public async Task ProviderIdentityUpdatesOneAiResourceWhenItsUrlChanges()
    {
        await using AppDbContext database = Database();
        Guid skillId = await AddSkillAsync(database);
        var service = new LearningResourceDiscoveryService(database);

        await service.PersistAsync(Roadmap(skillId, Candidate(
            "https://www.freecodecamp.org/learn/old-course",
            "responsive-web-design")), default);
        await service.PersistAsync(Roadmap(skillId, Candidate(
            "https://www.freecodecamp.org/learn/new-course",
            "responsive-web-design")), default);

        LearningResource saved = Assert.Single(await database.LearningResources.ToListAsync());
        Assert.Equal("https://www.freecodecamp.org/learn/new-course", saved.Url);
        Assert.Equal("responsive-web-design", saved.ProviderResourceID);
        Assert.Equal("AI", saved.SourceType);
    }

    [Fact]
    public async Task CanonicalUrlReusesCuratedResourceWithoutOverwritingIt()
    {
        await using AppDbContext database = Database();
        Guid skillId = await AddSkillAsync(database);
        database.LearningResources.Add(new LearningResource
        {
            ResourceID = Guid.NewGuid(),
            SkillID = skillId,
            ResourceName = "Reviewed course",
            ResourceType = "Course",
            Url = "https://example.com/course",
            CanonicalUrl = "https://example.com/course",
            Provider = "Example",
            DifficultyLevel = "Beginner",
            CostType = "Free",
            EstDuration = "2 hours",
            Description = "Reviewed by the catalogue team.",
            SourceType = "Curated",
            IsActive = true
        });
        await database.SaveChangesAsync();

        await new LearningResourceDiscoveryService(database).PersistAsync(
            Roadmap(skillId, Candidate("https://example.com/course", null)), default);

        LearningResource saved = Assert.Single(await database.LearningResources.ToListAsync());
        Assert.Equal("Reviewed course", saved.ResourceName);
        Assert.Equal("Curated", saved.SourceType);
    }

    [Fact]
    public async Task RoadmapRequestsOnlyMissingSlotsThenPersistsAndLinksTheNewResource()
    {
        await using AppDbContext database = Database();
        Guid skillId = await AddSkillAsync(database);
        Skill skill = await database.Skills.SingleAsync(item => item.SkillID == skillId);
        var career = new Career
        {
            CareerID = Guid.NewGuid(),
            CareerName = "Frontend Developer",
            Description = "Build user interfaces.",
            Industry = "Technology",
            EducationRequirements = "Portfolio"
        };
        career.CareerSkills.Add(new CareerSkill
        {
            CareerSkillID = Guid.NewGuid(),
            CareerID = career.CareerID,
            SkillID = skillId,
            Career = career,
            Skill = skill,
            ImportanceLevel = 5,
            IsCoreSkill = true
        });
        database.Careers.Add(career);
        database.LearningResources.AddRange(
            Curated(skillId, "HTML guide", "https://example.com/html"),
            Curated(skillId, "CSS guide", "https://example.com/css"));
        await database.SaveChangesAsync();

        var handler = new Handler(async (request, cancellationToken) =>
        {
            using var body = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(cancellationToken));
            JsonElement requestedSkill = body.RootElement.GetProperty("skills")[0];
            Assert.Equal(1, requestedSkill.GetProperty("resourceSlots").GetInt32());
            Assert.Equal(2, requestedSkill.GetProperty("existingResources").GetArrayLength());
            Assert.Contains(requestedSkill.GetProperty("existingResources").EnumerateArray(), resource =>
                resource.GetProperty("url").GetString() == "https://example.com/html");
            Guid requestId = body.RootElement.GetProperty("requestId").GetGuid();
            return Json(new
            {
                success = true,
                requestId,
                careerID = career.CareerID,
                roadmap = new
                {
                    title = "Frontend roadmap",
                    items = new[]
                    {
                        new
                        {
                            skillID = skillId,
                            title = "Strengthen responsive design",
                            description = "Complete focused responsive-design learning.",
                            priority = "high",
                            resources = new[]
                            {
                                new
                                {
                                    providerResourceID = "responsive-web-design",
                                    resourceName = "Responsive Web Design",
                                    resourceType = "Course",
                                    url = "https://www.freecodecamp.org/learn/2022/responsive-web-design/",
                                    provider = "freeCodeCamp",
                                    difficultyLevel = "Beginner",
                                    costType = "Free",
                                    estDuration = "Self-paced",
                                    description = "Build responsive pages with HTML and CSS."
                                }
                            }
                        }
                    }
                }
            });
        });
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["N8n:RoadmapUrl"] = "https://example.com/webhook/roadmap-generate",
                ["N8n:AuthHeaderName"] = "X-LevelUp-API-Key",
                ["N8n:AuthHeaderValue"] = "secret"
            }).Build();
        var generator = new N8nRoadmapGenerator(
            new N8nAnalysisService(new HttpClient(handler), configuration),
            new TextRoadmapGenerator());
        var service = new RoadmapService(database, generator, new LearningResourceDiscoveryService(database));

        var roadmap = await service.GenerateRoadmapAsync(Guid.NewGuid(), career.CareerID, default);

        Assert.NotNull(roadmap);
        Assert.Equal(3, await database.LearningResources.CountAsync(resource => resource.SkillID == skillId));
        Assert.Equal(3, await database.RoadmapItemResources.CountAsync());
        Assert.Contains(await database.LearningResources.ToListAsync(), resource =>
            resource.ProviderResourceID == "responsive-web-design" && resource.SourceType == "AI");
    }

    private static AppDbContext Database() => new(new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options);

    private static async Task<Guid> AddSkillAsync(AppDbContext database)
    {
        var skill = new Skill
        {
            SkillID = Guid.NewGuid(),
            SkillName = "Responsive Web Design",
            Category = "Development",
            Description = "Build responsive websites."
        };
        database.Skills.Add(skill);
        await database.SaveChangesAsync();
        return skill.SkillID;
    }

    private static GeneratedLearningResource Candidate(string url, string? providerResourceId) => new(
        "Responsive Web Design",
        "Course",
        url,
        providerResourceId is null ? "Example" : "freeCodeCamp",
        providerResourceId is null ? "example" : "freecodecamp",
        providerResourceId,
        "Beginner",
        "Free",
        "Self-paced",
        "Learn responsive HTML and CSS.");

    private static LearningResource Curated(Guid skillId, string name, string url) => new()
    {
        ResourceID = Guid.NewGuid(),
        SkillID = skillId,
        ResourceName = name,
        ResourceType = "Guide",
        Url = url,
        CanonicalUrl = url,
        Provider = "Example",
        DifficultyLevel = "Beginner",
        CostType = "Free",
        EstDuration = "1 hour",
        Description = name,
        SourceType = "Curated",
        IsActive = true
    };

    private static GeneratedRoadmap Roadmap(Guid skillId, GeneratedLearningResource resource) => new(
        "Frontend roadmap",
        new[]
        {
            new GeneratedRoadmapItem(
                skillId,
                "Learn responsive design",
                "Build responsive pages.",
                "high",
                new[] { resource })
        });

    private static HttpResponseMessage Json(object value) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json")
    };

    /// <summary>
    /// Supplies controlled n8n responses while preserving the production HTTP request shape.
    /// </summary>
    private sealed class Handler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) => send(request, cancellationToken);
    }
}
