using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles exit command.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private readonly Action<bool> stopProgram;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="breakAll">Action delegate invokes stop program.</param>
        public ExitCommandHandler(Action<bool> breakAll)
        {
            if (breakAll is null)
            {
                throw new ArgumentNullException($"{breakAll} is null");
            }

            this.stopProgram = breakAll;
        }

        /// <summary>
        /// Calls exit method.
        /// </summary>
        /// <param name="request">Provides command and parameters.</param>
        public override void Handle(AddCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException($"{request} is null");
            }

            if (request.Command.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Exit(request.Parameters);
            }
        }

        private void Exit(string parameters)
        {
            if (parameters.Length > 0)
            {
                Console.WriteLine("Unrecognized parameter after command 'exit'");
                return;
            }

            Console.WriteLine("Exiting an application...");
            this.stopProgram.Invoke(false);
        }
    }
}
