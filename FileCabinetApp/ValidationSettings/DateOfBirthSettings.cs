using System;
using Newtonsoft.Json;

namespace FileCabinetApp.ValidationSettings
{
    /// <summary>
    /// Contains dateOfBirth validation settings.
    /// </summary>
    public class DateOfBirthSettings
    {
        /// <summary>
        /// Gets or sets min date.
        /// </summary>
        /// <value></value>
        [JsonProperty("from")]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets max date.
        /// </summary>
        /// <value></value>
        [JsonProperty("to")]
        public string To { get; set; }
    }
}
