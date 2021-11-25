using System;


namespace FileCabinetApp
{
    /// <summary>
    /// CustomGenderValidator.
    /// </summary>
    public class CustomGenderValidator : IRecordValidator<char, char>
    {
        /// <summary>
        /// Validates gender.
        /// </summary>
        /// <param name="parameters">Char to be validated.</param>
        /// <returns>Validated char.</returns>
        public char ValidateParameters(char parameters)
        {
            if (char.IsPunctuation(parameters))
            {
                throw new ArgumentException("Person's gender should be a letter M or F.");
            }

            return parameters;
        }
    }
}
