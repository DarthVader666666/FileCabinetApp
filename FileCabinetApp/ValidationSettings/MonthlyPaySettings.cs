using System;
using Newtonsoft.Json;

namespace FileCabinetApp.ValidationSettings
{
    /// <summary>
    /// Conatains monthlyPay validation settings.
    /// </summary>
    public class MonthlyPaySettings
    {
        /// <summary>
        /// Gets or sets min decimal value.
        /// </summary>
        /// <value>decimal.</value>
        [JsonProperty("min")]
        public decimal Min { get; set; }

        /// <summary>
        /// Gets or sets max decimal value.
        /// </summary>
        /// <value>decimal.</value>
        [JsonProperty("max")]
        public decimal Max { get; set; }
    }
}
