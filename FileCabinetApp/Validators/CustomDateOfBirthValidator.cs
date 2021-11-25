using System;


namespace FileCabinetApp
{
    /// <summary>
    /// CustomDateOfBirthValidator.
    /// </summary>
    public class CustomDateOfBirthValidator : IRecordValidator<DateTime, DateTime>
    {
        /// <summary>
        /// Validates dateOfBirth.
        /// </summary>
        /// <param name="parameters">Date to be validated.</param>
        /// <returns>Validated date.</returns>
        public DateTime ValidateParameters(DateTime parameters)
        {
            if (DateTime.Today.Year - parameters.Year < 18)
            {
                throw new ArgumentException("Person's too young");
            }

            return parameters;
        }
    }
}
