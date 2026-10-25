namespace CapstoneProjectBE.Models;

/// <summary>
/// Represents the persisted External Login domain entity.
/// </summary>
public class ExternalLogin
{
    public string Provider { get; set; } = "";
    public string Subject { get; set; } = "";
    public Guid UserID { get; set; }
    public User User { get; set; } = null!;
}
