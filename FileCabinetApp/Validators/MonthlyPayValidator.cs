using System;

namespace FileCabinetApp
{
    /// <summary>
    /// CustomMonthlyPayValidator.
    /// </summary>
    public class MonthlyPayValidator : IRecordValidator<decimal, decimal>
    {
        private readonly decimal min;

        private readonly decimal max;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonthlyPayValidator"/> class.
        /// </summary>
        /// <param name="min">Min monthly pay.</param>
        /// <param name="max">Max monthly pay.</param>
        public MonthlyPayValidator(decimal min, decimal max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Validates Monthly Pay.
        /// </summary>
        /// <param name="parameters">decimal to be validated.</param>
        /// <returns>validated decimal.</returns>
        public decimal ValidateParameters(decimal parameters)
        {
            if (parameters < this.min || parameters > this.max)
            {
                throw new ArgumentException("Person's pay doesn't fit conditions.");
            }

            return parameters;
        }
    }
}
