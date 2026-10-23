using CapstoneProjectBE.Data;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class DashboardAnalysisTests
{
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
