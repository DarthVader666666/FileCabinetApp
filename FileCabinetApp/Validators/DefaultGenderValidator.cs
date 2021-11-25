using System;

namespace FileCabinetApp
{
    /// <summary>
    /// DefaultGenderValidator.
    /// </summary>
    public class DefaultGenderValidator : IRecordValidator<char, char>
    {
        /// <summary>
        /// Validates gender.
        /// </summary>
        /// <param name="parameters">Char to be validated.</param>
        /// <returns>Validated char.</returns>
        public char ValidateParameters(char parameters)
        {
            if (!char.IsLetter(parameters))
            {
                throw new ArgumentException("Gender parameter gets no letter.");
            }

            return parameters;
        }
    }
}
