using System;

namespace FileCabinetApp
{
    /// <summary>
    /// CustomMonthlyPayValidator.
    /// </summary>
    public class CustomMonthlyPayValidator : IRecordValidator<decimal, decimal>
    {
        /// <summary>
        /// Validates Monthly Pay.
        /// </summary>
        /// <param name="parameters">decimal to be validated.</param>
        /// <returns>validated decimal.</returns>
        public decimal ValidateParameters(decimal parameters)
        {
            if (parameters > 5000)
            {
                throw new ArgumentException("Person's gets paid more than enough.");
            }

            return parameters;
        }
    }
}
