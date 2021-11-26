using System;

namespace FileCabinetApp
{
    /// <summary>
    /// CustomJobExperienceValidator.
    /// </summary>
    public class JobExperienceValidator : IRecordValidator
    {
        private readonly short min;
        private readonly short max;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobExperienceValidator"/> class.
        /// </summary>
        /// <param name="min">Min job experience.</param>
        /// <param name="max">Max job experience.</param>
        public JobExperienceValidator(short min, short max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Validates job experience.
        /// </summary>
        /// <param name="parameters">Short int to be validated.</param>
        /// <returns>Validated short int.</returns>
        public object ValidateParameters(FileCabinetEventArgs parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException($"{parameters} argument is null");
            }

            if (parameters.JobExperience < this.min || parameters.JobExperience > this.max)
            {
                throw new ArgumentException("Person's ready to retire");
            }

            return parameters.JobExperience;
        }
    }
}
