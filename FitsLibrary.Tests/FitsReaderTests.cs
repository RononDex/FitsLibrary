using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.Deserialization;
using FitsLibrary.DocumentParts;
using FitsLibrary.Validation;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace FitsLibrary.Tests
{
    public class FitsReaderTests
    {
        [Test]
        public async Task ReadAsync_WithOneValidatorReturningSuccess_ReturnsParsedFileAsync()
        {
            var testee = new TesteeBuilder<float>()
                .WithEmptyHeader()
                .WithEmptyContent()
                .WithOneHeaderValidatorReturningSuccess()
                .Build();

            var actual = await testee.ReadAsync(new MemoryStream());

            actual.Should().NotBeNull();
        }

        [Test]
        public void ReadAsync_WithOneValidatorReturningFailed_ThrowsExceptionAsync()
        {
            var testee = new TesteeBuilder<float>()
                .WithEmptyHeader()
                .WithEmptyContent()
                .WithOneHeaderValidatorReturningFailure("whatever")
                .Build();

            Func<Task> action = async () => await testee.ReadAsync(new MemoryStream());

            action.Should().ThrowAsync<InvalidDataException>("Validation failed for the header of the fits file: whatever");
        }

        [Test]
        public void ReadAsync_WithOneValidatorReturningSuccessAndOneReturningFailure_ThrowsException()
        {
            var testee = new TesteeBuilder<float>()
                .WithEmptyHeader()
                .WithEmptyContent()
                .WithOneHeaderValidatorReturningSuccess()
                .WithOneHeaderValidatorReturningFailure("whatever")
                .Build();

            Func<Task> action = async () => await testee.ReadAsync(new MemoryStream());

            action.Should().ThrowAsync<InvalidDataException>("Validation failed for the header of the fits file: whatever");
        }

        [Test]
        public void ReadAsync_WithDeserializerThrowingException_ThrowsException()
        {
            var testee = new TesteeBuilder<float>()
                .WithEmptyHeader()
                .WithEmptyContent()
                .WithOneHeaderValidatorReturningSuccess()
                .WithDeserializerThrowingException()
                .Build();

            Func<Task> action = async () => await testee.ReadAsync(new MemoryStream());

            action.Should().ThrowAsync<Exception>();
        }

        [Test]
        public async Task ReadAsync_WithContentBeingEmpty_ReturnsFileWithNullContentAsync()
        {
            var testee = new TesteeBuilder<float>()
                .WithOneHeaderValidatorReturningSuccess()
                .WithEmptyHeader()
                .WithEmptyContent()
                .Build();

            var actual = await testee.ReadAsync(new MemoryStream());

            actual.Should().NotBeNull();
            actual.RawData.Should().BeNull();
        }

        private class TesteeBuilder<T> where T : INumber<T>
        {
            private readonly List<IValidator<Header>> headerValidators = new();
            private readonly Mock<IHeaderDeserializer> headerDeserializerMock = new(MockBehavior.Strict);
            private readonly Mock<IContentDeserializer<T>> contentDeserializerMock = new(MockBehavior.Strict);

            public FitsDocumentReader<T> Build()
            {
                return new FitsDocumentReader<T>(
                    headerDeserializerMock.Object,
                    headerValidators,
                    contentDeserializerMock.Object);
            }

            public TesteeBuilder<T> WithDeserializerThrowingException()
            {
                headerDeserializerMock
                    .Setup(mock => mock.DeserializeAsync(It.IsAny<PipeReader>()))
                    .Throws<Exception>();

                return this;
            }

            public TesteeBuilder<T> WithOneHeaderValidatorReturningSuccess()
            {
                var validatorMock = new Mock<IValidator<Header>>();
                validatorMock
                    .Setup(mock => mock.ValidateAsync(It.IsAny<Header>()))
                    .Returns(Task.FromResult(new ValidationResult(validationSuccessful: true, validationFailureMessage: null)));

                headerValidators.Add(validatorMock.Object);

                return this;
            }

            public TesteeBuilder<T> WithOneHeaderValidatorReturningFailure(string validationMessage)
            {
                var validatorMock = new Mock<IValidator<Header>>();
                validatorMock
                    .Setup(mock => mock.ValidateAsync(It.IsAny<Header>()))
                    .Returns(Task.FromResult(new ValidationResult(validationSuccessful: false, validationFailureMessage: validationMessage)));

                headerValidators.Add(validatorMock.Object);

                return this;
            }

            public TesteeBuilder<T> WithEmptyHeader()
            {
                headerDeserializerMock
                    .Setup(mock => mock.DeserializeAsync(It.IsAny<PipeReader>()))
                    .ReturnsAsync(value: (endOfStreamReached: true, parsedHeader: null));

                return this;
            }

            public TesteeBuilder<T> WithEmptyContent()
            {
                contentDeserializerMock
                    .Setup(mock => mock.DeserializeAsync(It.IsAny<PipeReader>(), It.IsAny<Header>()))
                    .ReturnsAsync(value: (endOfStreamReached: true, contentData: null));

                return this;
            }
        }
    }
}
