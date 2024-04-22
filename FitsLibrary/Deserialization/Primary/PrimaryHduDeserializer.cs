
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Numerics;
using System.Threading.Tasks;
using FitsLibrary.Deserialization.Image;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.ImageData;
using FitsLibrary.Validation;

namespace FitsLibrary.Deserialization.Primary;

internal class PrimaryHduDeserializer<T>(IList<IValidator<Header>> validators) : IHduDeserializer<ImageDataContent<T>> where T : INumber<T>
{
    private IList<IValidator<Header>> HeaderValidators { get; } = validators;

    public async Task<(bool endOfStreamReached, HeaderDataUnit<ImageDataContent<T>> data)> DeserializeAsync(PipeReader reader, Header header)
    {
        await ValidateHeader(header).ConfigureAwait(false);

        var contentDeserializer = new ImageContentDeserializer<T>();

        var (endOfStreamReachedContent, parsedContent) = await contentDeserializer.DeserializeAsync(reader, header).ConfigureAwait(false);

        return (
                endOfStreamReached: endOfStreamReachedContent,
                data: new ImageHeaderDataUnit<T>(HeaderDataUnitType.PRIMARY, header, parsedContent));
    }

    private async Task ValidateHeader(Header header)
    {
        var validatorTasks = new List<Task<ValidationResult>>();
        foreach (var headerValidator in this.HeaderValidators)
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

    async Task<(bool endOfStreamReached, HeaderDataUnit data)> IHduDeserializer.DeserializeAsync(PipeReader reader, Header header)
    {
        var (endOfStreamReached, data) = await DeserializeAsync(reader, header);
        return (endOfStreamReached, data);
    }
}
