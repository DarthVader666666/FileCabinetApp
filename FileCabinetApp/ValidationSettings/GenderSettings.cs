using System;
using Newtonsoft.Json;

namespace FileCabinetApp.ValidationSettings
{
    /// <summary>
    /// Contaons gender validation settings.
    /// </summary>
    public class GenderSettings
    {
        /// <summary>
        /// Gets or sets male symbol.
        /// </summary>
        /// <value>char.</value>
        [JsonProperty("male")]
        public char Male { get; set; }

        /// <summary>
        /// Gets or sets female symbol.
        /// </summary>
        /// <value>char.</value>
        [JsonProperty("female")]
        public char Female { get; set; }
    }
}
