using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Class provides custom validation algorythm.
    /// </summary>
    public sealed class DefaultValidator : IRecordValidator<FileCabinetEventArgs, FileCabinetRecord>
    {
        /// <summary>
        /// File Record parameters default validator.
        /// </summary>
        /// <param name="recordArgs">File recordArgs which parameters shall be validated.</param>
        /// <returns>Validated file record.</returns>
        public FileCabinetRecord ValidateParameters(FileCabinetEventArgs recordArgs)
        {
            if (recordArgs is null)
            {
                throw new ArgumentNullException(nameof(recordArgs), "recordArgs is null");
            }

            FileCabinetRecord record = new FileCabinetRecord();

            record.Id = recordArgs.Id;
            record.FirstName = new DefaultFirstNameValidator().ValidateParameters(recordArgs.FirstName);
            record.LastName = new DefaultLastNameValidator().ValidateParameters(recordArgs.LastName);
            record.DateOfBirth = new DefaultDateOfBirthValidator().ValidateParameters(recordArgs.DateOfBirth);
            record.JobExperience = new DefaultJobExperienceValidator().ValidateParameters(recordArgs.JobExperience);
            record.MonthlyPay = new DefaultMonthlyPayValidator().ValidateParameters(recordArgs.MonthlyPay);
            record.Gender = new DefaultGenderValidator().ValidateParameters(recordArgs.Gender);

            return record;
        }
    }
}
