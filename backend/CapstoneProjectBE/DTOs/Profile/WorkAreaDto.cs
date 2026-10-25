using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Profile
{
    /// <summary>
    /// Carries Work Area data between API clients and application services.
    /// </summary>
    public class WorkAreaDto
    {
        [Required]
        [StringLength(200)]
        public string PlaceId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Label { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[A-Z]{2}$")]
        public string CountryCode { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(country|region)$")]
        public string Kind { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        [MaxLength(10)]
        public List<string> Names { get; set; } = new();
    }
}
