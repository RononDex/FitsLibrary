using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FitsLibrary.Deserialization;
using FitsLibrary.DocumentParts;
using FitsLibrary.Validation;
using FitsLibrary.Validation.Header;

namespace FitsLibrary
{
    public class FitsDocumentReader : IFitsDocumentReader
    {
        private IReadOnlyList<IValidator<Header>> HeaderValidators;
        private IDeserializer<Header> HeaderDeserializer;

        public FitsDocumentReader()
        {
            UseValidatorsForReading();
            UseDeserializersForReading();
        }

        private void UseDeserializersForReading()
        {
            HeaderDeserializer = new HeaderDeserializer();
        }

        /// <summary>
        /// Used for Unit Testing
        /// </summary>
        /// <param name="headerDeserializer"></param>
        /// <param name="headerValidators"></param>
        internal FitsDocumentReader(
                IDeserializer<Header> headerDeserializer,
                List<IValidator<Header>> headerValidators)
        {
            HeaderValidators = headerValidators;
            HeaderDeserializer = headerDeserializer;
        }

        private void UseValidatorsForReading()
        {
            HeaderValidators = new List<IValidator<Header>>
            {
                new KeywordsMustBeUniqueValidator(),
                new MandatoryHeaderEntriesValidator(),
            };
        }

        public async Task<FitsDocument> ReadAsync(Stream inputStream)
        {
            var header = await HeaderDeserializer
                .DeserializeAsync(inputStream)
                .ConfigureAwait(false);

            foreach (var headerValidator in HeaderValidators)
            {
                var validationResult = headerValidator.Validate(header);
                if (!validationResult.ValidationSucessful)
                {
                    throw new InvalidDataException($"Validation failed for the header of the fits file: {validationResult.ValidationFailureMessage}");
                }
            }

            return new FitsDocument(
                header: header);
        }

        public Task<FitsDocument> ReadAsync(string filePath)
        {
            return ReadAsync(File.OpenRead(filePath));
        }
    }
}
