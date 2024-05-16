
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FitsLibrary.Validation.Header;

internal class HeaderValidator(IList<IValidator<DocumentParts.Header>> validators)
{
    private IList<IValidator<DocumentParts.Header>> Validators { get; } = validators;

    public async Task ValidateHeader(DocumentParts.Header header)
    {
        var validatorTasks = new List<Task<ValidationResult>>();
        foreach (var headerValidator in this.Validators)
        {
            validatorTasks.Add(headerValidator.ValidateAsync(header));
        }

        var validationResults = await Task
            .WhenAll(validatorTasks)
            .ConfigureAwait(continueOnCapturedContext: false);

        foreach (var validationResult in validationResults)
        {
            if (!validationResult.ValidationSuccessful)
            {
                throw new InvalidDataException($"Validation failed for the header of the fits file: {validationResult.ValidationFailureMessage}");
            }
        }
    }
}
