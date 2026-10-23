using CapstoneProjectBE.DTOs.Assessments;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface IAssessmentService
    {
        Task<AssessmentDto> StartAsync(Guid userId, StartAssessmentDto request, CancellationToken cancellationToken);
        Task<AssessmentDto?> GetByIdAsync(Guid userId, Guid assessmentId, CancellationToken cancellationToken);
        Task<AssessmentResultDto?> SubmitAsync(Guid userId, Guid assessmentId, SubmitAssessmentDto request, CancellationToken cancellationToken);
        Task<IReadOnlyList<AssessmentHistoryDto>> GetHistoryAsync(Guid userId, Guid skillId, CancellationToken cancellationToken);
    }
}
