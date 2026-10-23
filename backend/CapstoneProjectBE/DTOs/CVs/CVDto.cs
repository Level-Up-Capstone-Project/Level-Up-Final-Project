namespace CapstoneProjectBE.DTOs.CVs
{
    public class CVDto
    {
        public Guid CVID { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public bool IsCurrent { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
