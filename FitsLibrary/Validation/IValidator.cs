using System.Threading.Tasks;

namespace FitsLibrary.Validation
{
    public abstract class IValidator<T>
    {
        /// <summary>
        /// Validate the passed object, returns Option.None if no validation
        /// error was found
        /// </summary>
        /// <param name="objToValidate">The object which to validate</param>
        public abstract Task<ValidationResult> ValidateAsync(T objToValidate);
    }
}
