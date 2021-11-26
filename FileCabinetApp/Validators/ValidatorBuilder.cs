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
        /// Fills validators List with default validators.
        /// </summary>
        /// <returns>ValidatorBuilder instance.</returns>
        public ValidatorBuilder DefaultValidation()
        {
            this.ValidateFirstName(2, 50);
            this.ValidateLastName(2, 50);
            this.ValidateDateOfBirth(1960, 1990);
            this.ValidateJobExperience(0, 20);
            this.ValidateMonthlyPay(20, 5000);
            this.ValidateGender('m', 'f');

            return this;
        }

        /// <summary>
        /// Fills validators List with custom validators.
        /// </summary>
        /// <returns>ValidatorBuilder instance.</returns>
        public ValidatorBuilder CustomValidation()
        {
            this.ValidateFirstName(1, 60);
            this.ValidateLastName(1, 60);
            this.ValidateDateOfBirth(1950, 2000);
            this.ValidateJobExperience(1, 30);
            this.ValidateMonthlyPay(0, 4000);
            this.ValidateGender('M', 'F');

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

        /// <summary>
        /// Adds FirstNameValidator to validators List.
        /// </summary>
        /// <param name="min">Min string Length.</param>
        /// <param name="max">Max string Length.</param>
        private void ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(min, max));
        }

        /// <summary>
        /// Adds LastNameValidator to validators List.
        /// </summary>
        /// <param name="min">Min string Length.</param>
        /// <param name="max">Max string Length.</param>
        private void ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(min, max));
        }

        /// <summary>
        /// Adds DateOfBirthValidator to validators List.
        /// </summary>
        /// <param name="from">Min year of birth.</param>
        /// <param name="to">Max year of birth.</param>
        private void ValidateDateOfBirth(int from, int to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
        }

        /// <summary>
        /// Adds JobExperienceValidator to validators List.
        /// </summary>
        /// <param name="min">Min years of experience.</param>
        /// <param name="max">Max years of experience.</param>
        private void ValidateJobExperience(short min, short max)
        {
            this.validators.Add(new JobExperienceValidator(min, max));
        }

        /// <summary>
        /// Adds MonthlyPayValidator to validators List.
        /// </summary>
        /// <param name="min">Min pay.</param>
        /// <param name="max">Max pay.</param>
        private void ValidateMonthlyPay(decimal min, decimal max)
        {
            this.validators.Add(new MonthlyPayValidator(min, max));
        }

        /// <summary>
        /// Adds GenderValidator to validators List.
        /// </summary>
        /// <param name="male">Male definding symbol.</param>
        /// <param name="female">Female definding symbol.</param>
        private void ValidateGender(char male, char female)
        {
            this.validators.Add(new GenderValidator(male, female));
        }
    }
}
