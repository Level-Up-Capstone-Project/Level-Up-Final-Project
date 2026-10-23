using System.Text.Json;
using CapstoneProjectBE.Services;

namespace CapstoneProjectBE.ExternalServices.Assessments;

public sealed class N8nAssessmentQuestionGenerator(N8nAnalysisService workflow)
{
    public async Task<IReadOnlyList<AssessmentQuestionSeed>> GenerateAsync(
        Guid skillId,
        string skillName,
        N8nKnowledgeCheckCourse course,
        int attemptNumber,
        CancellationToken ct)
    {
        var requestId = Guid.NewGuid();
        var result = await workflow.GenerateKnowledgeCheckAsync(requestId, skillId, skillName, course, attemptNumber, ct);
        return Validate(result, requestId, skillId, attemptNumber);
    }

    public static IReadOnlyList<AssessmentQuestionSeed> Validate(JsonElement root, Guid requestId, Guid skillId, int attemptNumber)
    {
        static N8nAnalysisException Invalid() => new("The knowledge check response did not match the expected format.");
        static string Text(JsonElement item, string name, int max)
        {
            if (!item.TryGetProperty(name, out var value) || value.ValueKind != JsonValueKind.String) throw Invalid();
            var text = value.GetString()?.Trim();
            if (string.IsNullOrWhiteSpace(text) || text.Length > max) throw Invalid();
            return text;
        }
        if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty("success", out var success) || success.ValueKind != JsonValueKind.True ||
            !Guid.TryParse(Text(root, "requestId", 36), out var returnedRequest) || returnedRequest != requestId ||
            !Guid.TryParse(Text(root, "skillID", 36), out var returnedSkill) || returnedSkill != skillId ||
            !root.TryGetProperty("attemptNumber", out var attempt) || attempt.ValueKind != JsonValueKind.Number || !attempt.TryGetInt32(out var number) || number != attemptNumber ||
            !root.TryGetProperty("assessment", out var assessment) || assessment.ValueKind != JsonValueKind.Object ||
            !assessment.TryGetProperty("questions", out var questions) || questions.ValueKind != JsonValueKind.Array || questions.GetArrayLength() is < 3 or > 10)
            throw Invalid();
        var result = new List<AssessmentQuestionSeed>();
        foreach (var question in questions.EnumerateArray())
        {
            if (question.ValueKind != JsonValueKind.Object) throw Invalid();
            var type = Text(question, "questionType", 30);
            if (type != "MultipleChoice" || !question.TryGetProperty("choices", out var choicesElement) ||
                choicesElement.ValueKind != JsonValueKind.Array || choicesElement.GetArrayLength() is < 3 or > 6)
                throw Invalid();
            var choices = choicesElement.EnumerateArray().Select(choice =>
            {
                if (choice.ValueKind != JsonValueKind.String) throw Invalid();
                var value = choice.GetString()?.Trim();
                if (string.IsNullOrWhiteSpace(value) || value.Length > 500) throw Invalid();
                return value;
            }).ToArray();
            if (choices.Distinct(StringComparer.OrdinalIgnoreCase).Count() != choices.Length) throw Invalid();
            var answer = Text(question, "correctAnswer", 500);
            var canonicalAnswer = choices.SingleOrDefault(choice => string.Equals(choice, answer, StringComparison.OrdinalIgnoreCase));
            if (canonicalAnswer is null) throw Invalid();
            result.Add(new(Text(question, "questionText", 2000), choices, canonicalAnswer));
        }
        if (result.Select(question => question.QuestionText).Distinct(StringComparer.OrdinalIgnoreCase).Count() != result.Count) throw Invalid();
        return result;
    }
}
