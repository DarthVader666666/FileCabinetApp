using System;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles import command.
    /// </summary>
    public class ImportCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Calls import method or next handler.
        /// </summary>
        /// <param name="request">Provides command and parameters.</param>
        public override void Handle(AddCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException($"{request} is null");
            }

            if (request.Command.Equals("import", StringComparison.InvariantCultureIgnoreCase))
            {
                Import(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private static void Import(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentException("Parameters argument is null");
            }

            FileCabinetServiceSnapshot snapshot;
            StreamReader streamReader;
            string[] importArguments = parameters.Split(' ');
            string dataType = importArguments[0];
            string path = importArguments[1];

            if (importArguments.Length < 2)
            {
                Console.WriteLine("Wrong data type or command format.");
                return;
            }

            if (dataType.ToUpperInvariant() != "CSV" && dataType.ToUpperInvariant() != "XML")
            {
                Console.WriteLine("Wrong data type or command format.");
                return;
            }

            if (!importArguments[0].ToUpperInvariant().Equals(path[(Array.FindIndex(path.ToCharArray(), i => i.Equals('.')) + 1) ..].ToUpperInvariant()))
            {
                Console.WriteLine("Wrong import file extension.");
                return;
            }

            if (!File.Exists(path))
            {
                Console.WriteLine("File doesn't exist.");
                return;
            }

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

            try
            {
                switch (dataType.ToUpperInvariant())
                {
                    case "CSV":
                        snapshot = Program.fileCabinetService.MakeSnapshot();
                        streamReader = new StreamReader(fileStream);
                        snapshot.LoadFromCsv(streamReader);
                        Program.fileCabinetService.Restore(snapshot);
                        Console.WriteLine($"{snapshot.Records.Count} records were imported from {path}");
                        streamReader.Close();
                        break;
                    case "XML":
                        snapshot = Program.fileCabinetService.MakeSnapshot();
                        streamReader = new StreamReader(fileStream);
                        snapshot.LoadFromXml(streamReader);
                        Program.fileCabinetService.Restore(snapshot);
                        Console.WriteLine($"{snapshot.Records.Count} records were imported from {path}");
                        streamReader.Close();
                        break;
                }

                fileStream.Close();
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Can't open file {path} due it's access limitations.");
                fileStream.Close();
            }
        }
    }
}
