using CapstoneProjectBE.Data;
using CapstoneProjectBE.ExternalServices.CVAnalysis;
using CapstoneProjectBE.ExternalServices.Storage;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class CvWorkflowPipelineTests
{
    [Fact]
    public async Task UsesOwnedCvAndSavedCareerThenPersistsScoresAndDetectedSkills()
    {
        using var db = Database();
        var user = Guid.NewGuid(); var career = Guid.NewGuid(); var cv = Guid.NewGuid(); var skill = Guid.NewGuid();
        db.Users.Add(new User {UserID=user,Email="cv-pipeline@example.com",IsActive=true});
        db.Careers.Add(new Career {CareerID=career,CareerName="Software Developer"});
        db.UserProfiles.Add(new UserProfile {ProfileID=Guid.NewGuid(),UserID=user,TargetCareerID=career,FName="Ada",LName="Lovelace"});
        db.CVs.Add(new CV {CVID=cv,UserID=user,FileName="cv.pdf",FileType="application/pdf",FileLocation="owned-file",IsCurrent=true});
        db.Skills.Add(new Skill {SkillID=skill,SkillName="SQL"});
        await db.SaveChangesAsync();
        var provider = new Provider(); var storage = new Storage();
        var service = new CVAnalysisService(db,storage,provider);
        Assert.Null(await service.AnalyseAsync(Guid.NewGuid(),cv,default));
        Assert.Null(provider.Request);
        Assert.Equal(0,storage.Downloads);
        var result = await service.AnalyseAsync(user,cv,default);
        Assert.NotNull(result);
        Assert.Equal(78,result.QualityScore);
        Assert.Equal(cv,provider.Request!.CVID);
        Assert.Equal(career,provider.Request.CareerID);
        Assert.Equal("Software Developer",provider.Request.CareerName);
        Assert.Equal("application/pdf", provider.Request.ContentType);
        Assert.Equal(Storage.Bytes, provider.Request.Document);
        Assert.NotEqual(Guid.Empty,provider.Request.RequestId);
        Assert.Single(db.CVAnalyses);
        Assert.Single(db.CVAnalysisSkills);
        var evidence = Assert.Single(db.UserSkills);
        Assert.True(evidence.IsCVDetected);
        Assert.False(evidence.IsKnowledgeVerified);
    }

    [Fact]
    public async Task WorkflowFailureDoesNotSaveOrInventAnAnalysis()
    {
        using var db = Database();
        var user=Guid.NewGuid(); var cv=Guid.NewGuid();
        db.Users.Add(new User {UserID=user,Email="cv-failure@example.com",IsActive=true});
        db.CVs.Add(new CV {CVID=cv,UserID=user,FileName="cv.pdf",FileType="application/pdf"});
        await db.SaveChangesAsync();
        var service=new CVAnalysisService(db,new Storage(),new Provider {Fail=true});
        await Assert.ThrowsAsync<N8nAnalysisException>(()=>service.AnalyseAsync(user,cv,default));
        Assert.Empty(db.CVAnalyses);
        Assert.Empty(db.CVAnalysisSkills);
        Assert.Empty(db.UserSkills);
    }

    private static AppDbContext Database()=>new(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    private sealed class Provider : ICVAnalysisProvider
    {
        public bool Fail {get;init;}
        public CVAnalysisRequest? Request {get;private set;}
        public Task<CVAnalysisResult> AnalyseAsync(CVAnalysisRequest request,CancellationToken ct)
        {
            Request=request;
            if(Fail)throw new N8nAnalysisException("Workflow unavailable");
            return Task.FromResult(new CVAnalysisResult {QualityScore=78,ATSScore=80,FormattingScore=72,Strengths="SQL experience",Weaknesses="Add outcomes",Recommendations="Quantify results",DetectedSkills=request.KnownSkills});
        }
    }
    private sealed class Storage : ICVStorageService
    {
        public static readonly byte[] Bytes = "%PDF-1.4 workflow document"u8.ToArray();
        public int Downloads {get;private set;}
        public Task<byte[]> DownloadAsync(string path,CancellationToken ct){Downloads++;return Task.FromResult(Bytes);}
        public Task DeleteAsync(string path,CancellationToken ct)=>throw new NotSupportedException();
        public Task<string> UploadAsync(Guid user,Guid cv,IFormFile file,CancellationToken ct)=>throw new NotSupportedException();
    }
}
