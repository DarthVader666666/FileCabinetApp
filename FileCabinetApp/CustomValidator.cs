using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Class provides custom validation algorythm.
    /// </summary>
    public sealed class CustomValidator : IRecordValidator
    {
        /// <summary>
        /// File Record parameters default validator.
        /// </summary>
        /// <param name="recordArgs">File recordArgs which parameters shall be validated.</param>
        /// <returns>Validated file record.</returns>
        FileCabinetRecord IRecordValidator.ValidateParameters(FileCabinetEventArgs recordArgs)
        {
            if (recordArgs is null)
            {
                throw new ArgumentNullException(nameof(recordArgs), "recordArgs is null");
            }

            FileCabinetRecord record = new FileCabinetRecord();

            record.Id = recordArgs.Id;
            record.FirstName = ValidateFirstName(recordArgs.FirstName);
            record.LastName = ValidateLastName(recordArgs.LastName);
            record.DateOfBirth = ValidateDateOfBirth(recordArgs.DateOfBirth);
            record.JobExperience = ValidateJobExperience(recordArgs.JobExperience);
            record.MonthlyPay = ValidateMonthlyPay(recordArgs.MonthlyPay);
            record.Gender = ValidateGender(recordArgs.Gender);

            return record;
        }

        private static string ValidateFirstName(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters) || parameters.Length < 2 || parameters.Length > 60)
            {
                if (parameters is null)
                {
                    throw new ArgumentNullException($"{parameters} argument is null");
                }
                else
                {
                    throw new ArgumentException("First Name is invalid");
                }
            }

            if (Array.FindIndex(parameters.ToCharArray(), i => char.IsDigit(i)) >= 0)
            {
                throw new ArgumentException("First Name contains digits");
            }

            return parameters;
        }

        private static string ValidateLastName(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters) || parameters.Length < 2 || parameters.Length > 60)
            {
                if (parameters is null)
                {
                    throw new ArgumentNullException($"{parameters} argument is null");
                }
                else
                {
                    throw new ArgumentException("Last Name is invalid");
                }
            }

            if (Array.FindIndex(parameters.ToCharArray(), i => char.IsDigit(i)) >= 0)
            {
                throw new ArgumentException("Last Name contains digits");
            }

            return parameters;
        }

        private static DateTime ValidateDateOfBirth(DateTime parameters)
        {
            if (DateTime.Today.Year - parameters.Year < 18)
            {
                throw new ArgumentException("Person's too young");
            }

            return parameters;
        }

        private static short ValidateJobExperience(short parameters)
        {
            if (parameters > 40)
            {
                throw new ArgumentException("Person's ready to retire");
            }

            return parameters;
        }

        private static decimal ValidateMonthlyPay(decimal parameters)
        {
            if (parameters > 5000)
            {
                throw new ArgumentException("Person's gets paid more than enough.");
            }

            return parameters;
        }

        private static char ValidateGender(char parameters)
        {
            if (char.IsPunctuation(parameters))
            {
                throw new ArgumentException("Person's gender should be a letter M or F.");
            }

            return parameters;
        }
    }
}
