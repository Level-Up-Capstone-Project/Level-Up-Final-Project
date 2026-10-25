namespace CapstoneProjectBE.Services
{
    /// <summary>
    /// Coordinates User Skill Status Resolver application behavior.
    /// </summary>
    public class UserSkillStatusResolver
    {
        public const string Missing = "missing";
        public const string Partial = "partial";
        public const string Have = "have";

        public static string Resolve(
            bool isSelfReported,
            bool isCVDetected,
            bool isKnowledgeVerified,
            bool hasAssessmentAttempt = false)
        {
            if(isKnowledgeVerified || isCVDetected)
            {
                return Have;
            }

            if(isSelfReported || hasAssessmentAttempt)
            {
                return Partial;
            }

            return Missing;
        }

        public static string ToApiStatus(string databaseStatus)
        {
            return databaseStatus.ToLowerInvariant() switch
            {
                Have => "Have",
                Partial => "Partial",
                _ => "Missing"
            };

        }

        public static decimal ToMatchWeight(string databaseStatus)
        {
            return databaseStatus.ToLowerInvariant() switch
            {
                Have => 1.0m,
                Partial => 0.5m,
                _ => 0.0m
            };
        }
    }
}
