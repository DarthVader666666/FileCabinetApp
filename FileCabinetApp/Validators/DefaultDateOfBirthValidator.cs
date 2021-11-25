using System;

namespace FileCabinetApp
{
    /// <summary>
    /// DefaultDateOfBirthValidator.
    /// </summary>
    public class DefaultDateOfBirthValidator : IRecordValidator<DateTime, DateTime>
    {
        /// <summary>
        /// Validates Date of birth.
        /// </summary>
        /// <param name="parameters">DateTime to be validated.</param>
        /// <returns>Validated DateTime.</returns>
        public DateTime ValidateParameters(DateTime parameters)
        {
            if (parameters.CompareTo(new DateTime(1950, 1, 1)) < 0 || parameters.CompareTo(DateTime.Today) > 0)
            {
                throw new ArgumentException("Person is too old or from the future.");
            }

            return parameters;
        }
    }
}
