using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles list command.
    /// </summary>
    public class ListCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Calls list method or next handler.
        /// </summary>
        /// <param name="request">Provides command and parameters.</param>
        public override void Handle(AddCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException($"{request} is null");
            }

            if (request.Command.Equals("list", StringComparison.InvariantCultureIgnoreCase))
            {
                List(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private static void List(string parameters)
        {
            if (parameters.Length > 0)
            {
                Console.WriteLine("Unrecognized parameter after command 'exit'");
                return;
            }

            ReadOnlyCollection<FileCabinetRecord> recordList = Program.fileCabinetService.GetRecords();

            if (recordList.Count == 0)
            {
                Console.WriteLine("Record list is empty.");
                return;
            }

            foreach (FileCabinetRecord fileCabinetRecord in recordList)
            {
                Console.WriteLine($"#{fileCabinetRecord.Id}, {fileCabinetRecord.FirstName}, {fileCabinetRecord.LastName}, " +
                    $"{fileCabinetRecord.DateOfBirth.Year}-{fileCabinetRecord.DateOfBirth.Month}-{fileCabinetRecord.DateOfBirth.Day}, " +
                    $"{fileCabinetRecord.JobExperience}, " + string.Format(CultureInfo.InvariantCulture, "{0:F2}", fileCabinetRecord.MonthlyPay) +
                    $", {fileCabinetRecord.Gender}");
            }
        }
    }
}
