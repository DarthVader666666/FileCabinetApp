using System;
using Newtonsoft.Json;

namespace FileCabinetApp.ValidationSettings
{
    /// <summary>
    /// Contains Last Name validation settings.
    /// </summary>
    public class LastNameSettings
    {
        /// <summary>
        /// Gets or sets min lastName length.
        /// </summary>
        /// <value>int.</value>
        [JsonProperty("min")]
        public int Min { get; set; }

        /// <summary>
        /// Gets or sets min lastName length.
        /// </summary>
        /// <value>int.</value>
        [JsonProperty("max")]
        public int Max { get; set; }
    }
}
