using System;
using Newtonsoft.Json;

namespace FileCabinetApp.ValidationSettings
{
    /// <summary>
    /// Contains First Name validation settings.
    /// </summary>
    public class FirstNameSettings
    {
        /// <summary>
        /// Gets or sets min firstName length.
        /// </summary>
        /// <value></value>
        [JsonProperty("min")]
        public int Min { get; set; }

        /// <summary>
        /// Gets or sets max firstName length.
        /// </summary>
        /// <value></value>
        [JsonProperty("max")]
        public int Max { get; set; }
    }
}
