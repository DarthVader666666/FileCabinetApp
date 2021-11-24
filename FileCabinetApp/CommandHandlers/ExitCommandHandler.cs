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
                Exit(request.Parameters);
            }
        }

        private static void Exit(string parameters)
        {
            if (parameters.Length > 0)
            {
                Console.WriteLine("Unrecognized parameter after command 'exit'");
                return;
            }

            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
        }
    }
}
