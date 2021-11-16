using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface which provides read input validation strategy.
    /// </summary>
    public interface IReadInputValidator
    {
        /// <summary>
        /// Validates input string.
        /// </summary>
        /// <param name="inputData">Input string.</param>
        /// <returns>Validation result.</returns>
        Tuple<bool, string> ValidateString(string inputData);

        /// <summary>
        /// Validates input string.
        /// </summary>
        /// <param name="inputData">Input DateTime.</param>
        /// <returns>Validation result.</returns>
        Tuple<bool, string> ValidateDateTime(DateTime inputData);

        /// <summary>
        /// Validates input string.
        /// </summary>
        /// <param name="inputData">Input short.</param>
        /// <returns>Validation result.</returns>
        Tuple<bool, string> ValidateShort(short inputData);

        /// <summary>
        /// Validates input string.
        /// </summary>
        /// <param name="inputData">Input decimal.</param>
        /// <returns>Validation result.</returns>
        Tuple<bool, string> ValidateDecimal(decimal inputData);

        /// <summary>
        /// Validates input string.
        /// </summary>
        /// <param name="inputData">Input char.</param>
        /// <returns>Validation result.</returns>
        Tuple<bool, string> ValidateChar(char inputData);
    }
}
