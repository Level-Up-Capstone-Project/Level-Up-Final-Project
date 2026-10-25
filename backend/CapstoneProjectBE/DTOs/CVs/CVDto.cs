namespace CapstoneProjectBE.DTOs.CVs
{
    /// <summary>
    /// Carries CV data between API clients and application services.
    /// </summary>
    public class CVDto
    {
        public Guid CVID { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public bool IsCurrent { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
