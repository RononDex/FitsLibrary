using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.Deserialization;
using FitsLibrary.DocumentParts;
using FitsLibrary.Validation;
using FitsLibrary.Validation.Header;

namespace FitsLibrary
{
    public class FitsDocumentReader : IFitsDocumentReader
    {
        private IReadOnlyList<IValidator<Header>> headerValidators;
        private IHeaderDeserializer headerDeserializer;
        private IExtensionDeserializer extensionsDeserializer;

        private const int ChunkSize = 2880;

        public FitsDocumentReader()
        {
            UseValidatorsForReading();
            UseDeserializersForReading();
        }

        private void UseDeserializersForReading()
        {
            headerDeserializer = new HeaderDeserializer();
            extensionsDeserializer = new ExtensionDeserializer(headerDeserializer, contentDeserializer);
        }

        /// <summary>
        /// Used for Unit Testing
        /// </summary>
        /// <param name="headerDeserializer"></param>
        /// <param name="headerValidators"></param>
        /// <param name="contentDeserializer"></param>
        internal FitsDocumentReader(
                IHeaderDeserializer headerDeserializer,
                List<IValidator<Header>> headerValidators,
                IContentDeserializer contentDeserializer)
        {
            this.headerValidators = headerValidators;
            this.contentDeserializer = contentDeserializer;
            this.headerDeserializer = headerDeserializer;
        }

        private void UseValidatorsForReading()
        {
            headerValidators = new List<IValidator<Header>>
            {
                new KeywordsMustBeUniqueValidator(),
                new MandatoryHeaderEntriesValidator(),
            };
        }

        public Task<FitsDocument> ReadAsync(string filePath)
        {
            return ReadAsync(File.OpenRead(filePath));
        }

        public async Task<FitsDocument> ReadAsync(Stream inputStream)
        {
            var pipeReader = PipeReader.Create(
                    inputStream,
                    new StreamPipeReaderOptions(
                        bufferSize: ChunkSize,
                        minimumReadSize: ChunkSize))!;

            var headerResult = await headerDeserializer
                .DeserializeAsync(pipeReader)
                .ConfigureAwait(false);

            var validatorTasks = new List<Task<ValidationResult>>();
            foreach (var headerValidator in headerValidators)
            {
                validatorTasks.Add(headerValidator.ValidateAsync(headerResult.parsedHeader));
            }

            var validationResults = await Task.WhenAll(validatorTasks).ConfigureAwait(continueOnCapturedContext: false);

            foreach (var validationResult in validationResults)
            {
                if (!validationResult.ValidationSuccessful)
                {
                    throw new InvalidDataException($"Validation failed for the header of the fits file: {validationResult.ValidationFailureMessage}");
                }
            }

            (bool endOfStreamReached, Memory<object>? contentData)? contentResult = null;
            if (!headerResult.endOfStreamReached
                    && headerResult.parsedHeader.NumberOfAxisInMainContent > 0)
            {
                contentResult = await contentDeserializer
                    .DeserializeAsync(pipeReader, headerResult.parsedHeader)
                    .ConfigureAwait(false);
            }

            var extensions = new List<Extension>();
            var endOfStreamReached = contentResult?.endOfStreamReached == true || headerResult.endOfStreamReached;

            while (!endOfStreamReached)
            {
                var extensionResult = await extensionsDeserializer.DeserializeAsync(pipeReader).ConfigureAwait(false);
                endOfStreamReached = extensionResult.endOfStreamReached;
                extensions.Add(extensionResult.parsedExtension);
            }

            return new FitsDocument(
                header: headerResult.parsedHeader,
                content: contentResult?.contentData,
                extensions: extensions);
        }
    }
}
