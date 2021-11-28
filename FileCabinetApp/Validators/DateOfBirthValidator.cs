using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// CustomDateOfBirthValidator.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime from;

        private readonly DateTime to;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">Min year of birth.</param>
        /// <param name="to">Max year of birth.</param>
        public DateOfBirthValidator(string from, string to)
        {
            this.from = DateTime.Parse(from, CultureInfo.CreateSpecificCulture("en-GB"));
            this.to = DateTime.Parse(to, CultureInfo.CreateSpecificCulture("en-GB"));
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

            if (DateTime.Compare(parameters.DateOfBirth, this.from) < 0 || DateTime.Compare(parameters.DateOfBirth, this.to) > 0)
            {
                throw new ArgumentException("Person's age unappropriate.");
            }

            return parameters.DateOfBirth;
        }
    }
}
