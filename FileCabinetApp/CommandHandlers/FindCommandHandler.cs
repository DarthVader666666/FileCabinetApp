﻿using System;
using System.Collections.ObjectModel;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles find command.
    /// </summary>
    public class FindCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="service">FileCabinetService instance.</param>
        public FindCommandHandler(IFileCabinetService service)
        {
            this.fileCabinetService = service;
        }

        /// <summary>
        /// Calls find method or next handler.
        /// </summary>
        /// <param name="request">Provides command and parameters.</param>
        public override void Handle(AddCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException($"{request} is null");
            }

            if (request.Command.Equals("find", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Find(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private void Find(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentException("Parameters argument is null");
            }

            string[] searchArguments = parameters.Split(' ');
            ReadOnlyCollection<FileCabinetRecord> fileCabinetRecords;
            searchArguments[0] = searchArguments[0].ToUpperInvariant();

            if (searchArguments[1][0] != '"' && searchArguments[1][^1] != '"')
            {
                Console.WriteLine("! Search value must be in quotes. Abort find command.");
                return;
            }

            switch (searchArguments[0])
            {
                case "FIRSTNAME": fileCabinetRecords = this.fileCabinetService.FindByFirstName(searchArguments[1][1..^1]); break;
                case "LASTNAME": fileCabinetRecords = this.fileCabinetService.FindByLastName(searchArguments[1][1..^1]); break;
                case "DATEOFBIRTH": fileCabinetRecords = this.fileCabinetService.FindByDateOfBirth(searchArguments[1][1..^1]); break;
                default: Console.WriteLine("! Wrong search parameter."); return;
            }

            foreach (FileCabinetRecord record in fileCabinetRecords)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, " +
                    $"{record.DateOfBirth.Year}-{record.DateOfBirth.Month}-{record.DateOfBirth.Day}, " +
                    $"{record.JobExperience}, {record.MonthlyPay}, {record.Gender}");
            }
        }
    }
}
