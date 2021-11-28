﻿using System;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles edit command.
    /// </summary>
    public class EditCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="service">FileCabinetService instance.</param>
        public EditCommandHandler(IFileCabinetService service)
            : base(service)
        {
            EditRecordEvent += this.fileCabinetService.EditRecord;
        }

        /// <summary>
        /// Edit record handler
        /// </summary>
        private static event EventHandler<FileCabinetEventArgs> EditRecordEvent;

        /// <summary>
        /// Calls edit method or next handler.
        /// </summary>
        /// <param name="request">Provides command and parameters.</param>
        public override void Handle(AddCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException($"{request} is null");
            }

            if (request.Command.Equals("edit", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Edit(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private void Edit(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("No number input.");
                return;
            }

            FileCabinetRecord record = new FileCabinetRecord();
            int id = int.Parse(parameters, CultureInfo.InvariantCulture);

            if (!this.fileCabinetService.RecordExists(id))
            {
                Console.WriteLine($"#{id} record not found");
                return;
            }

            record.Id = id;

            Program.InputRecordProperties(record);
            FileCabinetEventArgs recordArgs = new FileCabinetEventArgs(record);
            EditRecordEvent(null, recordArgs);
        }
    }
}
