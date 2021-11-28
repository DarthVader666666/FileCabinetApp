using System;
using Newtonsoft.Json;

namespace FileCabinetApp.ValidationSettings
{
    /// <summary>
    /// Contains default validation rules.
    /// </summary>
    [JsonObject("default")]
    public class DefaultSettings
    {
        /// <summary>
        /// Gets or sets FirstNameSettings value.
        /// </summary>
        /// <value></value>
        [JsonProperty("firstName")]
        public FirstNameSettings FirstName
        { get; set; }

        /// <summary>
        /// Gets or sets LastNameSettings value.
        /// </summary>
        /// <value></value>
        [JsonProperty("lastName")]
        public LastNameSettings LastName
        { get; set; }

        /// <summary>
        /// Gets or sets DateOfBirthSettings value.
        /// </summary>
        /// <value></value>
        [JsonProperty("dateOfBirth")]
        public DateOfBirthSettings DateOfBirth
        { get; set; }

        /// <summary>
        /// Gets or sets JobExperienceSettings value.
        /// </summary>
        /// <value></value>
        [JsonProperty("jobExperience")]
        public JobExperienceSettings JobExperience
        { get; set; }

        /// <summary>
        /// Gets or sets MonthlyPaySettings value.
        /// </summary>
        /// <value></value>
        [JsonProperty("monthlyPay")]
        public MonthlyPaySettings MonthlyPay
        { get; set; }

        /// <summary>
        /// Gets or sets GenderSettings value.
        /// </summary>
        /// <value></value>
        [JsonProperty("gender")]
        public GenderSettings Gender
        { get; set; }
    }
}
