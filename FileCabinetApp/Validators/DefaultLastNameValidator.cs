using System;

namespace FileCabinetApp
{
    /// <summary>
    /// DefaultLastNameValidator.
    /// </summary>
    public class DefaultLastNameValidator : IRecordValidator<string, string>
    {
        /// <summary>
        /// Validates First Name.
        /// </summary>
        /// <param name="parameters">String to be validated.</param>
        /// <returns>Validated string.</returns>
        public string ValidateParameters(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters) || parameters.Length < 2 || parameters.Length > 60)
            {
                if (parameters is null)
                {
                    throw new ArgumentNullException($"{parameters} argument is null");
                }
                else
                {
                    throw new ArgumentException("Last Name is invalid");
                }
            }

            if (Array.FindAll(parameters.ToCharArray(), i => char.IsWhiteSpace(i)).Length > 0)
            {
                throw new ArgumentException("Last Name shouldn't contain white spaces.");
            }

            return parameters;
        }
    }
}
