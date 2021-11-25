using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// DefaultFirstNameValidator.
    /// </summary>
    public class DefaultFirstNameValidator : IRecordValidator<string, string>
    {
        /// <summary>
        /// Validates First Name.
        /// </summary>
        /// <param name="parameters">String to be Validated.</param>
        /// <returns>Validated String.</returns>
        public string ValidateParameters(string parameters)
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
    }
}
