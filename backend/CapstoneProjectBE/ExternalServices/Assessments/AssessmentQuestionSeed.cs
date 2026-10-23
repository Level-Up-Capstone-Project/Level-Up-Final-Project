namespace CapstoneProjectBE.ExternalServices.Assessments
{
    public sealed record AssessmentQuestionSeed(
        string QuestionText,
        IReadOnlyList<string> Choices,
        string CorrectAnswer);
}
