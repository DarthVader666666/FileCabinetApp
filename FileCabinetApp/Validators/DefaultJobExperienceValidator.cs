using System;

namespace FileCabinetApp
{
    /// <summary>
    /// DefaultJobExperienceValidator.
    /// </summary>
    public class DefaultJobExperienceValidator : IRecordValidator<short, short>
    {
        /// <summary>
        /// Validates job experience.
        /// </summary>
        /// <param name="parameters">Short int to be validated.</param>
        /// <returns>Validated short int.</returns>
        public short ValidateParameters(short parameters)
        {
            if (parameters < 2)
            {
                throw new ArgumentException("Job Experiens is too short.");
            }

            return parameters;
        }
    }
}
