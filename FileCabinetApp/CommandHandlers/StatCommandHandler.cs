using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles stat command.
    /// </summary>
    public class StatCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Calls stat method or next handler.
        /// </summary>
        /// <param name="request">Provides command and parameters.</param>
        public override void Handle(AddCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException($"{request} is null");
            }

            if (request.Command.Equals("stat", StringComparison.InvariantCultureIgnoreCase))
            {
                Stat(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private static void Stat(string parameters)
        {
            if (parameters.Length > 0)
            {
                Console.WriteLine($"Unrecognized parameter {parameters}");
                return;
            }

            Tuple<int, int> countDeleted = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{countDeleted.Item1} recods in list, {countDeleted.Item2} deleted.");
        }
    }
}
