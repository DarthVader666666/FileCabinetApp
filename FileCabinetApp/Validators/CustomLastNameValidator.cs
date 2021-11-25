using System;

namespace FileCabinetApp
{
    /// <summary>
    /// CustomLastNameValidator.
    /// </summary>
    public class CustomLastNameValidator : IRecordValidator<string, string>
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

            if (Array.FindIndex(parameters.ToCharArray(), i => char.IsDigit(i)) >= 0)
            {
                throw new ArgumentException("Last Name contains digits");
            }

            return parameters;
        }
    }
}
