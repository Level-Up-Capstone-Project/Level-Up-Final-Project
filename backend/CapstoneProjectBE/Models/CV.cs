namespace CapstoneProjectBE.Models
{
    public class CV
    {
        public Guid CVID { get; set; }
        public Guid UserID { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileLocation { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public bool IsCurrent { get; set; }
        public DateTime UploadedAt { get; set; }
        public User User { get; set; } = null!;
        public ICollection<CVAnalysis> Analyses { get; set; } = new List<CVAnalysis>();
    }
}
