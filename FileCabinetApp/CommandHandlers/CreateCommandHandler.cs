using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles create command.
    /// </summary>
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">FileCabinetServic instance.</param>
        public CreateCommandHandler(IFileCabinetService service)
            : base(service)
        {
            CreateRecordEvent += this.fileCabinetService.CreateRecord;
        }

        /// <summary>
        /// Create record handler
        /// </summary>
        private static event EventHandler<FileCabinetEventArgs> CreateRecordEvent;

        /// <summary>
        /// Calls method or next handler.
        /// </summary>
        /// <param name="request">Provides command and parameters.</param>
        public override void Handle(AddCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException($"{request} is null");
            }

            if (request.Command.Equals("create", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Create(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private void Create(string parameters)
        {
            if (parameters.Length > 0)
            {
                Console.WriteLine("Unrecognized parameter after command 'create'");
                return;
            }

            FileCabinetRecord record = new FileCabinetRecord();
            Program.InputRecordProperties(record);
            record.Id = this.fileCabinetService.GetMaxId() + 1;
            FileCabinetEventArgs recordArgs = new FileCabinetEventArgs(record);
            CreateRecordEvent(null, recordArgs);
        }
    }
}
