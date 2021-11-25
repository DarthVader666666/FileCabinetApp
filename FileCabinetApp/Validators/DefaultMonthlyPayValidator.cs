using System;

namespace FileCabinetApp
{
    /// <summary>
    /// DefaultMonthlyPayValidator.
    /// </summary>
    public class DefaultMonthlyPayValidator : IRecordValidator<decimal, decimal>
    {
        /// <summary>
        /// Validates Monthly Pay.
        /// </summary>
        /// <param name="parameters">decimal to be validated.</param>
        /// <returns>validated decimal.</returns>
        public decimal ValidateParameters(decimal parameters)
        {
            if (parameters == 0)
            {
                throw new ArgumentException("Person needs to get paid.");
            }

            return parameters;
        }
    }
}
