using CapstoneProjectBE.Data;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

/// <summary>
/// Verifies Dashboard Analysis behavior.
/// </summary>
public class DashboardAnalysisTests
{
    [Fact]
    public async Task LatestTranscriptSkillsContributeToCareerAndJobMatches()
    {
        using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var userId = Guid.NewGuid();
        var career = new Career { CareerID = Guid.NewGuid(), CareerName = "Developer" };
        var skill = new Skill { SkillID = Guid.NewGuid(), SkillName = "SQL", Category = "Technical" };
        db.Users.Add(new User { UserID = userId, Email = "skills@example.com", IsActive = true });
        db.Careers.Add(career);
        db.Skills.Add(skill);
        db.CareerSkills.Add(new CareerSkill { CareerSkillID = Guid.NewGuid(), Career = career, Skill = skill, ImportanceLevel = 1 });
        db.UserProfiles.Add(new UserProfile { ProfileID = Guid.NewGuid(), UserID = userId, TargetCareerID = career.CareerID });
        db.TranscriptAnalyses.Add(new TranscriptAnalysis { AnalysisID = Guid.NewGuid(), UserID = userId, AnalysedAt = DateTime.UtcNow,
            ResultJson = $"{{\"detectedSkills\":[{{\"skillID\":\"{skill.SkillID}\"}}]}}" });
        var job = new Job { JobID = Guid.NewGuid(), MappedCareer = career, JobTitle = "Developer", CompanyName = "Example" };
        job.JobSkills.Add(new JobSkill { JobSkillID = Guid.NewGuid(), Skill = skill, ImportanceLevel = 1 });
        db.Jobs.Add(job);
        await db.SaveChangesAsync();
        var gap = await new SkillGapService(db).GetSkillGapAsync(userId, default);
        Assert.NotNull(gap);
        Assert.Equal(50m, gap.SkillMatchPercentage);
        Assert.Equal("Partial", Assert.Single(gap.Skills).Status);
        var match = await new JobMatchService(db).GetMatchForJobAsync(userId, job.JobID, default);
        Assert.NotNull(match);
        Assert.Equal(50m, match.SkillMatchScore);
        Assert.Single(match.MatchedSkills);
    }

    [Fact]
    public async Task SavingTargetCareerCreatesIncompleteProfileAndEnablesReadiness()
    {
        using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var id = Guid.NewGuid();
        var careerId = Guid.NewGuid();
        db.Users.Add(new User {UserID = id, Email = "target@example.com", IsActive = true});
        db.Careers.Add(new Career {CareerID = careerId, CareerName = "Software Developer"});
        var cv = new CV {CVID = Guid.NewGuid(), UserID = id, IsCurrent = true};
        db.CVs.Add(cv);
        db.CVAnalyses.Add(new CVAnalysis {AnalysisID = Guid.NewGuid(), CVID = cv.CVID, CV = cv, QualityScore = 70, AnalysedAt = DateTime.UtcNow});
        await db.SaveChangesAsync();
        var profile = await new ProfileService(db).UpdateTargetCareerAsync(id, careerId, default);
        Assert.NotNull(profile);
        Assert.False(profile.ProfileCompleted);
        var readiness = await new CareerReadinessService(db, new SkillGapService(db)).GetCareerReadinessAsync(id, default);
        Assert.NotNull(readiness);
        Assert.Equal(35, readiness.CareerReadinessScore);
    }

    [Fact]
    public async Task DashboardKeepsCvScoreWithoutInventingCareerReadinessWhenTargetIsMissing()
    {
        using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var id = Guid.NewGuid();
        db.Users.Add(new User {UserID = id, Email = "dashboard@example.com", IsActive = true});
        var cv = new CV {CVID = Guid.NewGuid(), UserID = id, IsCurrent = true, FileName = "cv.pdf"};
        db.CVs.Add(cv);
        db.CVAnalyses.Add(new CVAnalysis {AnalysisID = Guid.NewGuid(), CVID = cv.CVID, CV = cv, QualityScore = 76, AnalysedAt = DateTime.UtcNow});
        await db.SaveChangesAsync();
        var service = new DashboardService(db, new CareerReadinessService(db, new SkillGapService(db)));
        var result = await service.GetAsync(id, default);
        Assert.NotNull(result);
        Assert.Equal(76m, result.CVQualityScore);
        Assert.Null(result.CareerReadinessScore);
        Assert.Null(result.TargetCareer);
    }
}
