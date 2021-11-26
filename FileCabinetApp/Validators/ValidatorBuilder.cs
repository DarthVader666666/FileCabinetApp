using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Builds adjustible Composite Validator.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators = new List<IRecordValidator>();

        /// <summary>
        /// Adds FirstNameValidator to validators List.
        /// </summary>
        /// <param name="min">Min string Length.</param>
        /// <param name="max">Max string Length.</param>
        /// <returns>ValidatorBuilder instance.</returns>
        public ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Adds LastNameValidator to validators List.
        /// </summary>
        /// <param name="min">Min string Length.</param>
        /// <param name="max">Max string Length.</param>
        /// <returns>ValidatorBuilder instance.</returns>
        public ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Adds DateOfBirthValidator to validators List.
        /// </summary>
        /// <param name="from">Min year of birth.</param>
        /// <param name="to">Max year of birth.</param>
        /// <returns>ValidatorBuilder instance.</returns>
        public ValidatorBuilder ValidateDateOfBirth(int from, int to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        /// <summary>
        /// Adds JobExperienceValidator to validators List.
        /// </summary>
        /// <param name="min">Min years of experience.</param>
        /// <param name="max">Max years of experience.</param>
        /// <returns>ValidatorBuilder instance.</returns>
        public ValidatorBuilder ValidateJobExperience(short min, short max)
        {
            this.validators.Add(new JobExperienceValidator(min, max));
            return this;
        }

        /// <summary>
        /// Adds MonthlyPayValidator to validators List.
        /// </summary>
        /// <param name="min">Min pay.</param>
        /// <param name="max">Max pay.</param>
        /// <returns>ValidatorBuilder instance.</returns>
        public ValidatorBuilder ValidateMonthlyPay(decimal min, decimal max)
        {
            this.validators.Add(new MonthlyPayValidator(min, max));
            return this;
        }

        /// <summary>
        /// Adds GenderValidator to validators List.
        /// </summary>
        /// <param name="male">Male definding symbol.</param>
        /// <param name="female">Female definding symbol.</param>
        /// <returns>ValidatorBuilder instance.</returns>
        public ValidatorBuilder ValidateGender(char male, char female)
        {
            this.validators.Add(new GenderValidator(male, female));
            return this;
        }

        /// <summary>
        /// Creates new Composite Validator.
        /// </summary>
        /// <returns>CompositeValidator.</returns>
        public CompositeValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
