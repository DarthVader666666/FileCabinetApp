using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Class provides custom validation algorythm.
    /// </summary>
    public sealed class DefaultValidator : IRecordValidator
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

            if (Array.FindAll(parameters.ToCharArray(), i => char.IsWhiteSpace(i)).Length > 0)
            {
                throw new ArgumentException("First Name shouldn't contain white spaces.");
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

            if (Array.FindAll(parameters.ToCharArray(), i => char.IsWhiteSpace(i)).Length > 0)
            {
                throw new ArgumentException("Last Name shouldn't contain white spaces.");
            }

            return parameters;
        }

        private static DateTime ValidateDateOfBirth(DateTime parameters)
        {
            if (parameters.CompareTo(new DateTime(1950, 1, 1)) < 0 || parameters.CompareTo(DateTime.Today) > 0)
            {
                throw new ArgumentException("Person is too old or from the future.");
            }

            return parameters;
        }

        private static short ValidateJobExperience(short parameters)
        {
            if (parameters < 2)
            {
                throw new ArgumentException("Job Experiens is too short.");
            }

            return parameters;
        }

        private static decimal ValidateMonthlyPay(decimal parameters)
        {
            if (parameters == 0)
            {
                throw new ArgumentException("Person needs to get paid.");
            }

            return parameters;
        }

        private static char ValidateGender(char parameters)
        {
            if (!char.IsLetter(parameters))
            {
                throw new ArgumentException("Gender parameter gets no letter.");
            }

            return parameters;
        }
    }
}
