using CapstoneProjectBE.Data;
using CapstoneProjectBE.Services.Interfaces;
using CapstoneProjectBE.Services;
using Microsoft.EntityFrameworkCore;
using CapstoneProjectBE.Models;
using Microsoft.AspNetCore.Identity;
using CapstoneProjectBE.ExternalServices.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CapstoneProjectBE.ExternalServices.Storage;
using CapstoneProjectBE.ExternalServices.CVAnalysis;
using CapstoneProjectBE.ExternalServices.Assessments;
using CapstoneProjectBE.Controllers;
using Microsoft.AspNetCore.Authentication;
using CapstoneProjectBE.ExternalServices.Transcripts;
using CapstoneProjectBE.ExternalServices.Roadmaps;
using CapstoneProjectBE.Database;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var frontendOrigins = (builder.Configuration["Frontend:Origins"] ?? builder.Configuration["Frontend:Origin"] ?? "http://localhost:3000")
    .Split([';', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
if (frontendOrigins.Length == 0 || frontendOrigins.Any(origin => !Uri.TryCreate(origin, UriKind.Absolute, out var uri) || uri.GetLeftPart(UriPartial.Authority) != origin.TrimEnd('/')))
    throw new InvalidOperationException("Frontend:Origins must contain one or more absolute frontend origins.");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "The DefaultConnection connection string is missing. Configure it with User Secrets during development.");
}

string jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is missing");
string jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is missing");
string jwtSigningKey = builder.Configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT signing key is missing");

byte[] jwtKeyBytes;

try
{
    jwtKeyBytes = Convert.FromBase64String(jwtSigningKey);
}
catch(FormatException ex)
{
    throw new InvalidOperationException("JWT signing key must be valid base64", ex);
}
if (jwtKeyBytes.Length < 32)
{
    throw new InvalidOperationException("JWT signing key must contain at least 32 bytes");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(frontendOrigins).AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IPasswordHasher<EmailVerification>, PasswordHasher<EmailVerification>>();
builder.Services.AddScoped<IPasswordHasher<PasswordReset>, PasswordHasher<PasswordReset>>();
builder.Services.AddScoped<IVerificationCodeService, VerificationCodeService>();
builder.Services.AddLevelUpEmail(builder.Configuration, builder.Environment);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ICareerService, CareerService>();
builder.Services.AddScoped<ILearningResourceService, LearningResourceService>();
builder.Services.AddScoped<IRoadmapService, RoadmapService>();
builder.Services.AddScoped<LearningResourceDiscoveryService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IUserSkillService, UserSkillService>();
builder.Services.AddScoped<ISkillGapService, SkillGapService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<ISavedJobService, SavedJobService>();
builder.Services.AddScoped<IJobMatchService, JobMatchService>();
builder.Services.AddSingleton<ICVTextExtractor, CVTextExtractor>();
builder.Services.AddSingleton<LocalPdfOcr>();
builder.Services.AddSingleton<AdobePdfOcr>();
// Transcripts always pass through Adobe, independently of the CV fallback provider.
builder.Services.AddKeyedSingleton<ICVTextExtractor>("Transcript", (services, _) =>
    new CVTextExtractor(services.GetRequiredService<AdobePdfOcr>(), alwaysOcr: true));
builder.Services.AddHttpClient("AdobeOcr", client => client.Timeout = TimeSpan.FromSeconds(120))
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler {AllowAutoRedirect = false})
    .RemoveAllLoggers(); // Signed asset URLs and credential-bearing responses must not enter request logs.
builder.Services.AddSingleton<IPdfOcr>(services =>
    string.Equals(builder.Configuration["Ocr:Provider"], "Adobe", StringComparison.OrdinalIgnoreCase)
        ? services.GetRequiredService<AdobePdfOcr>() : services.GetRequiredService<LocalPdfOcr>());
builder.Services.AddSingleton<TextCVAnalysisProvider>();
builder.Services.AddScoped<ICVAnalysisProvider, N8nCVAnalysisProvider>();
builder.Services.AddScoped<ICVAnalysisService, CVAnalysisService>();
builder.Services.AddScoped<ITranscriptAnalysisProvider, N8nTranscriptAnalysisProvider>();
builder.Services.AddHttpClient<N8nAnalysisService>(client => client.Timeout = Timeout.InfiniteTimeSpan)
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler {AllowAutoRedirect = false})
    .RemoveAllLoggers();
builder.Services.AddScoped<ICareerReadinessService, CareerReadinessService>();
builder.Services.AddScoped<ISavedResourceService, SavedResourceService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<N8nAssessmentQuestionGenerator>();
builder.Services.AddSingleton<TextRoadmapGenerator>();
builder.Services.AddScoped<N8nRoadmapGenerator>();
builder.Services.AddScoped<IAssessmentService, AssessmentService>();

builder.Services.AddHttpClient<ICVStorageService, CVStorageService>();
builder.Services.AddScoped<ICVService, CVService>();

var authentication = builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes),
        ValidateLifetime = true,
        RequireExpirationTime = true,
        RequireSignedTokens = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddScoped<GoogleLoginService>();
authentication.AddCookie(GoogleAuthController.ExternalCookie, options =>
{
    options.Cookie.Name = "levelup.google.external";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
});
if (!string.IsNullOrWhiteSpace(builder.Configuration["Authentication:Google:ClientId"]) &&
    !string.IsNullOrWhiteSpace(builder.Configuration["Authentication:Google:ClientSecret"]))
{
    authentication.AddGoogle("Google", options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.SignInScheme = GoogleAuthController.ExternalCookie;
        options.UsePkce = true;
        options.SaveTokens = false;
        options.ClaimActions.MapJsonKey("google:email_verified", "email_verified");
        options.ClaimActions.MapJsonKey("google:email_verified", "verified_email");
        options.Events.OnRemoteFailure = context =>
        {
            context.HandleResponse();
            context.Response.Redirect("/api/auth/google/error");
            return Task.CompletedTask;
        };
    });
}

builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    static string IpPartition(HttpContext context) => context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    static string UserPartition(HttpContext context) =>
        context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? IpPartition(context);

    options.AddPolicy("authentication", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(IpPartition(httpContext),
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
    options.AddPolicy("password-recovery", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            IpPartition(httpContext),
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(5),
                QueueLimit = 0
            }));
    options.AddPolicy("file-upload", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(UserPartition(httpContext),
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(5),
                QueueLimit = 0
            }));
    options.AddPolicy("ai-workflow", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(UserPartition(httpContext),
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(5),
                QueueLimit = 0
            }));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme, Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (args.Contains("--migrate-database", StringComparer.Ordinal))
{
    using var scope = app.Services.CreateScope();
    var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DatabaseMigrationManager.MigrateAsync(database);
    Console.WriteLine("Database migrations are up to date.");
    return;
}

if (args.Contains("--adopt-existing-database", StringComparer.Ordinal))
{
    using var scope = app.Services.CreateScope();
    var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DatabaseMigrationManager.AdoptExistingSchemaAsync(database);
    Console.WriteLine("Existing database adopted into the current EF migration baseline.");
    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();
