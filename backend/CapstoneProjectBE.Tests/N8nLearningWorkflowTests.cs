using System.Net;
using System.Text;
using System.Text.Json;
using CapstoneProjectBE.ExternalServices.Assessments;
using CapstoneProjectBE.ExternalServices.Roadmaps;
using CapstoneProjectBE.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

public class N8nLearningWorkflowTests
{
    private static IConfiguration Configuration(string workflow, string url) => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            [$"N8n:{workflow}Url"] = url,
            ["N8n:AuthHeaderName"] = "X-LevelUp-API-Key",
            ["N8n:AuthHeaderValue"] = "secret"
        }).Build();

    [Fact]
    public async Task KnowledgeCheckSendsBoundedAuthenticatedPayloadAndAcceptsMultipleChoice()
    {
        var skillId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var handler = new Handler(async (request, cancellationToken) =>
        {
            Assert.Equal("secret", request.Headers.GetValues("X-LevelUp-API-Key").Single());
            using var body = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(cancellationToken));
            var requestId = body.RootElement.GetProperty("requestId").GetGuid();
            Assert.Equal(skillId, body.RootElement.GetProperty("skill").GetProperty("skillID").GetGuid());
            Assert.Equal(courseId, body.RootElement.GetProperty("course").GetProperty("resourceID").GetGuid());
            Assert.Equal("C# Fundamentals", body.RootElement.GetProperty("course").GetProperty("resourceName").GetString());
            Assert.Equal("Microsoft Learn", body.RootElement.GetProperty("course").GetProperty("provider").GetString());
            Assert.Equal(2, body.RootElement.GetProperty("attemptNumber").GetInt32());
            return Json(new
            {
                success = true, requestId, skillID = skillId, attemptNumber = 2,
                assessment = new {questions = Questions()}
            });
        });
        var generator = new N8nAssessmentQuestionGenerator(new N8nAnalysisService(new HttpClient(handler),
            Configuration("KnowledgeCheck", "https://example.com/webhook/knowledge-check-generate")));

        var questions = await generator.GenerateAsync(skillId, "C#", new N8nKnowledgeCheckCourse(
            courseId, "C# Fundamentals", "Microsoft Learn", "Course", "Learn C# basics."), 2, default);

        Assert.Equal(3, questions.Count);
        Assert.Equal(new[] {"A", "B", "C"}, questions[0].Choices);
        Assert.Equal("B", questions[0].CorrectAnswer);
    }

    [Fact]
    public void KnowledgeCheckRejectsFreeTextDuplicateChoicesAndAnswersOutsideChoices()
    {
        var requestId = Guid.NewGuid();
        var skillId = Guid.NewGuid();
        foreach (var questions in new object[]
        {
            Questions("Scenario"),
            Questions(choices: new[] {"A", "A", "C"}),
            Questions(answer: "D")
        })
        {
            using var json = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                success = true, requestId, skillID = skillId, attemptNumber = 1,
                assessment = new {questions}
            }));
            Assert.Throws<N8nAnalysisException>(() =>
                N8nAssessmentQuestionGenerator.Validate(json.RootElement, requestId, skillId, 1));
        }
    }

    [Fact]
    public async Task RoadmapSendsEverySkillButAcceptsExactlyTheActionableSkills()
    {
        var careerId = Guid.NewGuid();
        var missing = new N8nRoadmapSkill(Guid.NewGuid(), "C#", "Development", 5, true, "Missing");
        var partial = new N8nRoadmapSkill(Guid.NewGuid(), "SQL", "Data", 4, true, "Partial");
        var have = new N8nRoadmapSkill(Guid.NewGuid(), "Git", "Tools", 3, false, "Have");
        var skills = new[] {missing, partial, have};
        var handler = new Handler(async (request, cancellationToken) =>
        {
            Assert.Equal("secret", request.Headers.GetValues("X-LevelUp-API-Key").Single());
            using var body = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(cancellationToken));
            Assert.Equal(3, body.RootElement.GetProperty("skills").GetArrayLength());
            var requestId = body.RootElement.GetProperty("requestId").GetGuid();
            return Json(new
            {
                success = true, requestId, careerID = careerId,
                roadmap = new
                {
                    title = "Developer route",
                    items = new[]
                    {
                        new {skillID = missing.SkillID, title = "Learn C#", description = "Build a service.", priority = "high"},
                        new {skillID = partial.SkillID, title = "Practise SQL", description = "Query a database.", priority = "medium"}
                    }
                }
            });
        });
        var generator = new N8nRoadmapGenerator(new N8nAnalysisService(new HttpClient(handler),
            Configuration("Roadmap", "https://example.com/webhook/roadmap-generate")), new TextRoadmapGenerator());

        var roadmap = await generator.GenerateAsync(careerId, "Developer", skills, default);

        Assert.Equal("Developer route", roadmap.Title);
        Assert.Equal(new[] {missing.SkillID, partial.SkillID}, roadmap.Items.Select(item => item.SkillID));
    }

    [Fact]
    public async Task RoadmapUsesTrustedFallbackWhenN8nResponseIsInvalid()
    {
        var careerId = Guid.NewGuid();
        var missing = new N8nRoadmapSkill(Guid.NewGuid(), "C#", "Development", 5, true, "Missing");
        var partial = new N8nRoadmapSkill(Guid.NewGuid(), "SQL", "Data", 3, false, "Partial");
        var have = new N8nRoadmapSkill(Guid.NewGuid(), "REST APIs", "Development", 5, true, "Have");
        var handler = new Handler((_, _) => Task.FromResult(Json(new {message = "guardrail rejected output"})));
        var generator = new N8nRoadmapGenerator(new N8nAnalysisService(new HttpClient(handler),
            Configuration("Roadmap", "https://example.com/webhook/roadmap-generate")), new TextRoadmapGenerator());

        var roadmap = await generator.GenerateAsync(careerId, "Software Developer", [missing, partial, have], default);

        Assert.Equal("Software Developer Learning Roadmap", roadmap.Title);
        Assert.Equal(new[] {missing.SkillID, partial.SkillID}, roadmap.Items.Select(item => item.SkillID));
        Assert.Equal(new[] {"high", "medium"}, roadmap.Items.Select(item => item.Priority));
    }

    [Fact]
    public void RoadmapRejectsUnknownDuplicateOrMissingSkills()
    {
        var requestId = Guid.NewGuid();
        var careerId = Guid.NewGuid();
        var skill = new N8nRoadmapSkill(Guid.NewGuid(), "C#", null, 5, true, "Missing");
        foreach (var ids in new[] {Array.Empty<Guid>(), new[] {Guid.NewGuid()}, new[] {skill.SkillID, skill.SkillID}})
        {
            using var json = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                success = true, requestId, careerID = careerId,
                roadmap = new {title = "Route", items = ids.Select(id => new {skillID = id, title = "Step", description = "Description", priority = "high"})}
            }));
            Assert.Throws<N8nAnalysisException>(() => N8nRoadmapGenerator.Validate(json.RootElement, requestId, careerId, new[] {skill}));
        }
    }

    [Fact]
    public void RoadmapAcceptsN8nCodeNodeItemEnvelope()
    {
        var requestId = Guid.NewGuid();
        var careerId = Guid.NewGuid();
        var skill = new N8nRoadmapSkill(Guid.NewGuid(), "C#", null, 5, true, "Missing");
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(new[]
        {
            new
            {
                statusCode = 200,
                response = new
                {
                    success = true,
                    requestId,
                    careerID = careerId,
                    roadmap = new
                    {
                        title = "Developer route",
                        items = new[]
                        {
                            new {skillID = skill.SkillID, title = "Learn C#", description = "Build a small service.", priority = "high"}
                        }
                    }
                }
            }
        }));

        var roadmap = N8nRoadmapGenerator.Validate(json.RootElement, requestId, careerId, new[] {skill});

        Assert.Equal("Developer route", roadmap.Title);
        Assert.Equal(skill.SkillID, Assert.Single(roadmap.Items).SkillID);
    }

    [Fact]
    public void RoadmapAcceptsNestedEnvelopeAndCaseInsensitiveFieldNames()
    {
        var requestId = Guid.NewGuid();
        var careerId = Guid.NewGuid();
        var skill = new N8nRoadmapSkill(Guid.NewGuid(), "C#", null, 5, true, "Missing");
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(new
        {
            body = new[]
            {
                new
                {
                    json = new
                    {
                        SUCCESS = true,
                        REQUESTID = requestId,
                        CAREERID = careerId,
                        ROADMAP = new
                        {
                            TITLE = "Developer route",
                            ITEMS = new[]
                            {
                                new {SKILLID = skill.SkillID, TITLE = "Learn C#", DESCRIPTION = "Build a service.", PRIORITY = "high"}
                            }
                        }
                    }
                }
            }
        }));

        var roadmap = N8nRoadmapGenerator.Validate(json.RootElement, requestId, careerId, new[] {skill});

        Assert.Equal("Developer route", roadmap.Title);
        Assert.Equal(skill.SkillID, Assert.Single(roadmap.Items).SkillID);
    }

    [Fact]
    public void RoadmapRejectsDuplicateTitlesAndDatabaseLengthViolations()
    {
        var requestId = Guid.NewGuid();
        var careerId = Guid.NewGuid();
        var first = new N8nRoadmapSkill(Guid.NewGuid(), "C#", null, 5, true, "Missing");
        var second = new N8nRoadmapSkill(Guid.NewGuid(), "SQL", null, 4, true, "Partial");
        foreach (var (roadmapTitle, firstTitle, secondTitle) in new[]
        {
            ("Route", "Same step", "same STEP"),
            (new string('R', 151), "First", "Second"),
            ("Route", new string('S', 151), "Second")
        })
        {
            using var json = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                success = true, requestId, careerID = careerId,
                roadmap = new
                {
                    title = roadmapTitle,
                    items = new[]
                    {
                        new {skillID = first.SkillID, title = firstTitle, description = "Description", priority = "high"},
                        new {skillID = second.SkillID, title = secondTitle, description = "Description", priority = "medium"}
                    }
                }
            }));
            Assert.Throws<N8nAnalysisException>(() =>
                N8nRoadmapGenerator.Validate(json.RootElement, requestId, careerId, new[] {first, second}));
        }
    }

    private static object[] Questions(string type = "MultipleChoice", string[]? choices = null, string answer = "B") =>
        Enumerable.Range(1, 3).Select(index => (object)new
        {
            questionType = type, questionText = $"Question {index}", choices = choices ?? new[] {"A", "B", "C"}, correctAnswer = answer
        }).ToArray();

    private static HttpResponseMessage Json(object value) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json")
    };

    private sealed class Handler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            send(request, cancellationToken);
    }
}
