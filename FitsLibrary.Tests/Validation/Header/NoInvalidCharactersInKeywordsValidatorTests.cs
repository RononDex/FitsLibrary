using System;
using System.Linq;
using System.Threading.Tasks;
using FitsLibrary.Validation.Header;
using FluentAssertions;
using NUnit.Framework;

namespace FitsLibrary.Tests.Validation.Header;

public class NoInvalidCharactersInKeywordsValidatorTests
{
    private static readonly char[] ValidCharacters = new[]
    {
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        '_', '⁻',
    };

    private static char[] InvalidCharacters =>
        Enumerable
            .Range(1, 255)
            .Select(Convert.ToChar)
            .Except(ValidCharacters)
            .ToArray();

    [Test]
    public async Task ValidateAsync_WithEmptyHeader_ValidationSucessfullAsync()
    {
        // Arrange
        var testee = new NoInvalidCharactersInKeywordsValidator();
        var header = new HeaderBuilder()
            .WithEmptyHeader()
            .Build();

        // Act
        var result = await testee.ValidateAsync(header);

        // Assert
        result.ValidationSuccessful.Should().Be(true);
        result.ValidationFailureMessage.Should().BeNull();
    }

    [Test]
    [TestCaseSource(nameof(InvalidCharacters))]
    public async Task ValidateAsync_WithInvalidCharacter_ValidacionFailsAsync(char invalidCharacter)
    {
        // Arrange
        var testee = new NoInvalidCharactersInKeywordsValidator();
        var header = new HeaderBuilder()
            .WithHeaderEntry(
                key: $"{invalidCharacter}",
                value: null,
                comment: null)
            .Build();

        // Act
        var result = await testee.ValidateAsync(header);

        // Assert
        result.ValidationSuccessful.Should().Be(false);
        result.ValidationFailureMessage.Should().Be($"The header key \"{invalidCharacter}\" contains not allowed characters!");
    }

    [Test]
    [TestCaseSource(nameof(ValidCharacters))]
    public async Task ValidateAsync_WithValidCharacter_ValidacionSuccessfulAsync(char invalidCharacter)
    {
        // Arrange
        var testee = new NoInvalidCharactersInKeywordsValidator();
        var header = new HeaderBuilder()
            .WithHeaderEntry(
                key: $"{invalidCharacter}",
                value: null,
                comment: null)
            .Build();

        // Act
        var result = await testee.ValidateAsync(header);

        // Assert
        result.ValidationSuccessful.Should().Be(true);
        result.ValidationFailureMessage.Should().BeNull();
    }
}
