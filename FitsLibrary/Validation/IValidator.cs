namespace FitsLibrary.Validation
{
    public abstract class IValidator<T>
    {
        /// <summary>
        /// Validate the passed object, returns Option.None if no validation
        /// error was found
        /// </summary>
        /// <param name="objToValidate">The object which to validate</param>
        public abstract ValidationResult Validate(T objToValidate);
    }
}
