using System;

namespace FileCabinetApp
{
    /// <summary>
    /// CustomDateOfBirthValidator.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly int from;

        private readonly int to;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">Min year of birth.</param>
        /// <param name="to">Max year of birth.</param>
        public DateOfBirthValidator(int from, int to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Validates dateOfBirth.
        /// </summary>
        /// <param name="parameters">Date to be validated.</param>
        /// <returns>Validated date.</returns>
        public object ValidateParameters(FileCabinetEventArgs parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException($"{parameters} argument is null");
            }

            if (parameters.DateOfBirth.Year < this.from || parameters.DateOfBirth.Year > this.to)
            {
                throw new ArgumentException("Person's age unappropriate.");
            }

            return parameters.DateOfBirth;
        }
    }
}
