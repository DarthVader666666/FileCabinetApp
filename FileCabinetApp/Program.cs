﻿using System;
using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Vadzim Rumiantsau";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static FileCabinetService fileCabinetService = new FileCabinetService();

        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
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
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            Console.Write("First name: ");
            var firstName = Console.ReadLine();
            Console.Write("Last name: ");
            var lastName = Console.ReadLine();
            Console.Write("Date of birth: ");
            var dateOfBirth = DateTime.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
            Console.Write("Job experience (yrs): ");
            var jobExperience = short.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
            Console.Write("Monthly pay ($): ");
            var monthlyPay = decimal.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
            Console.Write("Gender (M/F): ");
            var gender = (char)Console.Read();

            if (!(gender == 'M' || gender == 'm' || gender == 'F' || gender == 'f'))
            {
                throw new ArgumentException("Invalid gender");
            }

            var recordId = Program.fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, jobExperience, monthlyPay, gender);

            Console.Write($"Record #{recordId} is created.");
        }

        private static void List(string parameters)
        {
            var recordList = Program.fileCabinetService.GetRecords();

            foreach (FileCabinetRecord fcr in recordList)
            {
                Console.WriteLine($"#{fcr.Id}, {fcr.FirstName}, {fcr.LastName}, {fcr.DateOfBirth.Year}-" +
                    $"{fcr.DateOfBirth.Month}-{fcr.DateOfBirth.Day}, {fcr.JobExperience}, {fcr.MonthlyPay}, {fcr.Gender}");
            }
        }
    }
}