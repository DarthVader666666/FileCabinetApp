using System;
using Newtonsoft.Json;

namespace FileCabinetApp.ValidationSettings
{
    /// <summary>
    /// Contains custom validation rules.
    /// </summary>
    [JsonObject("custom")]
    public class CustomSettings
    {
        /// <summary>
        /// Gets or sets FirstNameSettings value.
        /// </summary>
        /// <value>FirstNameSettings.</value>
        [JsonProperty("firstName")]
        public FirstNameSettings FirstName
        { get; set; }

        /// <summary>
        /// Gets or sets LastNameSettings value.
        /// </summary>
        /// <value>LastNameSettings.</value>
        [JsonProperty("lastName")]
        public LastNameSettings LastName
        { get; set; }

        /// <summary>
        /// Gets or sets DateOfBirthSettings value.
        /// </summary>
        /// <value>DateOfBirthSettings.</value>
        [JsonProperty("dateOfBirth")]
        public DateOfBirthSettings DateOfBirth
        { get; set; }

        /// <summary>
        /// Gets or sets JobExperienceSettings value.
        /// </summary>
        /// <value>JobExperienceSettings.</value>
        [JsonProperty("jobExperience")]
        public JobExperienceSettings JobExperience
        { get; set; }

        /// <summary>
        /// Gets or sets MonthlyPaySettings value.
        /// </summary>
        /// <value>MonthlyPaySettings.</value>
        [JsonProperty("monthlyPay")]
        public MonthlyPaySettings MonthlyPay
        { get; set; }

        /// <summary>
        /// Gets or sets GenderSettings value.
        /// </summary>
        /// <value>v.</value>
        [JsonProperty("gender")]
        public GenderSettings Gender
        { get; set; }
    }
}
