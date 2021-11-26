using System;

namespace FileCabinetApp
{
    /// <summary>
    /// CustomMonthlyPayValidator.
    /// </summary>
    public class MonthlyPayValidator : IRecordValidator
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
        public object ValidateParameters(FileCabinetEventArgs parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException($"{parameters} argument is null");
            }

            if (parameters.MonthlyPay < this.min || parameters.MonthlyPay > this.max)
            {
                throw new ArgumentException("Person's pay doesn't fit conditions.");
            }

            return parameters.MonthlyPay;
        }
    }
}
