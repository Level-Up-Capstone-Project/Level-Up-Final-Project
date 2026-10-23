namespace CapstoneProjectBE.DTOs.Dashboard
{
    public class DashboardDto
    {
        public DashboardProfileDto? Profile { get; set; }
        public DashboardCareerDto? TargetCareer { get; set; }
        public decimal? CareerReadinessScore { get; set; }
        public decimal? CVQualityScore { get; set; }
        public decimal? SkillMatchPercentage { get; set; }
        public decimal RoadmapProgressPercentage { get; set; }
        public DashboardCVDto? CurrentCV { get; set; }
        public int SavedJobsCount { get; set; }
        public int SavedResourcesCount { get; set; }
    }

    public class DashboardProfileDto
    {
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public bool ProfileCompleted { get; set; }
    }

    public class DashboardCareerDto
    {
        public Guid CareerID { get; set; }
        public string CareerName { get; set; } = string.Empty;
    }

    public class DashboardCVDto
    {
        public Guid CVID { get; set; }
        public string FileName { get; set; } = string.Empty;
    }
}
