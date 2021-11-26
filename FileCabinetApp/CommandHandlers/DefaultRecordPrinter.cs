using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Defines default Print method.
    /// </summary>
    public class DefaultRecordPrinter : IRecordPrinter
    {
        /// <summary>
        /// Implements default Print method.
        /// </summary>
        /// <param name="records">File records to be printed.</param>
        public void Print(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException($"{records} is null");
            }

            if (records.Count == 0)
            {
                Console.WriteLine("Record list is empty.");
                return;
            }

            foreach (FileCabinetRecord fileCabinetRecord in records)
            {
                Console.WriteLine($"#{fileCabinetRecord.Id}, {fileCabinetRecord.FirstName}, {fileCabinetRecord.LastName}, " +
                    $"{fileCabinetRecord.DateOfBirth.Year}-{fileCabinetRecord.DateOfBirth.Month}-{fileCabinetRecord.DateOfBirth.Day}, " +
                    $"{fileCabinetRecord.JobExperience}, " + string.Format(CultureInfo.InvariantCulture, "{0:F2}", fileCabinetRecord.MonthlyPay) +
                    $", {fileCabinetRecord.Gender}");
            }
        }
    }
}
