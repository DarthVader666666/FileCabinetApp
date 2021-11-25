using System;


namespace FileCabinetApp
{
    /// <summary>
    /// CustomJobExperienceValidator
    /// </summary>
    public class CustomJobExperienceValidator : IRecordValidator<short, short>
    {
        /// <summary>
        /// Validates job experience.
        /// </summary>
        /// <param name="parameters">Short int to be validated.</param>
        /// <returns>Validated short int.</returns>
        public short ValidateParameters(short parameters)
        {
            if (parameters > 40)
            {
                throw new ArgumentException("Person's ready to retire");
            }

            return parameters;
        }
    }
}
