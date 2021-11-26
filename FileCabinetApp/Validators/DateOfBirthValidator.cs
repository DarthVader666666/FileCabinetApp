using System;

namespace FileCabinetApp
{
    /// <summary>
    /// CustomDateOfBirthValidator.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator<DateTime, DateTime>
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
        public DateTime ValidateParameters(DateTime parameters)
        {
            if (parameters.Year < this.from || parameters.Year > this.to)
            {
                throw new ArgumentException("Person's age unappropriate.");
            }

            return parameters;
        }
    }
}
