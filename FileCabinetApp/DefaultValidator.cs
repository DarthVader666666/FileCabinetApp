using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Class provides custom validation algorythm.
    /// </summary>
    public sealed class DefaultValidator : IRecordValidator, IReadInputValidator
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

            if (string.IsNullOrWhiteSpace(recordArgs.FirstName) || recordArgs.FirstName.Length < 2 || recordArgs.FirstName.Length > 60)
            {
                if (recordArgs.FirstName is null)
                {
                    throw new ArgumentNullException($"{recordArgs.FirstName} argument is null");
                }
                else
                {
                    throw new ArgumentException("firstName is invalid");
                }
            }

            if (string.IsNullOrWhiteSpace(recordArgs.LastName) || recordArgs.LastName.Length < 2 || recordArgs.LastName.Length > 60)
            {
                if (recordArgs.LastName is null)
                {
                    throw new ArgumentNullException($"{recordArgs.LastName} argument is null");
                }
                else
                {
                    throw new ArgumentException("lastName is invalid");
                }
            }

            if (recordArgs.DateOfBirth.CompareTo(new DateTime(1950, 1, 1)) < 0 || recordArgs.DateOfBirth.CompareTo(DateTime.Today) > 0)
            {
                throw new ArgumentException("dateOfBirth is invalid");
            }

            if (recordArgs.JobExperience < 0)
            {
                throw new ArgumentException("jobExperience is invalid");
            }

            if (recordArgs.MonthlyPay < 0)
            {
                throw new ArgumentException("mothlyPay is invalid");
            }

            if (!(recordArgs.Gender == 'M' || recordArgs.Gender == 'F' || recordArgs.Gender == 'm' || recordArgs.Gender == 'f'))
            {
                throw new ArgumentException("Invalid gender");
            }

            return new FileCabinetRecord()
            {
                Id = recordArgs.Id,
                FirstName = recordArgs.FirstName,
                LastName = recordArgs.LastName,
                DateOfBirth = recordArgs.DateOfBirth,
                JobExperience = recordArgs.JobExperience,
                MonthlyPay = recordArgs.MonthlyPay,
                Gender = char.ToUpper(recordArgs.Gender, System.Globalization.CultureInfo.InvariantCulture),
            };
        }

        /// <summary>
        /// Validates input string.
        /// </summary>
        /// <param name="inputData">Input string.</param>
        /// <returns>Validation result.</returns>
        Tuple<bool, string> IReadInputValidator.ValidateString(string inputData)
        {
            bool successful = true;
            string failureMessage = string.Empty;

            if (Array.FindAll(inputData.ToCharArray(), i => char.IsWhiteSpace(i)).Length > 0)
            {
                failureMessage = "Name should not contain white spaces";
                successful = false;
            }

            return new Tuple<bool, string>(successful, failureMessage);
        }

        /// <summary>
        /// Validates input DateTime.
        /// </summary>
        /// <param name="inputData">Input DateTime.</param>
        /// <returns>Validation result.</returns>
        Tuple<bool, string> IReadInputValidator.ValidateDateTime(DateTime inputData)
        {
            bool successful = true;
            string failureMessage = string.Empty;

            if (inputData.CompareTo(new DateTime(1950, 1, 1)) < 0 || inputData.CompareTo(DateTime.Today) > 0)
            {
                failureMessage = "DateOfBirth is unoppropriate";
                successful = false;
            }

            return new Tuple<bool, string>(successful, failureMessage);
        }

        /// <summary>
        /// Validates input short.
        /// </summary>
        /// <param name="inputData">Input short.</param>
        /// <returns>Validation result.</returns>
        Tuple<bool, string> IReadInputValidator.ValidateShort(short inputData)
        {
            bool successful = true;
            string failureMessage = string.Empty;

            if (inputData < 0)
            {
                failureMessage = "Number shouldn't be negative";
                successful = false;
            }

            return new Tuple<bool, string>(successful, failureMessage);
        }

        /// <summary>
        /// Validates input decimal.
        /// </summary>
        /// <param name="inputData">Input decimal.</param>
        /// <returns>Validation result.</returns>
        Tuple<bool, string> IReadInputValidator.ValidateDecimal(decimal inputData)
        {
            bool successful = true;
            string failureMessage = string.Empty;

            if (inputData < 0)
            {
                failureMessage = "Number shouldn't be negative";
                successful = false;
            }

            return new Tuple<bool, string>(successful, failureMessage);
        }

        /// <summary>
        /// Validates input char.
        /// </summary>
        /// <param name="inputData">Input char.</param>
        /// <returns>Validation result.</returns>
        Tuple<bool, string> IReadInputValidator.ValidateChar(char inputData)
        {
            bool successful = true;
            string failureMessage = string.Empty;

            if (!char.IsLetter(inputData))
            {
                failureMessage = "Please, print letter M or F";
                successful = false;
            }

            return new Tuple<bool, string>(successful, failureMessage);
        }
    }
}
