namespace CapstoneProjectBE.Models;

/// <summary>
/// Represents the persisted Transcript Analysis domain entity.
/// </summary>
public class TranscriptAnalysis
{
    public Guid AnalysisID { get; set; }
    public Guid UserID { get; set; }
    public DateTime AnalysedAt { get; set; }
    public string ResultJson { get; set; } = "";
    public User User { get; set; } = null!;
}
