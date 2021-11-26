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
            record.FirstName = new FirstNameValidator(2, 50).ValidateParameters(recordArgs.FirstName);
            record.LastName = new LastNameValidator(2, 50).ValidateParameters(recordArgs.LastName);
            record.DateOfBirth = new DateOfBirthValidator(1960, 1990).ValidateParameters(recordArgs.DateOfBirth);
            record.JobExperience = new JobExperienceValidator(0, 10).ValidateParameters(recordArgs.JobExperience);
            record.MonthlyPay = new MonthlyPayValidator(20, 2000).ValidateParameters(recordArgs.MonthlyPay);
            record.Gender = new GenderValidator('m', 'f').ValidateParameters(recordArgs.Gender);

            return record;
        }
    }
}
