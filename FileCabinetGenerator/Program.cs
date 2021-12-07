// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using FileCabinetApp;

namespace FileCabinetGenerator
{
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
            if (args is null)
            {
                throw new ArgumentNullException($"{args} is null");
            }

            if (args.Length <= 8)
            {
                string wholeLine = string.Join(' ', args);
                args = wholeLine.Split(new char[] { ' ', '=' });
            }

            if ((args[0].ToUpperInvariant() == "--OUTPUT-TYPE" || args[0].ToUpperInvariant() == "-T") && (args[2].ToUpperInvariant() == "--OUTPUT" || args[2].ToUpperInvariant() == "-O") &&
                (args[4].ToUpperInvariant() == "--RECORDS-AMOUNT" || args[4].ToUpperInvariant() == "-A") && (args[6].ToUpperInvariant() == "--START-ID" || args[6].ToUpperInvariant() == "-I"))
            {
                fileType = args[1];
                filePath = args[3];

                if (!(fileType.ToUpperInvariant() == "CSV" || fileType.ToUpperInvariant() == "XML"))
                {
                    Console.WriteLine($"Wrong file type '{fileType}'");
                    return;
                }

                if (!fileType.ToUpperInvariant().Equals(filePath[(Array.FindIndex(filePath.ToCharArray(), i => i.Equals('.')) + 1) ..].ToUpperInvariant()))
                {
                    Console.WriteLine($"File type '{fileType}' and file extention of {filePath} don't match.");
                    return;
                }

                if (!int.TryParse(args[5], out recordsAmount))
                {
                    Console.WriteLine($"Wrong records amount parameter '{args[5]}'.");
                    return;
                }

                if (!int.TryParse(args[7], out startId) || startId < 1)
                {
                    Console.WriteLine($"Wrong start Id parameter '{args[7]}'.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Wrong input parameters. Ex: --output-type xml --output d:\\file.xml --records-amount 40 --start-id 1");
                return;
            }

            Console.WriteLine($"File Generatort Application, developed by {DeveloperName}");
            Console.WriteLine(HintMessage);
            Console.WriteLine();
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

            switch (fileType.ToUpperInvariant())
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
                    Console.WriteLine("Writing... ");
                    fileGenerator.SerializeRecordsToXml(streamWriter);
                    streamWriter.Close();
                    Console.WriteLine($"{recordsAmount} records were written to {filePath}");
                    break;
                default: Console.WriteLine("Unsupported file format"); break;
            }
        }
    }
}
