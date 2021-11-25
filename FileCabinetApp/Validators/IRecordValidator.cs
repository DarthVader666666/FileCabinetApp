using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface which provides record validation strategy.
    /// </summary>
    /// <typeparam name="T1">Parameter to be validated.</typeparam>
    /// <typeparam name="T2">Validated parameter to be returned.</typeparam>
    public interface IRecordValidator<in T1, out T2>
    {
        /// <summary>
        /// File Record parameters validator.
        /// </summary>
        /// <param name="parameters">Parameter to be validated.</param>
        /// <returns>Validated parameter.</returns>
        public T2 ValidateParameters(T1 parameters);
    }
}
