using System.Linq;

namespace FitsLibrary.Validation.Header
{
    public class KeywordsMustBeUniqueValidator : BaseValidator<DocumentParts.Header>
    {
        public readonly string[] Exceptions = new[] { "COMMENT", string.Empty };

        public override ValidationResult Validate(DocumentParts.Header objToValidate)
        {
            var nonUniqueEntries = objToValidate
                .Entries
                .Where(entry => !Exceptions.Contains(entry.Key, System.StringComparer.Ordinal))
                .GroupBy(entries => entries.Key, System.StringComparer.Ordinal)
                .Where(group => group.Skip(1).Any())
                .Select(group => group.Key);

            if (nonUniqueEntries.Any())
            {
                return new ValidationResult(
                    validationSuccessful: false,
                    validationFailureMessage: $"Non unique KEYWORDS found. The header entries {string.Join(", ", nonUniqueEntries)} are contained more than once within the header.");
            }

            return new ValidationResult(
                validationSuccessful: true,
                validationFailureMessage: null);
        }
    }
}
