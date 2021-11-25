using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles remove command.
    /// </summary>
    public class RemoveCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="service">FileCabinetService instance.</param>
        public RemoveCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

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
                this.Remove(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private void Remove(string parameters)
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

            this.fileCabinetService.RemoveRecord(id);
        }
    }
}
