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
    public class KeywordsMustBeUniqueValidatorTests
    {
        [Test]
        public async Task ReadAsync_WithOneValidatorReturningSuccess_ReturnsParsedFileAsync()
        {
            var testee = new TesteeBuilder()
                .WithOneHeaderValidatorReturningSuccess()
                .Build();

            var actual = await testee.ReadAsync(new MemoryStream());

            actual.Should().NotBeNull();
        }

        [Test]
        public async Task ReadAsync_WithOneValidatorReturningFailed_ThrowsException()
        {
            var testee = new TesteeBuilder()
                .WithOneHeaderValidatorReturningFailure("whatever")
                .Build();

            Func<Task> action = async () => await testee.ReadAsync(new MemoryStream());

            action.Should().Throw<InvalidDataException>("Validation failed for the header of the fits file: whatever");
        }

        [Test]
        public async Task ReadAsync_WithOneValidatorReturningSuccessAndOneReturningFailure_ThrowsException()
        {
            var testee = new TesteeBuilder()
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
                .WithOneHeaderValidatorReturningSuccess()
                .WithDeserializerThrowingException()
                .Build();

            Func<Task> action = async () => await testee.ReadAsync(new MemoryStream());

            action.Should().Throw<Exception>();
        }

        private class TesteeBuilder
        {
            private readonly List<IValidator<Header>> headerValidators = new List<IValidator<Header>>();
            private readonly Mock<IDeserializer<Header>> headerDeserializerMock = new Mock<IDeserializer<Header>>();

            public FitsDocumentReader Build()
            {
                return new FitsDocumentReader(headerDeserializerMock.Object, headerValidators);
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
                    .Setup(mock => mock.Validate(It.IsAny<Header>()))
                    .Returns(new ValidationResult(validationSuccessful: true, validationFailureMessage: null));

                headerValidators.Add(validatorMock.Object);

                return this;
            }

            public TesteeBuilder WithOneHeaderValidatorReturningFailure(string validationMessage)
            {
                var validatorMock = new Mock<IValidator<Header>>();
                validatorMock
                    .Setup(mock => mock.Validate(It.IsAny<Header>()))
                    .Returns(new ValidationResult(validationSuccessful: false, validationFailureMessage: validationMessage));

                headerValidators.Add(validatorMock.Object);

                return this;
            }
        }
    }
}