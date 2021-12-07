using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles stat command.
    /// </summary>
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="service">FileCabinetServic instance.</param>
        public StatCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

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
                this.Stat(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private void Stat(string parameters)
        {
            if (parameters.Length > 0)
            {
                Console.WriteLine($"Unrecognized parameter {parameters}");
                return;
            }

            Tuple<int, int> countDeleted = this.Service.GetStat();
            Console.WriteLine($"{countDeleted.Item1} recods in list, {countDeleted.Item2} deleted.");
        }
    }
}
