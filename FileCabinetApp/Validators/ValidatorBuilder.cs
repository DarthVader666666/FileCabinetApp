using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Builds adjustible Composite Validator.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators = new List<IRecordValidator>();
        private readonly IConfiguration configBuilder = new ConfigurationBuilder().AddJsonFile("validation-rules.json", true, true).Build();

        /// <summary>
        /// Fills validators List with default validators.
        /// </summary>
        /// <returns>CompositeValidator instance.</returns>
        public CompositeValidator CreateDefault()
        {
            var defaultSetting = this.configBuilder.GetSection("default").Get<ValidationSettings.DefaultSettings>();

            this.ValidateFirstName(defaultSetting.FirstName.Min, defaultSetting.FirstName.Max);
            this.ValidateLastName(defaultSetting.LastName.Min, defaultSetting.LastName.Max);
            this.ValidateDateOfBirth(defaultSetting.DateOfBirth.From, defaultSetting.DateOfBirth.To);
            this.ValidateJobExperience(defaultSetting.JobExperience.Min, defaultSetting.JobExperience.Max);
            this.ValidateMonthlyPay(defaultSetting.MonthlyPay.Min, defaultSetting.MonthlyPay.Max);
            this.ValidateGender(defaultSetting.Gender.Male, defaultSetting.Gender.Female);

            return new CompositeValidator(this.validators);
        }

        /// <summary>
        /// Fills validators List with custom validators.
        /// </summary>
        /// <returns>CompositeValidator instance.</returns>
        public CompositeValidator CreateCustom()
        {
            var customSetting = this.configBuilder.GetSection("custom").Get<ValidationSettings.CustomSettings>();

            this.ValidateFirstName(customSetting.FirstName.Min, customSetting.FirstName.Max);
            this.ValidateLastName(customSetting.LastName.Min, customSetting.LastName.Max);
            this.ValidateDateOfBirth(customSetting.DateOfBirth.From, customSetting.DateOfBirth.To);
            this.ValidateJobExperience(customSetting.JobExperience.Min, customSetting.JobExperience.Max);
            this.ValidateMonthlyPay(customSetting.MonthlyPay.Min, customSetting.MonthlyPay.Max);
            this.ValidateGender(customSetting.Gender.Male, customSetting.Gender.Female);

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
        private void ValidateDateOfBirth(string from, string to)
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
