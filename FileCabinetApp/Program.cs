using System;
using System.Collections.ObjectModel;
using System.Globalization;

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
        private const string DefaultValidationMessage = "Using default validation rules.";
        private const string CustomValidationMessage = "Using custom validation rules.";
        private const string FileStorageMessage = "Using file storage.";
        private const string MemoryStorageMessage = "Using memory storage.";

        /// <summary>
        /// file cabinet instance.
        /// </summary>
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new Validators.ValidatorBuilder().DefaultValidation().Create());
        private static bool isRunning = true;
        private static Action<bool> breakAll = StopProgram;
        private static Action<ReadOnlyCollection<FileCabinetRecord>> printer = Defaultprinter;

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

            // args = new string[] { "-s", "file" };
            if (args.Length == 1)
            {
                args = args[0].Split('=');
            }

            if (args.Length == 2 && args[1].Length > 0 && (args[0] == "--validation-rule" || args[0] == "-v"))
            {
                switch (args[1].ToUpper(CultureInfo.InvariantCulture))
                {
                    case "DEFAULT":
                        fileCabinetService = new FileCabinetMemoryService(new Validators.ValidatorBuilder().DefaultValidation().Create());
                        Console.WriteLine(DefaultValidationMessage); break;
                    case "CUSTOM":
                        fileCabinetService = new FileCabinetMemoryService(new Validators.ValidatorBuilder().CustomValidation().Create());
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
                        fileCabinetService = new FileCabinetMemoryService(new Validators.ValidatorBuilder().DefaultValidation().Create());
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

            Console.WriteLine($"File Cabinet Application, developed by {DeveloperName}");
            Console.WriteLine(HintMessage);
            Console.WriteLine();

            var handler = CreateCommandHandlers(fileCabinetService);

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

                const int parametersIndex = 1;
                string parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;

                handler.Handle(new CommandHandlers.AddCommandRequest() { Command = command, Parameters = parameters });
            }
            while (isRunning);
        }

        /// <summary>
        /// Record parameters input.
        /// </summary>
        /// <param name="record">File record instance.</param>
        public static void InputRecordProperties(FileCabinetRecord record)
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
            firstNameValidator = ValidateString;
            lastNameValidator = ValidateString;
            dateOfBirthValidator = ValidateDateTime;
            jobExperienceValidator = ValidateShort;
            monthlyPayValidator = ValidateDecimal;
            genderValidator = ValidateChar;

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

        private static CommandHandlers.ICommandHandler CreateCommandHandlers(IFileCabinetService service)
        {
            var helpHandler = new CommandHandlers.HelpCommandHandler();
            var statHandler = new CommandHandlers.StatCommandHandler(service);
            var listHandler = new CommandHandlers.ListCommandHandler(service, printer);
            var createHandler = new CommandHandlers.CreateCommandHandler(service);
            var findHandler = new CommandHandlers.FindCommandHandler(service, printer);
            var editHandler = new CommandHandlers.EditCommandHandler(service);
            var importHandler = new CommandHandlers.ImportCommandHandler(service);
            var exportHandler = new CommandHandlers.ExportCommandHandler(service);
            var removeHandler = new CommandHandlers.RemoveCommandHandler(service);
            var purgeHandler = new CommandHandlers.PurgeCommandHandler(service);
            var exitHandler = new CommandHandlers.ExitCommandHandler(breakAll);

            helpHandler.SetNext(statHandler);
            statHandler.SetNext(listHandler);
            listHandler.SetNext(createHandler);
            createHandler.SetNext(findHandler);
            findHandler.SetNext(editHandler);
            editHandler.SetNext(importHandler);
            importHandler.SetNext(exportHandler);
            exportHandler.SetNext(removeHandler);
            removeHandler.SetNext(purgeHandler);
            purgeHandler.SetNext(exitHandler);

            return helpHandler;
        }

        private static void StopProgram(bool stop)
        {
            isRunning = stop;
        }

        /// <summary>
        /// Implements Default Print method.
        /// </summary>
        /// <param name="records">File records to be printed.</param>
        private static void Defaultprinter(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException($"{records} is null");
            }

            if (records.Count == 0)
            {
                Console.WriteLine("Record list is empty.");
                return;
            }

            foreach (FileCabinetRecord fileCabinetRecord in records)
            {
                Console.WriteLine($"#{fileCabinetRecord.Id}, {fileCabinetRecord.FirstName}, {fileCabinetRecord.LastName}, " +
                    $"{fileCabinetRecord.DateOfBirth.Year}-{fileCabinetRecord.DateOfBirth.Month}-{fileCabinetRecord.DateOfBirth.Day}, " +
                    $"{fileCabinetRecord.JobExperience}, " + string.Format(CultureInfo.InvariantCulture, "{0:F2}", fileCabinetRecord.MonthlyPay) +
                    $", {fileCabinetRecord.Gender}");
            }
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
                result = char.Parse(inputString);
            }

            return new Tuple<bool, string, char>(successful, failureMessage, result);
        }

        private static Tuple<bool, string> ValidateString(string inputData)
        {
            bool successful = true;
            string failureMessage = string.Empty;

            if (inputData.Length == 0 || Array.FindIndex(inputData.ToCharArray(), i => char.IsDigit(i)) >= 0)
            {
                failureMessage = "Field shouldn't be empty or contain digits.";
                successful = false;
            }

            return new Tuple<bool, string>(successful, failureMessage);
        }

        private static Tuple<bool, string> ValidateDateTime(DateTime inputData)
        {
            bool successful = true;
            string failureMessage = string.Empty;

            if (inputData.CompareTo(DateTime.Today) > 0)
            {
                failureMessage = "This person is from the future.";
                successful = false;
            }

            return new Tuple<bool, string>(successful, failureMessage);
        }

        private static Tuple<bool, string> ValidateShort(short inputData)
        {
            bool successful = true;
            string failureMessage = string.Empty;

            if (inputData < 0)
            {
                failureMessage = "Number shouldn't be negative";
                successful = false;
            }

            return new Tuple<bool, string>(successful, failureMessage);
        }

        private static Tuple<bool, string> ValidateDecimal(decimal inputData)
        {
            bool successful = true;
            string failureMessage = string.Empty;

            if (inputData < 0)
            {
                failureMessage = "Number shouldn't be negative";
                successful = false;
            }

            return new Tuple<bool, string>(successful, failureMessage);
        }

        private static Tuple<bool, string> ValidateChar(char inputData)
        {
            bool successful = true;
            string failureMessage = string.Empty;

            if (char.ToUpper(inputData, CultureInfo.InvariantCulture) != 'M' && char.ToUpper(inputData, CultureInfo.InvariantCulture) != 'F')
            {
                failureMessage = "Please, print letter M or F";
                successful = false;
            }

            return new Tuple<bool, string>(successful, failureMessage);
        }
    }
}