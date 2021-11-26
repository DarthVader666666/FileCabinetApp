using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface which provides record validation strategy.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// File Record parameters validator.
        /// </summary>
        /// <param name="parameters">Parameter to be validated.</param>
        /// <returns>Validated parameter.</returns>
        public object ValidateParameters(FileCabinetEventArgs parameters);
    }
}
