using CapstoneProjectBE.Data;
using CapstoneProjectBE.ExternalServices.CVAnalysis;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CapstoneProjectBE.Tests;

public class MvpCatalogueSeedTests
{
    [Fact]
    public async Task Catalogue_CoversCareersResourcesAndCuratedJobSearches()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();

        Assert.Equal(6, await context.Careers.CountAsync());
        Assert.Equal(22, await context.Skills.CountAsync());
        Assert.Equal(44, await context.LearningResources.CountAsync(resource => resource.IsActive));
        Assert.Equal(12, await context.Jobs.CountAsync());

        var resourceCounts = await context.LearningResources
            .Where(resource => resource.IsActive)
            .GroupBy(resource => resource.SkillID)
            .Select(group => new { SkillID = group.Key, Count = group.Count() })
            .ToDictionaryAsync(item => item.SkillID, item => item.Count);

        var careerSkillIds = await context.CareerSkills
            .Select(careerSkill => careerSkill.SkillID)
            .Distinct()
            .ToListAsync();

        Assert.All(careerSkillIds, skillId => Assert.Equal(2, resourceCounts[skillId]));

        var careerJobCounts = await context.Jobs
            .GroupBy(job => job.MappedCareerID)
            .Select(group => group.Count())
            .ToListAsync();

        Assert.Equal(6, careerJobCounts.Count);
        Assert.All(careerJobCounts, count => Assert.Equal(2, count));
        Assert.All(await context.Jobs.ToListAsync(), job =>
        {
            Assert.Equal("Curated search", job.JobType);
            Assert.StartsWith("https://", job.ApplicationUrl);
        });
    }

    [Fact]
    public async Task SoftwareDeveloper_ContainsRequestedSkills_AndCvTextCanMatchThem()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();

        var skillNames = await context.CareerSkills
            .Where(careerSkill => careerSkill.Career.CareerName == "Software Developer")
            .Select(careerSkill => careerSkill.Skill.SkillName)
            .ToListAsync();

        string[] requested = ["C#", "Java", "Python", "SQL", "PostgreSQL", "React", "HTML", "CSS", "JavaScript"];
        Assert.All(requested, skill => Assert.Contains(skill, skillNames));

        const string cvText = "Built React interfaces with HTML, CSS and JavaScript; developed C# and Java services using Python, SQL and PostgreSQL.";
        Assert.All(requested, skill => Assert.True(
            SkillNameMatcher.TextContains(cvText, skill),
            $"Expected the CV matcher to detect {skill}."));
    }

    [Fact]
    public async Task CuratedJobs_UseTheSameSkillSetAsTheirCareer()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();

        var careerSkillRows = await context.CareerSkills.ToListAsync();
        var careerSkills = careerSkillRows
            .GroupBy(item => item.CareerID)
            .ToDictionary(group => group.Key, group => group.Select(item => item.SkillID).Order().ToArray());

        var jobs = await context.Jobs.Include(job => job.JobSkills).ToListAsync();

        Assert.All(jobs, job => Assert.Equal(
            careerSkills[job.MappedCareerID],
            job.JobSkills.Select(item => item.SkillID).Order().ToArray()));
    }

    [Theory]
    [InlineData("React", "React.js")]
    [InlineData("React", "ReactJS")]
    [InlineData("PostgreSQL", "Postgres")]
    [InlineData("HTML", "HTML5")]
    [InlineData("CSS", "CSS3")]
    [InlineData("JavaScript", "JS")]
    [InlineData("TypeScript", "TS")]
    [InlineData("Node.js", "Node JS")]
    [InlineData("Power BI", "PowerBI")]
    public void CvMatcher_RecognisesCommonCatalogueAliases(string catalogueSkill, string cvSpelling)
    {
        Assert.True(SkillNameMatcher.NamesMatch(catalogueSkill, cvSpelling));
        Assert.True(SkillNameMatcher.TextContains($"Experienced with {cvSpelling} in production.", catalogueSkill));
    }

    private static AppDbContext CreateContext() => new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
