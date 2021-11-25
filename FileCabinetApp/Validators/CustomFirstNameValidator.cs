using System;

namespace FileCabinetApp
{
    /// <summary>
    /// CustomFirstNameValidator.
    /// </summary>
    public class CustomFirstNameValidator : IRecordValidator<string, string>
    {
        /// <summary>
        /// Validates FirstName.
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
                    throw new ArgumentException("First Name is invalid");
                }
            }

            if (Array.FindIndex(parameters.ToCharArray(), i => char.IsDigit(i)) >= 0)
            {
                throw new ArgumentException("First Name contains digits");
            }

            return parameters;
        }
    }
}
