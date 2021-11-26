using System;

namespace FileCabinetApp
{
    /// <summary>
    /// CustomGenderValidator.
    /// </summary>
    public class GenderValidator : IRecordValidator<char, char>
    {
        private readonly char male;

        private readonly char female;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenderValidator"/> class.
        /// </summary>
        /// <param name="male">Male letter.</param>
        /// <param name="female">Female letter.</param>
        public GenderValidator(char male, char female)
        {
            this.male = male;
            this.female = female;
        }

        /// <summary>
        /// Validates gender.
        /// </summary>
        /// <param name="parameters">Char to be validated.</param>
        /// <returns>Validated char.</returns>
        public char ValidateParameters(char parameters)
        {
            if (parameters != this.male || parameters != this.female)
            {
                throw new ArgumentException("Person's gender unrecognized");
            }

            return parameters;
        }
    }
}
