using System;

namespace FileCabinetApp
{
    /// <summary>
    /// CustomFirstNameValidator.
    /// </summary>
    public class FirstNameValidator : IRecordValidator<FileCabinetEventArgs, string>
    {
        private readonly int min;

        private readonly int max;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="min">Min count of letters in First Name.</param>
        /// <param name="max">Max count of lettes in FIrst Name.</param>
        public FirstNameValidator(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Validates FirstName.
        /// </summary>
        /// <param name="parameters">String to be validated.</param>
        /// <returns>Validated string.</returns>
        public string ValidateParameters(FileCabinetEventArgs parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException($"{parameters} argument is null");
            }

            if (string.IsNullOrWhiteSpace(parameters.FirstName) || parameters.FirstName.Length < this.min || parameters.FirstName.Length > this.max)
            {
                throw new ArgumentException("First Name is invalid");
            }

            return parameters.FirstName;
        }
    }
}
