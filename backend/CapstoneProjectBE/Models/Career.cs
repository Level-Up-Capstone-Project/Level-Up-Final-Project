namespace CapstoneProjectBE.Models
{
    public class Career
    {
        public Guid CareerID { get; set; }
        public string CareerName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string EducationRequirements { get; set; } = string.Empty;
        public ICollection<CareerSkill> CareerSkills { get; set; } = new List<CareerSkill>();
    }
}
