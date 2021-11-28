using System;
using Newtonsoft.Json;

namespace FileCabinetApp.ValidationSettings
{
    /// <summary>
    /// Contains jobExperience validation settings.
    /// </summary>
    public class JobExperienceSettings
    {
        /// <summary>
        /// Gets or sets min short value.
        /// </summary>
        /// <value></value>
        [JsonProperty("min")]
        public short Min { get; set; }

        /// <summary>
        /// Gets or sets max short value.
        /// </summary>
        /// <value></value>
        [JsonProperty("max")]
        public short Max { get; set; }
    }
}
