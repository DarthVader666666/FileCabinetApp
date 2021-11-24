using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles purge command.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
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
                Purge(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private static void Purge(string parameters)
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

            Program.fileCabinetService.PurgeFile();
        }
    }
}
