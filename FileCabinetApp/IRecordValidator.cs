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
        /// <param name="recordArgs">File recordArgs which parameters shall be validated.</param>
        /// <returns>Validated file record.</returns>
        public FileCabinetRecord ValidateParameters(FileCabinetEventArgs recordArgs);
    }
}
