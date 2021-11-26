using System;

namespace FileCabinetApp
{
    /// <summary>
    /// CustomGenderValidator.
    /// </summary>
    public class GenderValidator : IRecordValidator<FileCabinetEventArgs, object>
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
        public object ValidateParameters(FileCabinetEventArgs parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException($"{parameters} argument is null");
            }

            if (parameters.Gender != this.male && parameters.Gender != this.female)
            {
                throw new ArgumentException("Person's gender unrecognized");
            }

            return parameters.Gender;
        }
    }
}
