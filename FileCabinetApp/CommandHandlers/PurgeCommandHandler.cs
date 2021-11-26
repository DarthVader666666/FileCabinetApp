using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles purge command.
    /// </summary>
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="service">FileCabinetService instance.</param>
        public PurgeCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Calls purge method or next handler.
        /// </summary>
        /// <param name="request">Provides command and parameters.</param>
        public override void Handle(AddCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException($"{request} is null");
            }

            if (request.Command.Equals("purge", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Purge(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private void Purge(string parameters)
        {
            if (parameters.Length > 0)
            {
                Console.WriteLine("Unrecognized parameter after command 'purge'");
                return;
            }

            if (parameters.Length > 0)
            {
                Console.WriteLine($"Unrecognized parameter {parameters}");
                return;
            }

            this.fileCabinetService.PurgeFile();
        }
    }
}
