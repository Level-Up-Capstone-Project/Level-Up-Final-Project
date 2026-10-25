using CapstoneProjectBE.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Data
{
    /// <summary>
    /// Provides App Db Context database configuration and lifecycle behavior.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Career> Careers { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<CareerSkill> CareerSkills { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ExternalLogin> ExternalLogins { get; set; }
        public DbSet<TranscriptAnalysis> TranscriptAnalyses { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<EmailVerification> EmailVerifications { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }
        public DbSet<SavedJob> SavedJobs { get; set; }
        public DbSet<JobMatch> JobMatches { get; set; }
        public DbSet<LearningResource> LearningResources { get; set; }
        public DbSet<LearningRoadmap> LearningRoadmaps { get; set; }
        public DbSet<RoadmapItem> RoadmapItems { get; set; }
        public DbSet<RoadmapItemResource> RoadmapItemResources { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }
        public DbSet<CV> CVs { get; set; }
        public DbSet<CVAnalysis> CVAnalyses { get; set; }
        public DbSet<CVAnalysisSkill> CVAnalysisSkills { get; set; }
        public DbSet<SavedResource> SavedResources { get; set; }
        public DbSet<SkillAssessment> SkillAssessments { get; set; }
        public DbSet<AssessmentQuestion> AssessmentQuestions { get; set; }
        public DbSet<AssessmentResponse> AssessmentResponses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TranscriptAnalysis>(entity =>
            {
                entity.ToTable("transcript_analyses");
                entity.HasKey(item => item.AnalysisID);
                entity.Property(item => item.AnalysisID).HasColumnName("analysis_id");
                entity.Property(item => item.UserID).HasColumnName("user_id");
                entity.Property(item => item.AnalysedAt).HasColumnName("analysed_at");
                entity.Property(item => item.ResultJson).HasColumnName("result_json").HasColumnType("jsonb").IsRequired();
                entity.HasOne(item => item.User).WithMany().HasForeignKey(item => item.UserID).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(item => new {item.UserID, item.AnalysedAt});
            });
            modelBuilder.Entity<ExternalLogin>(entity =>
            {
                entity.ToTable("external_logins");
                entity.HasKey(login => new { login.Provider, login.Subject });
                entity.Property(login => login.Provider).HasColumnName("provider").HasMaxLength(40);
                entity.Property(login => login.Subject).HasColumnName("subject").HasMaxLength(255);
                entity.Property(login => login.UserID).HasColumnName("user_id");
                entity.HasOne(login => login.User).WithMany().HasForeignKey(login => login.UserID).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(login => new { login.UserID, login.Provider }).IsUnique();
            });

            modelBuilder.Entity<Career>(entity =>
            {
                entity.ToTable("careers");

                entity.HasKey(career => career.CareerID)
                    .HasName("careers_pkey");

                entity.Property(career => career.CareerID)
                    .HasColumnName("career_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(career => career.CareerName)
                    .HasColumnName("career_name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(career => career.Description)
                    .HasColumnName("description")
                    .IsRequired();

                entity.Property(career => career.Industry)
                    .HasColumnName("industry")
                    .IsRequired();

                entity.Property(career => career.EducationRequirements)
                    .HasColumnName("education_requirements")
                    .IsRequired();

                entity.HasIndex(career => career.CareerName)
                    .IsUnique()
                    .HasDatabaseName("careers_career_name_key");
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.ToTable("skills");

                entity.HasKey(skill => skill.SkillID)
                    .HasName("skills_pkey");

                entity.Property(skill => skill.SkillID)
                    .HasColumnName("skill_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(skill => skill.SkillName)
                    .HasColumnName("skill_name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(skill => skill.Category)
                    .HasColumnName("category")
                    .IsRequired();

                entity.Property(skill => skill.Description)
                    .HasColumnName("description")
                    .IsRequired();

                entity.HasIndex(skill => skill.SkillName)
                    .IsUnique()
                    .HasDatabaseName("skills_skill_name_key");
            });

            modelBuilder.Entity<CareerSkill>(entity =>
            {
                entity.ToTable("career_skills");

                entity.HasKey(careerSkill => careerSkill.CareerSkillID)
                    .HasName("career_skills_pkey");

                entity.Property(careerSkill => careerSkill.CareerSkillID)
                    .HasColumnName("career_skill_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(careerSkill => careerSkill.CareerID)
                    .HasColumnName("career_id");

                entity.Property(careerSkill => careerSkill.SkillID)
                    .HasColumnName("skill_id");

                entity.Property(careerSkill => careerSkill.ImportanceLevel)
                    .HasColumnName("importance_level");

                entity.Property(careerSkill => careerSkill.IsCoreSkill)
                    .HasColumnName("is_core_skill")
                    .HasDefaultValue(false);

                entity.HasIndex(careerSkill => new
                {
                    careerSkill.CareerID,
                    careerSkill.SkillID
                })
                    .IsUnique()
                    .HasDatabaseName(
                        "career_skills_career_id_skill_id_key");

                entity.HasIndex(careerSkill => careerSkill.SkillID)
                    .HasDatabaseName("career_skills_skill_id_idx");

                entity.HasOne(careerSkill => careerSkill.Career)
                    .WithMany(career => career.CareerSkills)
                    .HasForeignKey(careerSkill => careerSkill.CareerID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("career_skills_career_id_fkey");

                entity.HasOne(careerSkill => careerSkill.Skill)
                    .WithMany(skill => skill.CareerSkills)
                    .HasForeignKey(careerSkill => careerSkill.SkillID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("career_skills_skill_id_fkey");
            });

            modelBuilder.Entity<LearningRoadmap>(entity =>
            {

                entity.ToTable("learning_roadmaps");

                entity.HasKey(roadmap => roadmap.RoadmapID);

                entity.Property(roadmap => roadmap.RoadmapID)
                .HasColumnName("roadmap_id")
                .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(roadmap => roadmap.UserID)
                .HasColumnName("user_id");

                entity.Property(roadmap => roadmap.CareerID)
                .HasColumnName("career_id");

                entity.Property(roadmap => roadmap.Title)
                .HasColumnName("title")
                .HasMaxLength(150)
                .IsRequired();

                entity.Property(roadmap => roadmap.ProgressPercentage)
                .HasColumnName("progress_percentage")
                .HasPrecision(5,2);

                entity.Property(roadmap => roadmap.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

                entity.Property(roadmap => roadmap.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

                entity.HasOne(roadmap => roadmap.Career)
                .WithMany()
                .HasForeignKey(roadmap => roadmap.CareerID);

                entity.HasOne(roadmap => roadmap.User)
                .WithMany()
                .HasForeignKey(roadmap => roadmap.UserID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("learning_roadmaps_user_id_fkey");

            });


            modelBuilder.Entity<RoadmapItem>(entity =>
            {
                entity.ToTable("roadmap_items");
                entity.HasKey(item => item.RoadmapItemID);

                entity.Property(item => item.RoadmapItemID)
                .HasColumnName("roadmap_item_id")
                .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(item => item.RoadmapID)
                .HasColumnName("roadmap_id");

                entity.Property(item => item.SkillID)
                .HasColumnName("skill_id");

                entity.Property(item => item.Title)
                .HasColumnName("title")
                .HasMaxLength(150)
                .IsRequired();

                entity.Property(item => item.Description)
                .HasColumnName("description");

                entity.Property(item => item.StepOrder)
                .HasColumnName("step_order");

                entity.Property(item => item.Priority)
                .HasColumnName("priority");

                entity.Property(item => item.Status)
                .HasColumnName("status");

                entity.Property(item => item.TargetDate)
                .HasColumnName("target_date")
                .HasColumnType("date");

                entity.Property(item => item.CompletedAt)
                .HasColumnName("completed_at")
                .HasColumnType("timestamp with time zone");

                entity.HasIndex(item => new { item.RoadmapID, item.StepOrder})
                .IsUnique();

                entity.HasOne(item => item.Roadmap)
                .WithMany(roadmap => roadmap.Items)
                .HasForeignKey(item => item.RoadmapID)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(item => item.Skill)
                .WithMany()
                .HasForeignKey(item => item.SkillID);

            
                
            });

            modelBuilder.Entity<LearningResource>(entity =>
            {
                entity.ToTable("learning_resources");

                entity.HasKey(resource => resource.ResourceID);

                entity.Property(resource => resource.ResourceID)
                .HasColumnName("resource_id")
                .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(resource => resource.SkillID)
                .HasColumnName("skill_id");

                entity.Property(resource => resource.ResourceName)
                .HasColumnName("resource_name")
                .HasMaxLength(200)
                .IsRequired();
                
                entity.Property(resource => resource.ResourceType)
                .HasColumnName("resource_type");

                entity.Property(resource => resource.Url)
                .HasColumnName("url")
                .HasMaxLength(500)
                .IsRequired();

                entity.Property(resource => resource.Provider)
                .HasColumnName("provider");

                entity.Property(resource => resource.DifficultyLevel)
                .HasColumnName("difficulty_level");

                entity.Property(resource => resource.CostType)
                .HasColumnName("cost_type");

                entity.Property(resource => resource.EstDuration)
                .HasColumnName("est_duration");

                entity.Property(resource => resource.Description)
                .HasColumnName("description");

                entity.Property(resource => resource.SourceType)
                .HasColumnName("source_type");

                entity.Property(resource => resource.IsActive)
                .HasColumnName("is_active");

                entity.HasIndex(resource => resource.SkillID);

                entity.HasOne(resource => resource.Skill)
                .WithMany()
                .HasForeignKey(resource => resource.SkillID);

                
            });

            modelBuilder.Entity<RoadmapItemResource>(entity =>
            {
                entity.ToTable("roadmap_item_resources");

                entity.HasKey(link => link.RoadmapItemResourceID);

                entity.Property(link => link.RoadmapItemResourceID)
                .HasColumnName("roadmap_item_resource_id")
                .HasDefaultValueSql("gen_random_uuid()");

               entity.Property(link => link.RoadmapItemID)
               .HasColumnName("roadmap_item_id");

                entity.Property(link => link.ResourceID)
               .HasColumnName("resource_id");

                entity.Property(link => link.IsCompleted)
               .HasColumnName("is_completed");

                entity.Property(link => link.CompletedAt)
               .HasColumnName("completed_at")
               .HasColumnType("timestamp with time zone");

               entity.HasIndex(link => new { link.RoadmapItemID, link.ResourceID})
               .IsUnique();

               entity.HasOne(link => link.RoadmapItem)
               .WithMany(item => item.Resources)
               .HasForeignKey(link => link.RoadmapItemID)
               .OnDelete(DeleteBehavior.Cascade);

               entity.HasOne(link => link.LearningResource)
               .WithMany()
               .HasForeignKey(link => link.ResourceID);
               
            });

            MvpCatalogueSeed.Configure(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(user => user.UserID)
                    .HasName("users_pkey");

                entity.Property(user => user.UserID)
                    .HasColumnName("user_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(user => user.Email)
                    .HasColumnName("email")
                    .HasMaxLength(254)
                    .IsRequired();

                entity.HasIndex(user => user.Email)
                    .IsUnique()
                    .HasDatabaseName("users_email_key");

                entity.Property(user => user.PasswordHash)
                    .HasColumnName("password_hash")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(user => user.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.Property(user => user.IsActive)
                    .HasColumnName("is_active")
                    .HasDefaultValue(false);

                entity.HasOne(user => user.UserProfile)
                    .WithOne(profile => profile.User)
                    .HasForeignKey<UserProfile>(profile => profile.UserID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("user_profiles_user_id_fkey");
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("user_profiles");

                entity.HasKey(profile => profile.ProfileID)
                    .HasName("user_profiles_pkey");

                entity.Property(profile => profile.ProfileID)
                    .HasColumnName("profile_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(profile => profile.UserID)
                    .HasColumnName("user_id");

                entity.Property(profile => profile.TargetCareerID)
                    .HasColumnName("target_career_id");

                entity.Property(profile => profile.FName)
                    .HasColumnName("first_name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(profile => profile.LName)
                    .HasColumnName("last_name")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(profile => profile.Qualification)
                    .HasColumnName("qualification")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(profile => profile.FieldOfStudy)
                    .HasColumnName("field_of_study")
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(profile => profile.ExperienceLevel)
                    .HasColumnName("experience_level")
                    .HasMaxLength(100);

                entity.Property(profile => profile.Interests)
                    .HasColumnName("interests");

                entity.Property(profile => profile.Industry)
                    .HasColumnName("industry")
                    .HasMaxLength(100);

                entity.Property(profile => profile.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(30);

                entity.Property(profile => profile.TargetRoles)
                    .HasColumnName("target_roles")
                    .HasColumnType("text[]")
                    .IsRequired();

                entity.Property(profile => profile.WorkAreasJson)
                    .HasColumnName("work_areas")
                    .HasColumnType("jsonb");

                entity.Property(profile => profile.ProfileCompleted)
                    .HasColumnName("profile_completed")
                    .HasDefaultValue(false);

                entity.Property(profile => profile.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.HasIndex(profile => profile.UserID)
                    .IsUnique()
                    .HasDatabaseName("user_profiles_user_id_key");

                entity.HasIndex(profile => profile.TargetCareerID)
                    .HasDatabaseName("user_profiles_target_career_id_idx");

                entity.HasOne(profile => profile.TargetCareer)
                    .WithMany()
                    .HasForeignKey(profile => profile.TargetCareerID)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName(
                        "user_profiles_target_career_id_fkey");
            });

            modelBuilder.Entity<EmailVerification>(entity =>
            {
                entity.ToTable("email_verifications");

                entity.HasKey(verification => verification.EmailVerificationID)
                    .HasName("email_verifications_pkey");

                entity.Property(verification => verification.EmailVerificationID)
                    .HasColumnName("email_verification_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(verification => verification.UserID)
                    .HasColumnName("user_id");

                entity.Property(verification => verification.CodeHash)
                    .HasColumnName("code_hash")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(verification => verification.ExpiresAt)
                    .HasColumnName("expires_at")
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();

                entity.Property(verification => verification.LastSentAt)
                    .HasColumnName("last_sent_at")
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();

                entity.Property(verification => verification.FailedAttempts)
                    .HasColumnName("failed_attempts")
                    .HasDefaultValue(0);

                entity.HasIndex(verification => verification.UserID)
                    .IsUnique()
                    .HasDatabaseName("email_verifications_user_id_key");

                entity.HasOne(verification => verification.User)
                    .WithOne(user => user.EmailVerification)
                    .HasForeignKey<EmailVerification>(
                        verification => verification.UserID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("email_verifications_user_id_fkey");
            });

            modelBuilder.Entity<PasswordReset>(entity =>
            {
                entity.ToTable("password_resets");
                entity.HasKey(reset => reset.PasswordResetID).HasName("password_resets_pkey");
                entity.Property(reset => reset.PasswordResetID)
                    .HasColumnName("password_reset_id")
                    .HasDefaultValueSql("gen_random_uuid()");
                entity.Property(reset => reset.UserID).HasColumnName("user_id");
                entity.Property(reset => reset.CodeHash).HasColumnName("code_hash").HasMaxLength(500).IsRequired();
                entity.Property(reset => reset.ExpiresAt).HasColumnName("expires_at").HasColumnType("timestamp with time zone").IsRequired();
                entity.Property(reset => reset.LastSentAt).HasColumnName("last_sent_at").HasColumnType("timestamp with time zone").IsRequired();
                entity.Property(reset => reset.FailedAttempts).HasColumnName("failed_attempts").HasDefaultValue(0);
                entity.HasIndex(reset => reset.UserID).IsUnique().HasDatabaseName("password_resets_user_id_key");
                entity.HasOne(reset => reset.User).WithOne(user => user.PasswordReset)
                    .HasForeignKey<PasswordReset>(reset => reset.UserID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("password_resets_user_id_fkey");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("jobs");

                entity.HasKey(job => job.JobID)
                .HasName("jobs_pkey");

                entity.Property(job => job.JobID)
                .HasColumnName("job_id")
                .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(job => job.MappedCareerID)
                .HasColumnName("mapped_career_id");

                entity.Property(job => job.JobTitle)
                .HasColumnName("job_title")
                .HasMaxLength(200)
                .IsRequired();

                entity.Property(job => job.CompanyName)
                .HasColumnName("company_name")
                .HasMaxLength(200)
                .IsRequired();

                entity.Property(job => job.Location)
                .HasColumnName("location")
                .HasMaxLength(200);

                entity.Property(job => job.Description)
                .HasColumnName("description");

                entity.Property(job => job.QualificationRequirement)
                .HasColumnName("qualification_requirement");

                entity.Property(job => job.ExperienceMin)
                .HasColumnName("experience_min");

                entity.Property(job => job.ExperienceMax)
                .HasColumnName("experience_max");

                entity.Property(job => job.JobType)
                .HasColumnName("job_type")
                .HasMaxLength(50);

                entity.Property(job => job.ApplicationUrl)
                .HasColumnName("application_url")
                .HasMaxLength(500);

                entity.Property(job => job.SourceName)
                .HasColumnName("source_name")
                .HasMaxLength(100);

                entity.Property(job => job.SourceListingID)
                .HasColumnName("source_listing_id")
                .HasMaxLength(200);

                entity.Property(job => job.PostedAt)
                .HasColumnName("posted_at")
                .HasColumnType("timestamp with time zone");

                entity.Property(job => job.ExpiresAt)
                .HasColumnName("expires_at")
                .HasColumnType("timestamp with time zone");

                entity.Property(job => job.RetrievedAt)
                .HasColumnName("retrieved_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

                entity.HasOne(job => job.MappedCareer)
                .WithMany()
                .HasForeignKey(job => job.MappedCareerID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("jobs_mapped_career_id_fkey");

                entity.HasIndex(job => new { job.SourceName, job.SourceListingID})
                .IsUnique()
                .HasDatabaseName("jobs_source_name_source_listing_id_key");

                entity.HasIndex(job => job.MappedCareerID).HasDatabaseName("jobs_mapped_career_id_idx");
                entity.HasIndex(job => job.ExpiresAt).HasDatabaseName("jobs_expires_at_idx");

            });

            modelBuilder.Entity<JobSkill>(entity =>
            {
                entity.ToTable("job_skills");

                entity.HasKey(jobSkill => jobSkill.JobSkillID)
                .HasName("job_skills_pkey");

                entity.Property(jobSkill => jobSkill.JobSkillID)
                .HasColumnName("job_skill_id")
                .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(jobSkill => jobSkill.JobID)
                .HasColumnName("job_id");

                entity.Property(jobSkill => jobSkill.SkillID)
                .HasColumnName("skill_id");

                entity.Property(jobSkill => jobSkill.RequirementType)
                .HasColumnName("requirement_type")
                .HasMaxLength(50)
                .IsRequired();

                entity.Property(jobSkill => jobSkill.ImportanceLevel)
                .HasColumnName("importance_level");

                entity.HasOne(jobSkill => jobSkill.Job)
                .WithMany(job => job.JobSkills)
                .HasForeignKey(jobSkill => jobSkill.JobID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("job_skills_job_id_fkey");

                entity.HasOne(jobSkill => jobSkill.Skill)
                .WithMany()
                .HasForeignKey(jobSkill => jobSkill.SkillID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("job_skills_skill_id_fkey");

                entity.HasIndex(jobSkill => new { jobSkill.JobID, jobSkill.SkillID})
                .IsUnique()
                .HasDatabaseName("job_skills_job_id_skill_id_key");
                entity.HasIndex(jobSkill => jobSkill.SkillID).HasDatabaseName("job_skills_skill_id_idx");
            });

            modelBuilder.Entity<SavedJob>(entity =>
            {
                entity.ToTable("saved_jobs");

                entity.HasKey(savedJob => savedJob.SavedJobID)
                .HasName("saved_jobs_pkey");

                entity.Property(savedJob => savedJob.SavedJobID)
                .HasColumnName("saved_job_id")
                .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(savedJob => savedJob.UserID)
                .HasColumnName("user_id");

                entity.Property(savedJob => savedJob.JobID)
                .HasColumnName("job_id");

                entity.Property(savedJob => savedJob.SavedAt)
                .HasColumnName("saved_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

                entity.HasOne(savedJob => savedJob.User)
                .WithMany()
                .HasForeignKey(savedJob => savedJob.UserID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("saved_jobs_user_id_fkey");

                entity.HasOne(savedJob => savedJob.Job)
                .WithMany()
                .HasForeignKey(savedJob => savedJob.JobID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("saved_jobs_job_id_fkey");

                entity.HasIndex(savedJob => new { savedJob.UserID, savedJob.JobID})
                .IsUnique()
                .HasDatabaseName("saved_jobs_user_id_job_id_key");
                entity.HasIndex(savedJob => savedJob.JobID).HasDatabaseName("saved_jobs_job_id_idx");

            });

            modelBuilder.Entity<JobMatch>(entity =>
            {
                entity.ToTable("job_matches");

                entity.HasKey(match => match.JobMatchID)
                    .HasName("job_matches_pkey");

                entity.Property(match => match.JobMatchID)
                    .HasColumnName("job_match_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(match => match.UserID)
                    .HasColumnName("user_id");

                entity.Property(match => match.JobID)
                    .HasColumnName("job_id");

                entity.Property(match => match.SkillMatchScore)
                    .HasColumnName("skill_match_score")
                    .HasPrecision(5, 2);

                entity.Property(match => match.EducationMatchScore)
                    .HasColumnName("education_match_score")
                    .HasPrecision(5, 2);

                entity.Property(match => match.ExperienceMatchScore)
                    .HasColumnName("experience_match_score")
                    .HasPrecision(5, 2);

                entity.Property(match => match.CareerAlignmentScore)
                    .HasColumnName("career_alignment_score")
                    .HasPrecision(5, 2);

                entity.Property(match => match.OverallMatchScore)
                    .HasColumnName("overall_match_score")
                    .HasPrecision(5, 2);

                entity.Property(match => match.CalculatedAt)
                    .HasColumnName("calculated_at")
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("now()");

                entity.HasOne(match => match.User)
                    .WithMany()
                    .HasForeignKey(match => match.UserID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("job_matches_user_id_fkey");

                entity.HasOne(match => match.Job)
                    .WithMany(job => job.JobMatches)
                    .HasForeignKey(match => match.JobID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("job_matches_job_id_fkey");

                entity.HasIndex(match => new { match.UserID, match.JobID })
                    .IsUnique()
                    .HasDatabaseName("job_matches_user_id_job_id_key");

                entity.HasIndex(match => new { match.UserID, match.OverallMatchScore })
                    .HasDatabaseName("job_matches_user_score_idx");
            });

            modelBuilder.Entity<UserSkill>(entity =>
            {
                entity.ToTable("user_skills");

                entity.HasKey(userSkill => userSkill.UserSkillID).HasName("user_skills_pkey");

                entity.Property(userSkill => userSkill.UserSkillID).HasColumnName("user_skill_id").HasDefaultValueSql("gen_random_uuid()");

                entity.Property(userSkill => userSkill.UserID).HasColumnName("user_id");

                entity.Property(userSkill => userSkill.SkillID).HasColumnName("skill_id");

                entity.Property(userSkill => userSkill.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("missing").IsRequired();

                entity.Property(userSkill => userSkill.IsSelfReported).HasColumnName("is_self_reported").HasDefaultValue(false);

                entity.Property(userSkill => userSkill.IsCVDetected).HasColumnName("is_cv_detected").HasDefaultValue(false);

                entity.Property(userSkill => userSkill.IsKnowledgeVerified).HasColumnName("is_knowledge_verified").HasDefaultValue(false);

                entity.Property(userSkill => userSkill.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()").IsRequired();

                entity.HasIndex(userSkill => new
                {
                    userSkill.UserID,
                    userSkill.SkillID
                }).IsUnique().HasDatabaseName("user_skills_user_skill_unique");

                entity.HasIndex(userSkill => userSkill.SkillID).HasDatabaseName("user_skills_skill_id_idx");

                entity.HasOne(userSkill => userSkill.User).WithMany().HasForeignKey(userSkill => userSkill.UserID).OnDelete(DeleteBehavior.Cascade).HasConstraintName("user_skills_user_id_fkey");

                entity.HasOne(userSkill => userSkill.Skill).WithMany().HasForeignKey(userSkill => userSkill.SkillID).OnDelete(DeleteBehavior.Cascade).HasConstraintName("user_skills_skill_id_fkey");
            });

            modelBuilder.Entity<CV>(entity =>
            {
                entity.ToTable("cvs");

                entity.HasKey(cv => cv.CVID)
                    .HasName("cvs_pkey");

                entity.Property(cv => cv.CVID)
                    .HasColumnName("cv_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(cv => cv.UserID)
                    .HasColumnName("user_id");

                entity.Property(cv => cv.FileName)
                    .HasColumnName("file_name")
                    .IsRequired();

                entity.Property(cv => cv.FileLocation)
                    .HasColumnName("file_location")
                    .IsRequired();

                entity.Property(cv => cv.FileType)
                    .HasColumnName("file_type")
                    .IsRequired();

                entity.Property(cv => cv.IsCurrent)
                    .HasColumnName("is_current")
                    .HasDefaultValue(false);

                entity.Property(cv => cv.UploadedAt)
                    .HasColumnName("uploaded_at")
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.HasOne(cv => cv.User)
                    .WithMany(user => user.CVs)
                    .HasForeignKey(cv => cv.UserID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("cvs_user_id_fkey");
            });

            modelBuilder.Entity<CVAnalysis>(entity =>
            {
                entity.ToTable("cv_analyses");

                entity.HasKey(analysis => analysis.AnalysisID)
                    .HasName("cv_analyses_pkey");

                entity.Property(analysis => analysis.AnalysisID)
                    .HasColumnName("analysis_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(analysis => analysis.CVID)
                    .HasColumnName("cv_id");

                entity.Property(analysis => analysis.QualityScore)
                    .HasColumnName("quality_score")
                    .HasPrecision(5, 2);

                entity.Property(analysis => analysis.ATSScore)
                    .HasColumnName("ats_score")
                    .HasPrecision(5, 2);

                entity.Property(analysis => analysis.FormattingScore)
                    .HasColumnName("formatting_score")
                    .HasPrecision(5, 2);

                entity.Property(analysis => analysis.Strengths)
                    .HasColumnName("strengths")
                    .IsRequired();

                entity.Property(analysis => analysis.Weaknesses)
                    .HasColumnName("weaknesses")
                    .IsRequired();

                entity.Property(analysis => analysis.Recommendations)
                    .HasColumnName("recommendations")
                    .IsRequired();

                entity.Property(analysis => analysis.AnalysedAt)
                    .HasColumnName("analysed_at")
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.HasIndex(analysis => analysis.CVID)
                    .HasDatabaseName("cv_analyses_cv_id_idx");

                entity.HasOne(analysis => analysis.CV)
                    .WithMany(cv => cv.Analyses)
                    .HasForeignKey(analysis => analysis.CVID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("cv_analyses_cv_id_fkey");
            });

            modelBuilder.Entity<CVAnalysisSkill>(entity =>
            {
                entity.ToTable("cv_analysis_skills");

                entity.HasKey(link => link.CVAnalysisSkillID)
                    .HasName("cv_analysis_skills_pkey");

                entity.Property(link => link.CVAnalysisSkillID)
                    .HasColumnName("cv_analysis_skill_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(link => link.AnalysisID)
                    .HasColumnName("analysis_id");

                entity.Property(link => link.SkillID)
                    .HasColumnName("skill_id");

                entity.HasIndex(link => new
                {
                    link.AnalysisID,
                    link.SkillID
                })
                    .IsUnique()
                    .HasDatabaseName(
                        "cv_analysis_skills_analysis_skill_unique");

                entity.HasIndex(link => link.SkillID)
                    .HasDatabaseName(
                        "cv_analysis_skills_skill_id_idx");

                entity.HasOne(link => link.Analysis)
                    .WithMany(analysis => analysis.DetectedSkills)
                    .HasForeignKey(link => link.AnalysisID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName(
                        "cv_analysis_skills_analysis_id_fkey");

                entity.HasOne(link => link.Skill)
                    .WithMany()
                    .HasForeignKey(link => link.SkillID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName(
                        "cv_analysis_skills_skill_id_fkey");
            });

            modelBuilder.Entity<SavedResource>(entity =>
            {
                entity.ToTable("saved_resources");

                entity.HasKey(saved => saved.SavedResourceID)
                    .HasName("saved_resources_pkey");

                entity.Property(saved => saved.SavedResourceID)
                    .HasColumnName("saved_resource_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(saved => saved.UserID)
                    .HasColumnName("user_id");

                entity.Property(saved => saved.ResourceID)
                    .HasColumnName("resource_id")
                    .IsRequired();

                entity.Property(saved => saved.SavedAt)
                    .HasColumnName("saved_at")
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("now()");

                entity.HasOne(saved => saved.User)
                    .WithMany()
                    .HasForeignKey(saved => saved.UserID)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("saved_resources_user_id_fkey");

                entity.HasOne(saved => saved.Resource)
                    .WithMany()
                    .HasForeignKey(saved => saved.ResourceID)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("saved_resources_resource_id_fkey");

                entity.HasIndex(saved => new { saved.UserID, saved.ResourceID }).IsUnique()
                    .HasDatabaseName("saved_resources_user_resource_unique");
                entity.HasIndex(saved => saved.ResourceID).HasDatabaseName("saved_resources_resource_id_idx");
            });
            modelBuilder.Entity<SkillAssessment>(entity =>
            {
                entity.ToTable("skill_assessments");

                entity.HasKey(assessment => assessment.SkillAssessmentID)
                    .HasName("skill_assessments_pkey");

                entity.Property(assessment => assessment.SkillAssessmentID)
                    .HasColumnName("skill_assessment_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(assessment => assessment.UserID)
                    .HasColumnName("user_id");

                entity.Property(assessment => assessment.SkillID)
                    .HasColumnName("skill_id");

                entity.Property(assessment => assessment.RoadmapItemID)
                    .HasColumnName("roadmap_item_id");

                entity.Property(assessment => assessment.AttemptNumber)
                    .HasColumnName("attempt_number");

                entity.Property(assessment => assessment.ScorePercentage)
                    .HasColumnName("score_percentage")
                    .HasPrecision(5, 2);

                entity.Property(assessment => assessment.PerfectScore)
                    .HasColumnName("perfect_score")
                    .HasDefaultValue(false);

                entity.Property(assessment => assessment.GeneratedAt)
                    .HasColumnName("generated_at")
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("now()");

                entity.Property(assessment => assessment.CompletedAt)
                    .HasColumnName("completed_at")
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(assessment => assessment.User)
                    .WithMany()
                    .HasForeignKey(assessment => assessment.UserID)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("skill_assessments_user_id_fkey");

                entity.HasOne(assessment => assessment.Skill)
                    .WithMany()
                    .HasForeignKey(assessment => assessment.SkillID)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("skill_assessments_skill_id_fkey");

                entity.HasOne(assessment => assessment.RoadmapItem)
                    .WithMany()
                    .HasForeignKey(assessment => assessment.RoadmapItemID)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("skill_assessments_roadmap_item_id_fkey");

                entity.HasIndex(assessment => new { assessment.UserID, assessment.SkillID, assessment.AttemptNumber })
                    .IsUnique().HasFilter("attempt_number IS NOT NULL")
                    .HasDatabaseName("skill_assessments_user_skill_attempt_unique");
                entity.HasIndex(assessment => new { assessment.UserID, assessment.SkillID })
                    .HasDatabaseName("skill_assessments_user_skill_idx");
            });

            modelBuilder.Entity<AssessmentQuestion>(entity =>
            {
                entity.ToTable("assessment_questions");

                entity.HasKey(question => question.QuestionID)
                    .HasName("assessment_questions_pkey");

                entity.Property(question => question.QuestionID)
                    .HasColumnName("question_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(question => question.SkillAssessmentID)
                    .HasColumnName("skill_assessment_id");

                entity.Property(question => question.QuestionText)
                    .HasColumnName("question_text")
                    .IsRequired();

                entity.Property(question => question.Choices)
                    .HasColumnName("choices")
                    .HasColumnType("text[]")
                    .IsRequired();

                entity.Property(question => question.CorrectAnswer)
                    .HasColumnName("correct_answer")
                    .IsRequired();

                entity.Property(question => question.QuestionOrder)
                    .HasColumnName("question_order");

                entity.HasOne(question => question.SkillAssessment)
                    .WithMany(assessment => assessment.Questions)
                    .HasForeignKey(question => question.SkillAssessmentID)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("assessment_questions_skill_assessment_id_fkey");

                entity.HasIndex(question => new { question.SkillAssessmentID, question.QuestionOrder }).IsUnique()
                    .HasDatabaseName("assessment_questions_assessment_order_unique");
            });

            modelBuilder.Entity<AssessmentResponse>(entity =>
            {
                entity.ToTable("assessment_responses");

                entity.HasKey(response => response.ResponseID)
                    .HasName("assessment_responses_pkey");

                entity.Property(response => response.ResponseID)
                    .HasColumnName("response_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(response => response.QuestionID)
                    .HasColumnName("question_id");

                entity.Property(response => response.UserAnswer)
                    .HasColumnName("user_answer");

                entity.Property(response => response.IsCorrect)
                    .HasColumnName("is_correct");

                entity.HasOne(response => response.Question)
                    .WithMany(question => question.Responses)
                    .HasForeignKey(response => response.QuestionID)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("assessment_responses_question_id_fkey");

                entity.HasIndex(response => response.QuestionID).IsUnique()
                    .HasDatabaseName("assessment_responses_question_id_unique");
            });
        }
    }
}
