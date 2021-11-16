using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Class provides custom validation algorythm.
    /// </summary>
    public sealed class CustomValidator : IRecordValidator, IReadInputValidator
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

            if (Array.FindIndex(recordArgs.FirstName.ToCharArray(), i => char.IsDigit(i)) >= 0)
            {
                throw new ArgumentException("First Name contains digits");
            }

            if (Array.FindIndex(recordArgs.LastName.ToCharArray(), i => char.IsDigit(i)) >= 0)
            {
                throw new ArgumentException("Last Name contains digits");
            }

            if (DateTime.Today.Year - recordArgs.DateOfBirth.Year < 18)
            {
                throw new ArgumentException("Person's too young");
            }

            if (recordArgs.JobExperience < 2)
            {
                throw new ArgumentException("Person's experience not enough");
            }

            if (recordArgs.MonthlyPay == 0)
            {
                throw new ArgumentException("Person's unemployed");
            }

            if (!char.IsLetter(recordArgs.Gender))
            {
                throw new ArgumentException("Not a letter");
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

            if (Array.FindAll(inputData.ToCharArray(), i => char.IsPunctuation(i)).Length > 0)
            {
                failureMessage = "Name should not contain any punctuation";
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

            if (inputData.CompareTo(new DateTime(1960, 1, 1)) < 0 || inputData.CompareTo(DateTime.Today) > 0)
            {
                failureMessage = "Year of birth must be at least 1960";
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

            if (inputData > 40)
            {
                failureMessage = "Job experience can't be that large";
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

            if (inputData == 0)
            {
                failureMessage = "Employee must earn at least something";
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

            if (char.IsPunctuation(inputData))
            {
                failureMessage = "Please, print letter M or F";
                successful = false;
            }

            return new Tuple<bool, string>(successful, failureMessage);
        }
    }
}
