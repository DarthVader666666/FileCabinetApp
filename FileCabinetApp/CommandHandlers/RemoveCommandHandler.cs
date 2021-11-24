using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles remove command.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Calls remove method or next handler.
        /// </summary>
        /// <param name="request">Provides command and parameters.</param>
        public override void Handle(AddCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException($"{request} is null");
            }

            if (request.Command.Equals("remove", StringComparison.InvariantCultureIgnoreCase))
            {
                Remove(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private static void Remove(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentException("Parameters argument is null");
            }

            if (parameters.Length == 0)
            {
                Console.WriteLine("Type record's id.");
                return;
            }

            int id;

            if (!int.TryParse(parameters, out id))
            {
                Console.WriteLine("Unrecognized number.");
                return;
            }

            Program.fileCabinetService.RemoveRecord(id);
        }
    }
}
