namespace CapstoneProjectBE.Models;

public class ExternalLogin
{
    public string Provider { get; set; } = "";
    public string Subject { get; set; } = "";
    public Guid UserID { get; set; }
    public User User { get; set; } = null!;
}
