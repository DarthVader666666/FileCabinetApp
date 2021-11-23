﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Class which operates FileCabinetApp application.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Vadzim Rumiantsau";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string StorageDbFilePath = "cabinet-records.db";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private const string DefaultValidationMessage = "Using default validation rules.";
        private const string CustomValidationMessage = "Using custom validation rules.";
        private const string FileStorageMessage = "Using file storage.";
        private const string MemoryStorageMessage = "Using memory storage.";

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("purge", Purge),
            new Tuple<string, Action<string>>("stat", Stat),
        };

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

        private static IReadInputValidator readInputValidator = new DefaultValidator();
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());

        private static bool isRunning = true;

        /// <summary>
        /// Initializes FIleCabinetFilesystemService instance.
        /// </summary>
        private static FileStream fileStream;

        /// <summary>
        /// Create record handler
        /// </summary>
        private static event EventHandler<FileCabinetEventArgs> CreateRecordEvent;

        /// <summary>
        /// Edit record handler
        /// </summary>
        private static event EventHandler<FileCabinetEventArgs> EditRecordEvent;

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

            //args = new string[] { "-s", "file" };

            if (args.Length == 1)
            {
                args = args[0].Split('=');
            }

            if (args.Length == 2 && args[1].Length > 0 && (args[0] == "--validation-rule" || args[0] == "-v"))
            {
                switch (args[1].ToUpper(CultureInfo.InvariantCulture))
                {
                    case "DEFAULT":
                        fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
                        readInputValidator = new DefaultValidator();
                        Console.WriteLine(DefaultValidationMessage); break;
                    case "CUSTOM":
                        fileCabinetService = new FileCabinetMemoryService(new CustomValidator());
                        readInputValidator = new CustomValidator();
                        Console.WriteLine(CustomValidationMessage); break;
                }
            }
            else
            {
                Console.WriteLine(DefaultValidationMessage);
            }

            if (args.Length == 2 && args[1].Length > 0 && (args[0] == "--storage" || args[0] == "-s"))
            {
                switch (args[1].ToUpper(CultureInfo.InvariantCulture))
                {
                    case "MEMORY":
                        fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
                        Console.WriteLine(MemoryStorageMessage); break;
                    case "FILE":
                        fileCabinetService = new FileCabinetFilesystemService(StorageDbFilePath);
                        Console.WriteLine(FileStorageMessage); break;
                }
            }
            else
            {
                Console.WriteLine(MemoryStorageMessage);
            }

            CreateRecordEvent += fileCabinetService.CreateRecord;
            EditRecordEvent += fileCabinetService.EditRecord;

            Console.WriteLine($"File Cabinet Application, developed by {DeveloperName}");
            Console.WriteLine(HintMessage);
            Console.WriteLine();

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

        private static void Create(string parameters)
        {
            FileCabinetRecord record = new FileCabinetRecord();
            InputRecordProperties(record);
            record.Id = fileCabinetService.GetMaxId() + 1;
            FileCabinetEventArgs recordArgs = new FileCabinetEventArgs(record);
            CreateRecordEvent(null, recordArgs);
            Console.WriteLine($"Record #{record.Id} is created.");
        }

        private static void List(string parameters)
        {
            ReadOnlyCollection<FileCabinetRecord> recordList = fileCabinetService.GetRecords();

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
            int listCount = fileCabinetService.GetStat().Item1;

            if (record.Id > listCount || record.Id < 1)
            {
                Console.WriteLine($"#{record.Id} record not found");
                return;
            }

            InputRecordProperties(record);
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
                case "FIRSTNAME": fileCabinetRecords = fileCabinetService.FindByFirstName(searchArguments[1][1..^1]); break;
                case "LASTNAME": fileCabinetRecords = fileCabinetService.FindByLastName(searchArguments[1][1..^1]); break;
                case "DATEOFBIRTH": fileCabinetRecords = fileCabinetService.FindByDateOfBirth(searchArguments[1][1..^1]); break;
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
                    snapshot = fileCabinetService.MakeSnapshot();
                    streamWriter = new StreamWriter(path);
                    snapshot.SaveToCsv(streamWriter);
                    streamWriter.Close();
                    Console.WriteLine($"All records are exported to file {path}");
                    break;
                case "XML":
                    snapshot = fileCabinetService.MakeSnapshot();
                    streamWriter = new StreamWriter(path);
                    snapshot.SaveToXml(streamWriter);
                    streamWriter.Close();
                    Console.WriteLine($"All records are exported to file {path}");
                    break;
                default: Console.WriteLine("Unsupported file format"); break;
            }
        }

        private static void InputRecordProperties(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record is null");
            }

            Func<string, Tuple<bool, string, string>> stringConverter;
            Func<string, Tuple<bool, string, DateTime>> dateTimeConverter;
            Func<string, Tuple<bool, string, short>> shortConverter;
            Func<string, Tuple<bool, string, decimal>> decimalConverter;
            Func<string, Tuple<bool, string, char>> charConverter;
            Func<string, Tuple<bool, string>> firstNameValidator;
            Func<string, Tuple<bool, string>> lastNameValidator;
            Func<DateTime, Tuple<bool, string>> dateOfBirthValidator;
            Func<short, Tuple<bool, string>> jobExperienceValidator;
            Func<decimal, Tuple<bool, string>> monthlyPayValidator;
            Func<char, Tuple<bool, string>> genderValidator;

            stringConverter = ConvertToString;
            dateTimeConverter = ConvertToDateTime;
            shortConverter = ConvertToShort;
            decimalConverter = ConvertToDecimal;
            charConverter = ConvertToChar;
            firstNameValidator = readInputValidator.ValidateString;
            lastNameValidator = readInputValidator.ValidateString;
            dateOfBirthValidator = readInputValidator.ValidateDateTime;
            jobExperienceValidator = readInputValidator.ValidateShort;
            monthlyPayValidator = readInputValidator.ValidateDecimal;
            genderValidator = readInputValidator.ValidateChar;

            Console.Write("First name: ");
            record.FirstName = ReadInput(stringConverter, firstNameValidator);

            Console.Write("Last name: ");
            record.LastName = ReadInput(stringConverter, lastNameValidator);

            Console.Write("Date of birth: ");
            record.DateOfBirth = ReadInput(dateTimeConverter, dateOfBirthValidator);

            Console.Write("Job experience (yrs): ");
            record.JobExperience = ReadInput(shortConverter, jobExperienceValidator);

            Console.Write("Monthly pay ($): ");
            record.MonthlyPay = ReadInput(decimalConverter, monthlyPayValidator);

            Console.Write("Gender (M/F): ");
            record.Gender = ReadInput(charConverter, genderValidator);
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static Tuple<bool, string, string> ConvertToString(string inputString)
        {
            bool successful = !string.IsNullOrEmpty(inputString);
            string failureMessage = string.Empty;

            if (!successful)
            {
                inputString = string.Empty;
                failureMessage = "Input string is null or empty";
            }

            return new Tuple<bool, string, string>(successful, failureMessage, inputString);
        }

        private static Tuple<bool, string, DateTime> ConvertToDateTime(string inputString)
        {
            bool successful = true;
            DateTime result;
            string failureMessage = string.Empty;

            try
            {
                result = DateTime.Parse(inputString, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                successful = false;
                result = new ();
                failureMessage = "DateTime format must be day/month/year";
            }

            return new Tuple<bool, string, DateTime>(successful, failureMessage, result);
        }

        private static Tuple<bool, string, short> ConvertToShort(string inputString)
        {
            bool successful = true;
            short result = 0;
            string failureMessage = string.Empty;

            try
            {
                result = short.Parse(inputString, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                failureMessage = "Short gets wrong number format";
                successful = false;
            }
            catch (OverflowException)
            {
                failureMessage = "Short number is overflown";
                successful = false;
            }

            return new Tuple<bool, string, short>(successful, failureMessage, result);
        }

        private static Tuple<bool, string, decimal> ConvertToDecimal(string inputString)
        {
            bool successful = true;
            decimal result = 0;
            string failureMessage = string.Empty;

            try
            {
                result = decimal.Parse(inputString, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                failureMessage = "Decimal gets wrong number format";
                successful = false;
            }
            catch (OverflowException)
            {
                failureMessage = "Decimal is overflown";
                successful = false;
            }

            return new Tuple<bool, string, decimal>(successful, failureMessage, result);
        }

        private static Tuple<bool, string, char> ConvertToChar(string inputString)
        {
            bool successful = true;
            char result = ' ';
            string failureMessage = string.Empty;

            if (inputString.Length > 1 || inputString.Length == 0)
            {
                failureMessage = "Char gets more than one symbol ore none";
                successful = false;
            }
            else
            {
                result = char.ToUpper(char.Parse(inputString), CultureInfo.InvariantCulture);
            }

            return new Tuple<bool, string, char>(successful, failureMessage, result);
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

            try
            {
                fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

                switch (dataType.ToUpperInvariant())
                {
                    case "CSV":
                        snapshot = fileCabinetService.MakeSnapshot();
                        streamReader = new StreamReader(fileStream);
                        snapshot.LoadFromCsv(streamReader);
                        fileCabinetService.Restore(snapshot);
                        Console.WriteLine($"{snapshot.Records.Count} records were imported from {path}");
                        streamReader.Close();
                        break;
                    case "XML":
                        snapshot = fileCabinetService.MakeSnapshot();
                        streamReader = new StreamReader(fileStream);
                        snapshot.LoadFromXml(streamReader);
                        fileCabinetService.Restore(snapshot);
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

            fileCabinetService.RemoveRecord(id);
        }

        private static void Purge(string parameters)
        {
            if (parameters.Length > 0)
            {
                Console.WriteLine($"Unrecognized parameter {parameters}");
                return;
            }

            fileCabinetService.PurgeFile();
        }

        private static void Stat(string parameters)
        {
            if (parameters.Length > 0)
            {
                Console.WriteLine($"Unrecognized parameter {parameters}");
                return;
            }

            Tuple<int, int> countDeleted = fileCabinetService.GetStat();
            Console.WriteLine($"{countDeleted.Item1} recods in list, {countDeleted.Item2} deleted.");
        }
    }
}