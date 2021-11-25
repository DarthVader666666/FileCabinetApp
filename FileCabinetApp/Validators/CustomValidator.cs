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
            record.FirstName = new CustomFirstNameValidator().ValidateParameters(recordArgs.FirstName);
            record.LastName = new CustomLastNameValidator().ValidateParameters(recordArgs.LastName);
            record.DateOfBirth = new CustomDateOfBirthValidator().ValidateParameters(recordArgs.DateOfBirth);
            record.JobExperience = new CustomJobExperienceValidator().ValidateParameters(recordArgs.JobExperience);
            record.MonthlyPay = new CustomMonthlyPayValidator().ValidateParameters(recordArgs.MonthlyPay);
            record.Gender = new CustomGenderValidator().ValidateParameters(recordArgs.Gender);

            return record;
        }
    }
}
