namespace CapstoneProjectBE.DTOs.Careers
{
    public class CareerDto
    {
        public Guid CareerID { get; set; }
        public string CareerName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string EducationRequirements { get; set; } = string.Empty;
    }
}
