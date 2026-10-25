namespace CapstoneProjectBE.ExternalServices.Assessments
{
    /// <summary>
    /// Carries Assessment Question Seed values used by an external integration.
    /// </summary>
    public sealed record AssessmentQuestionSeed(
        string QuestionText,
        IReadOnlyList<string> Choices,
        string CorrectAnswer);
}
