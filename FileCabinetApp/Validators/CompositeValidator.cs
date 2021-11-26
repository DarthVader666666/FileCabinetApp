using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Composite Validator. Contains Default and Custom.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">Validators to be used for records.</param>
        public CompositeValidator(List<IRecordValidator> validators)
        {
            this.validators = validators;
        }

        /// <summary>
        /// Validates file record.
        /// </summary>
        /// <param name="parameters">Record to be validated.</param>
        /// <returns>Validated record.</returns>
        public object ValidateParameters(FileCabinetEventArgs parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException($"{parameters} is null.");
            }

            FileCabinetRecord record = new FileCabinetRecord();

            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(parameters);
            }

            record.Id = parameters.Id;
            record.FirstName = parameters.FirstName;
            record.LastName = parameters.LastName;
            record.DateOfBirth = parameters.DateOfBirth;
            record.JobExperience = parameters.JobExperience;
            record.MonthlyPay = parameters.MonthlyPay;
            record.Gender = parameters.Gender;

            return record;
        }
    }
}
