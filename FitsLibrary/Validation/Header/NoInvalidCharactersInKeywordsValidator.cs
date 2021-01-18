using System.Linq;
using System.Threading.Tasks;

namespace FitsLibrary.Validation.Header
{
    public class NoInvalidCharactersInKeywordsValidator : IValidator<DocumentParts.Header>
    {
        public char[] ValidCharacters = new[]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '_', '⁻',
        };

        public override Task<ValidationResult> ValidateAsync(DocumentParts.Header objToValidate)
        {
            return Task.Run(() =>
            {
                var firstInvalidEntry = objToValidate.Entries
                    .FirstOrDefault(entry =>
                            entry.Key.Any(keyChar => !ValidCharacters.Contains(keyChar)));

                return firstInvalidEntry != null
                    ? new ValidationResult(
                            validationSuccessful: false,
                            validationFailureMessage: $"The header key \"{firstInvalidEntry.Key}\" contains not allowed characters!")
                    : new ValidationResult(
                        validationSuccessful: true,
                        validationFailureMessage: null);
            });
        }
    }
}
