// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace FileCabinetGenerator
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using FileCabinetApp;

    /// <summary>
    /// Class which operates FileCabinetGenerator application.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Vadzim Rumiantsau";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "list", "prints generated record list" },
            new string[] { "generate", "generates record list" },
            new string[] { "export", "exports records into chosen file and format." },
        };

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("generate", Generate),
            new Tuple<string, Action<string>>("export", Export),
        };

        private static string fileType;
        private static string filePath;
        private static int recordsAmount;
        private static int startId;

        private static FileCabinetRecordGenerator fileGenerator = new FileCabinetRecordGenerator();
        private static FileStream fileStream;

        private static bool isRunning = true;

        /// <summary>
        /// Provides user interface and calls command handlers.
        /// </summary>
        /// <param name="args">Command line args.</param>
        public static void Main(string[] args)
        {
            //args = new string[] { "-t", "xml", "-o", "d:\\file.xml", "-a", "40", "-i", "1" };
            if (args is null)
            {
                throw new ArgumentNullException($"{args} is null");
            }

            Console.WriteLine($"File Generatort Application, developed by {DeveloperName}");
            Console.WriteLine(HintMessage);
            Console.WriteLine();

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

                if (!fileType.ToLower().Equals(filePath[(Array.FindIndex(filePath.ToCharArray(), i => i.Equals('.')) + 1) ..].ToLower()))
                {
                    Console.WriteLine("File type and file extention don't match.");
                    return;
                }

                try
                {
                    int.TryParse(args[5], out recordsAmount);
                }
                catch (ArgumentException)
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

            Console.WriteLine($"Chosen file type: {fileType}. Output file: {filePath}.");
            Console.WriteLine($"Amount of records: {recordsAmount}. Id start: {startId}");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} does not exist. It will be created during data export.");
            }

            Generate(string.Empty);

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

            if (fileStream != null)
            {
                fileStream.Close();
            }

            isRunning = false;
        }

        private static void List(string parameters)
        {
            ReadOnlyCollection<FileCabinetRecord> recordList = fileGenerator.GetRecords();

            if (recordList.Count == 0)
            {
                Console.WriteLine("Record list is empty.");
                return;
            }

            foreach (FileCabinetRecord fileCabinetRecord in recordList)
            {
                Console.WriteLine($"#{fileCabinetRecord.Id}, {fileCabinetRecord.FirstName}, {fileCabinetRecord.LastName}, " +
                    $"{fileCabinetRecord.DateOfBirth.Year}-{fileCabinetRecord.DateOfBirth.Month}-{fileCabinetRecord.DateOfBirth.Day}, " +
                    $"{fileCabinetRecord.JobExperience}, " + string.Format(CultureInfo.CreateSpecificCulture("en-US"), "{0:F2}", fileCabinetRecord.MonthlyPay) +
                    $" ,{fileCabinetRecord.Gender}");
            }
        }

        private static void Generate(string parameters)
        {
            Console.WriteLine("Generating. Please, wait...");
            fileGenerator.GenerateRecordList(startId, recordsAmount);
            Console.WriteLine("Records generated successfully. Type \"export\" to write chosen file.");
        }

        private static void Export(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentException("Parameters argument is null");
            }

            if (parameters.Length > 0)
            {
                Console.WriteLine($"Unrecognized additional parameters. {fileType} format was chosen.");
                Console.WriteLine($"Type \"export\" to write data into {filePath}.");
                return;
            }

            char unswer = ' ';
            bool run;

            if (File.Exists(filePath))
            {
                Console.WriteLine($"File exists - rewrite {filePath}? [Y/n]");
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
                StreamWriter stream = new StreamWriter(filePath);
                stream.Close();
            }

            switch (unswer)
            {
                case 'Y': ExportToFile(); break;
                case 'N': break;
                default: ExportToFile(); break;
            }
        }

        private static void ExportToFile()
        {
            FileCabinetServiceSnapshot snapshot;
            StreamWriter streamWriter;

            try
            {
                fileStream = new FileStream(filePath, FileMode.Open);
                fileStream.Dispose();
                fileStream.Close();
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Export failed: can't open file {filePath}");
                return;
            }

            switch (fileType.ToUpper())
            {
                case "CSV":
                    snapshot = fileGenerator.MakeSnapshot();
                    streamWriter = new StreamWriter(filePath);
                    snapshot.SaveToCsv(streamWriter);
                    streamWriter.Close();
                    Console.WriteLine($"{recordsAmount} records were written to {filePath}");
                    break;
                case "XML":
                    streamWriter = new StreamWriter(filePath);
                    Console.WriteLine("Writing...");
                    fileGenerator.SerializeRecordsToXml(streamWriter);
                    streamWriter.Close();
                    Console.WriteLine($"{recordsAmount} records were written to {filePath}");
                    break;
                default: Console.WriteLine("Unsupported file format"); break;
            }
        }
    }
}
