using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Class provides custom validation algorythm.
    /// </summary>
    public sealed class CustomValidator : IRecordValidator<FileCabinetEventArgs, FileCabinetRecord>
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
            record.FirstName = new FirstNameValidator(1, 60).ValidateParameters(recordArgs.FirstName);
            record.LastName = new LastNameValidator(1, 60).ValidateParameters(recordArgs.LastName);
            record.DateOfBirth = new DateOfBirthValidator(1950, 2000).ValidateParameters(recordArgs.DateOfBirth);
            record.JobExperience = new JobExperienceValidator(1, 20).ValidateParameters(recordArgs.JobExperience);
            record.MonthlyPay = new MonthlyPayValidator(0, 1000).ValidateParameters(recordArgs.MonthlyPay);
            record.Gender = new GenderValidator('M', 'F').ValidateParameters(recordArgs.Gender);

            return record;
        }
    }
}
