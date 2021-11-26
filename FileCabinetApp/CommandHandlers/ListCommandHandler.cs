using System;
using System.Collections.ObjectModel;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles list command.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Record printer to be used in ListCommandHandler.
        /// </summary>
        private readonly Action<ReadOnlyCollection<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">FileCabinetService instance.</param>
        /// <param name="defaultPrinter">Delegate which invokes Default Printer method.</param>
        public ListCommandHandler(IFileCabinetService service, Action<ReadOnlyCollection<FileCabinetRecord>> defaultPrinter)
            : base(service)
        {
            if (defaultPrinter is null)
            {
                throw new ArgumentNullException($"{defaultPrinter} is null");
            }

            this.printer = defaultPrinter;
        }

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
                this.List(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private void List(string parameters)
        {
            if (parameters.Length > 0)
            {
                Console.WriteLine("Unrecognized parameter after command 'exit'");
                return;
            }

            this.printer.Invoke(this.fileCabinetService.GetRecords());
        }
    }
}
