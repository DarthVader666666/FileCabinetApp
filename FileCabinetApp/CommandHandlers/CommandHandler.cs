using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    public class CommandHandler : CommandHandlerBase
    {
        public CommandHandler()
        {
            CreateRecordEvent += Program.fileCabinetService.CreateRecord;
            EditRecordEvent += Program.fileCabinetService.EditRecord;
        }

        public void Handle(AppCommandRequest request)
        {

        }



        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        /// <summary>
        /// Create record handler
        /// </summary>
        private static event EventHandler<FileCabinetEventArgs> CreateRecordEvent;

        /// <summary>
        /// Edit record handler
        /// </summary>
        private static event EventHandler<FileCabinetEventArgs> EditRecordEvent;

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "list", "prints record list" },
            new string[] { "create", "creates new record" },
            new string[] { "export", "exports records into chosen file and format (csv or xml). Ex: export csv D:\\file.csv" },
            new string[] { "find", "finds records by specified parameter. Ex: find firstname \"Vadim\"" },
            new string[] { "import", "Imports records from csv or xml file. Ex: import csv d:\\file.csv" },
            new string[] { "remove", "Removes specific record from record list (uses id parameter)." },
            new string[] { "purge", "Deletes record from *.db file in FilesystemService." },
            new string[] { "stat", "Displays record list statistics." },
        };

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

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
        }

        private static void Create(string parameters)
        {
            FileCabinetRecord record = new FileCabinetRecord();
            Program.InputRecordProperties(record);
            record.Id = Program.fileCabinetService.GetMaxId() + 1;
            FileCabinetEventArgs recordArgs = new FileCabinetEventArgs(record);
            CreateRecordEvent(null, recordArgs);
            Console.WriteLine($"Record #{record.Id} is created.");
        }

        private static void List(string parameters)
        {
            ReadOnlyCollection<FileCabinetRecord> recordList = Program.fileCabinetService.GetRecords();

            if (recordList.Count == 0)
            {
                Console.WriteLine("Record list is empty.");
                return;
            }

            foreach (FileCabinetRecord fileCabinetRecord in recordList)
            {
                Console.WriteLine($"#{fileCabinetRecord.Id}, {fileCabinetRecord.FirstName}, {fileCabinetRecord.LastName}, " +
                    $"{fileCabinetRecord.DateOfBirth.Year}-{fileCabinetRecord.DateOfBirth.Month}-{fileCabinetRecord.DateOfBirth.Day}, " +
                    $"{fileCabinetRecord.JobExperience}, " + string.Format(CultureInfo.InvariantCulture, "{0:F2}", fileCabinetRecord.MonthlyPay) +
                    $", {fileCabinetRecord.Gender}");
            }
        }

        private static void Edit(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("No number input.");
                return;
            }

            FileCabinetRecord record = new FileCabinetRecord();
            record.Id = int.Parse(parameters, CultureInfo.InvariantCulture);
            int listCount = Program.fileCabinetService.GetStat().Item1;

            if (record.Id > listCount || record.Id < 1)
            {
                Console.WriteLine($"#{record.Id} record not found");
                return;
            }

            Program.InputRecordProperties(record);
            FileCabinetEventArgs recordArgs = new FileCabinetEventArgs(record);
            EditRecordEvent(null, recordArgs);
        }

        private static void Find(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentException("Parameters argument is null");
            }

            string[] searchArguments = parameters.Split(' ');
            ReadOnlyCollection<FileCabinetRecord> fileCabinetRecords;
            searchArguments[0] = searchArguments[0].ToUpperInvariant();

            if (searchArguments[1][0] != '"' && searchArguments[1][^1] != '"')
            {
                Console.WriteLine("! Search value must be in quotes. Abort find command.");
                return;
            }

            switch (searchArguments[0])
            {
                case "FIRSTNAME": fileCabinetRecords = Program.fileCabinetService.FindByFirstName(searchArguments[1][1..^1]); break;
                case "LASTNAME": fileCabinetRecords = Program.fileCabinetService.FindByLastName(searchArguments[1][1..^1]); break;
                case "DATEOFBIRTH": fileCabinetRecords = Program.fileCabinetService.FindByDateOfBirth(searchArguments[1][1..^1]); break;
                default: Console.WriteLine("! Wrong search parameter."); return;
            }

            foreach (FileCabinetRecord record in fileCabinetRecords)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, " +
                    $"{record.DateOfBirth.Year}-{record.DateOfBirth.Month}-{record.DateOfBirth.Day}, " +
                    $"{record.JobExperience}, {record.MonthlyPay}, {record.Gender}");
            }
        }

        private static void Export(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentException("Parameters argument is null");
            }

            string[] exportParams = parameters.Split(' ');
            char unswer = ' ';
            bool run;

            if (File.Exists(exportParams[1]))
            {
                Console.WriteLine($"File exists - rewrite {exportParams[1]}? [Y/n]");
                do
                {
                    try
                    {
                        unswer = char.ToUpper(char.Parse(Console.ReadLine()), CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        unswer = ' ';
                    }

                    if (!(unswer == 'Y' || unswer == 'N'))
                    {
                        Console.WriteLine("Type Y or N");
                        run = true;
                    }
                    else
                    {
                        run = false;
                    }
                }
                while (run);
            }
            else
            {
                StreamWriter stream = new StreamWriter(exportParams[1]);
                stream.Close();
            }

            switch (unswer)
            {
                case 'Y': ExportToFile(exportParams[0], exportParams[1]); break;
                case 'N': break;
                default: ExportToFile(exportParams[0], exportParams[1]); break;
            }
        }

        private static void ExportToFile(string format, string path)
        {
            FileStream file;
            FileCabinetServiceSnapshot snapshot;
            StreamWriter streamWriter;

            try
            {
                file = new FileStream(path, FileMode.Open);
                file.Dispose();
                file.Close();
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Export failed: can't open file {path}");
                return;
            }

            switch (format.ToUpper(CultureInfo.InvariantCulture))
            {
                case "CSV":
                    snapshot = Program.fileCabinetService.MakeSnapshot();
                    streamWriter = new StreamWriter(path);
                    snapshot.SaveToCsv(streamWriter);
                    streamWriter.Close();
                    Console.WriteLine($"All records are exported to file {path}");
                    break;
                case "XML":
                    snapshot = Program.fileCabinetService.MakeSnapshot();
                    streamWriter = new StreamWriter(path);
                    snapshot.SaveToXml(streamWriter);
                    streamWriter.Close();
                    Console.WriteLine($"All records are exported to file {path}");
                    break;
                default: Console.WriteLine("Unsupported file format"); break;
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

            if (!importArguments[0].ToUpperInvariant().Equals(path[(Array.FindIndex(path.ToCharArray(), i => i.Equals('.')) + 1)..].ToUpperInvariant()))
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

        private static void Purge(string parameters)
        {
            if (parameters.Length > 0)
            {
                Console.WriteLine($"Unrecognized parameter {parameters}");
                return;
            }

            Program.fileCabinetService.PurgeFile();
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
