namespace FitsLibrary.Validation
{
    public class ValidationResult
    {
        public ValidationResult(bool validationSuccessful, string? validationFailureMessage)
        {
            ValidationSucessful = validationSuccessful;
            ValidationFailureMessage = validationFailureMessage;
        }

        public string? ValidationFailureMessage { get; }

        public bool ValidationSucessful { get; }
    }
}
