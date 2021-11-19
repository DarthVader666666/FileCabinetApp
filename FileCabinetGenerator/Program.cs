using System;
using System.Collections.ObjectModel;
using System.IO;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Class which operates FileCabinetGenerator application. 
    /// </summary>
    static class Program
    {
        private static string fileType;
        private static string filePath;
        private static int recordsAmount;
        private static int startId;
        private static StreamReader streamReader;

        private const string DeveloperName = "Vadzim Rumiantsau";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("generate", Generate),
            new Tuple<string, Action<string>>("list", List),
        };

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        private static bool isRunning = true;

        private static readonly FileCabinetGenerator fileCabinetGenerator = new FileCabinetGenerator();

        static void Main(string[] args)
        {
            args = new string[] { "-t", "csv", "-o", "file.csv", "-a", "5", "-i", "2" };

            if (args is null)
            {
                throw new ArgumentNullException($"{args} is null");
            }

            if (args.Length <= 8)
            {
                string wholeLine = string.Join(' ', args);
                args = wholeLine.Split(new char[] { ' ', '=' });
            }

            if ((args[0].ToLower() == "--output-type" || args[0].ToLower() == "-t") && (args[2].ToLower() == "--output" || args[2].ToLower() == "-o") &&
                (args[4].ToLower() == "--records-amount" || args[4].ToLower() == "-a") && (args[6].ToLower() == "--start-id" || args[6].ToLower() == "-i"))
            {
                fileType = args[1];
                filePath = args[3];

                if (!(fileType.ToLower() == "csv" || fileType.ToLower() == "xml"))
                {
                    Console.WriteLine("Wrong file type");
                    return;
                }

                if (!fileType.ToLower().Equals(filePath[(Array.FindIndex(filePath.ToCharArray(), i => i.Equals('.')) + 1)..].ToLower()))
                {
                    Console.WriteLine("File type and file extention don't match.");
                    return;
                }

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("No such file.");
                    return;
                }

                try
                {
                    int.TryParse(args[5], out recordsAmount);
                }
                catch(ArgumentException)
                {
                    Console.WriteLine("Wrong records amount parameter.");
                    return;
                }

                try
                {
                    int.TryParse(args[7], out startId);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Wrong start Id parameter.");
                    return;
                }
            }

            Console.WriteLine($"File Generatort Application, developed by {DeveloperName}");
            Console.WriteLine($"File type {fileType} chosen. Output file: {filePath}.");
            Console.WriteLine($"Amount of records: {recordsAmount}. Id start: {startId}");
            Console.WriteLine(HintMessage);
            Console.WriteLine();

            streamReader = new StreamReader(filePath);

            do
            {
                Console.Write("> ");
                string[] inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                string command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(HintMessage);
                    continue;
                }

                int index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    string parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    Commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
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

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");

            if (streamReader != null)
            {
                streamReader.Close();
            }

            isRunning = false;
        }

        private static void List(string parameters)
        {
            ReadOnlyCollection<FileCabinetRecord> recordList = fileCabinetGenerator.GetRecords();

            if (recordList.Count == 0)
            {
                Console.WriteLine("Record list is empty.");
                return;
            }

            foreach (FileCabinetRecord fileCabinetRecord in recordList)
            {
                Console.WriteLine($"#{fileCabinetRecord.Id}, {fileCabinetRecord.FirstName}, {fileCabinetRecord.LastName}, " +
                    $"{fileCabinetRecord.DateOfBirth.Year}-{fileCabinetRecord.DateOfBirth.Month}-{fileCabinetRecord.DateOfBirth.Day}, " +
                    $"{fileCabinetRecord.JobExperience}, {fileCabinetRecord.MonthlyPay}, {fileCabinetRecord.Gender}");
            }
        }

        private static void Generate(string parameters)
        {
            fileCabinetGenerator.GenerateRecordList(startId, recordsAmount);
            Console.WriteLine("Records generated successfully.");
        }

    }
}
