using System;

namespace FileCabinetApp
{
    /// <summary>
    /// CustomLastNameValidator.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private readonly int min;

        private readonly int max;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="min">Min count of letters in Last Name.</param>
        /// <param name="max">Max count of lettes in Last Name.</param>
        public LastNameValidator(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Validates Last Name.
        /// </summary>
        /// <param name="parameters">String to be validated.</param>
        /// <returns>Validated string.</returns>
        public object ValidateParameters(FileCabinetEventArgs parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException($"{parameters} argument is null");
            }

            if (string.IsNullOrWhiteSpace(parameters.LastName) || parameters.LastName.Length < this.min || parameters.LastName.Length > this.max)
            {
                throw new ArgumentException("Last Name is invalid");
            }

            return parameters.LastName;
        }
    }
}
