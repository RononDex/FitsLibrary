using System.Linq;

namespace FitsLibrary.Validation.Header
{
    public class KeywordsMustBeUniqueValidator : BaseValidator<DocumentParts.Header>
    {
        public override ValidationResult Validate(DocumentParts.Header header)
        {
            var nonUniqueEntries = header
                .Entries
                .GroupBy(entries => entries.Key, System.StringComparer.Ordinal)
                .Where(group => group.Skip(1).Any())
                .Select(group => group.Key);

            if (nonUniqueEntries.Any())
            {
                return new ValidationResult(
                    validationSucessful: false,
                    validationFailureMessage: $"Non unique KEYWORDS found. The header entries {string.Join(", ", nonUniqueEntries)} are contained more than once within the header.");
            }

            return new ValidationResult(
                validationSucessful: true,
                validationFailureMessage: null);
        }
    }
}
