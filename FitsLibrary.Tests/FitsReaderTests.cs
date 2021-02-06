using System;
using System.Collections.Generic;
using System.IO;
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
            var testee = new TesteeBuilder()
                .WithEmptyHeader()
                .WithEmptyContent()
                .WithOneHeaderValidatorReturningSuccess()
                .Build();

            var actual = await testee.ReadAsync(new MemoryStream());

            actual.Should().NotBeNull();
        }

        [Test]
        public async Task ReadAsync_WithOneValidatorReturningFailed_ThrowsExceptionAsync()
        {
            var testee = new TesteeBuilder()
                .WithEmptyHeader()
                .WithEmptyContent()
                .WithOneHeaderValidatorReturningFailure("whatever")
                .Build();

            Func<Task> action = async () => await testee.ReadAsync(new MemoryStream());

            action.Should().Throw<InvalidDataException>("Validation failed for the header of the fits file: whatever");
        }

        [Test]
        public async Task ReadAsync_WithOneValidatorReturningSuccessAndOneReturningFailure_ThrowsException()
        {
            var testee = new TesteeBuilder()
                .WithEmptyHeader()
                .WithEmptyContent()
                .WithOneHeaderValidatorReturningSuccess()
                .WithOneHeaderValidatorReturningFailure("whatever")
                .Build();

            Func<Task> action = async () => await testee.ReadAsync(new MemoryStream());

            action.Should().Throw<InvalidDataException>("Validation failed for the header of the fits file: whatever");
        }

        [Test]
        public async Task ReadAsync_WithDeserializerThrowingException_ThrowsException()
        {
            var testee = new TesteeBuilder()
                .WithEmptyHeader()
                .WithEmptyContent()
                .WithOneHeaderValidatorReturningSuccess()
                .WithDeserializerThrowingException()
                .Build();

            Func<Task> action = async () => await testee.ReadAsync(new MemoryStream());

            action.Should().Throw<Exception>();
        }

        [Test]
        public async Task ReadAsync_WithContentBeingEmpty_ReturnsFileWithNullContentAsync()
        {
            var testee = new TesteeBuilder()
                .WithOneHeaderValidatorReturningSuccess()
                .WithEmptyHeader()
                .WithEmptyContent()
                .Build();

            var actual = await testee.ReadAsync(new MemoryStream());

            actual.Should().NotBeNull();
            actual.Content.Should().BeNull();
        }

        private class TesteeBuilder
        {
            private readonly List<IValidator<Header>> headerValidators = new();
            private readonly Mock<IHeaderDeserializer> headerDeserializerMock = new(MockBehavior.Strict);
            private readonly Mock<IContentDeserializer> contentDeserializerMock = new(MockBehavior.Strict);

            public FitsDocumentReader Build()
            {
                return new FitsDocumentReader(
                    headerDeserializerMock.Object,
                    headerValidators,
                    contentDeserializerMock.Object);
            }

            public TesteeBuilder WithDeserializerThrowingException()
            {
                headerDeserializerMock
                    .Setup(mock => mock.DeserializeAsync(It.IsAny<Stream>()))
                    .Throws<Exception>();

                return this;
            }

            public TesteeBuilder WithOneHeaderValidatorReturningSuccess()
            {
                var validatorMock = new Mock<IValidator<Header>>();
                validatorMock
                    .Setup(mock => mock.ValidateAsync(It.IsAny<Header>()))
                    .Returns(Task.FromResult(new ValidationResult(validationSuccessful: true, validationFailureMessage: null)));

                headerValidators.Add(validatorMock.Object);

                return this;
            }

            public TesteeBuilder WithOneHeaderValidatorReturningFailure(string validationMessage)
            {
                var validatorMock = new Mock<IValidator<Header>>();
                validatorMock
                    .Setup(mock => mock.ValidateAsync(It.IsAny<Header>()))
                    .Returns(Task.FromResult(new ValidationResult(validationSuccessful: false, validationFailureMessage: validationMessage)));

                headerValidators.Add(validatorMock.Object);

                return this;
            }

            public TesteeBuilder WithEmptyHeader()
            {
                headerDeserializerMock
                    .Setup(mock => mock.DeserializeAsync(It.IsAny<Stream>()))
                    .ReturnsAsync(value: null);

                return this;
            }

            public TesteeBuilder WithEmptyContent()
            {
                contentDeserializerMock
                    .Setup(mock => mock.DeserializeAsync(It.IsAny<Stream>(), It.IsAny<Header>()))
                    .ReturnsAsync(value: null);

                return this;
            }
        }
    }
}
