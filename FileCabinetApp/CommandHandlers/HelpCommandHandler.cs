using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles help command.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "select", "prints record list (use parameters to print separate columns. Ex.: select firstname, lastname)" },
            new string[] { "create", "creates new record" },
            new string[] { "export", "exports records into chosen file and format (csv or xml). Ex: export csv D:\\file.csv" },
            new string[] { "find", "finds records by specified parameter. Ex: find firstname \"Vadim\"" },
            new string[] { "import", "Imports records from csv or xml file. Ex: import csv d:\\file.csv" },
            new string[] { "delete", "Deletes specific record from record list (uses record field parameter, Ex: delete where id = '1')." },
            new string[] { "update", "Updates records using specific parameters (Ex: update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith')." },
            new string[] { "purge", "Deletes record from *.db file in FilesystemService." },
            new string[] { "stat", "Displays record list statistics." },
            new string[] { "insert", "Inserts new record using delegated parameters. Ex: insert (id, firstname, lastname, dateofbirth...) values ('1', 'John', 'Doe', '5/18/1986'...)." },
        };

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

            if (request.Command.Equals("help", StringComparison.InvariantCultureIgnoreCase))
            {
                PrintHelp(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                int index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (string[] helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}
