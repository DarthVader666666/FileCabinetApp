using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    class FileCabinetGenerator
    {
        private readonly IRecordValidator validator;
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public FileCabinetGenerator()
        {
            validator = new FileCabinetApp.DefaultValidator();            
        }

        public void GenerateRecordList(int startId, int recordsAmount)
        {
            FileCabinetRecord record;
            Random random = new Random();
            char[] genderChars = new char[] { 'f', 'F', 'm', 'M' };
            bool running;
            list.Clear();

            for (int i = recordsAmount; i > 0; i--)
            {
                record = new FileCabinetRecord();

                do
                {
                    running = false;

                    try
                    {
                        record.Id = startId;
                        record.FirstName = GenerateString();
                        record.LastName = GenerateString();
                        record.DateOfBirth = GenerateDateTime();
                        record.JobExperience = (short)random.Next(0, 40);
                        record.MonthlyPay = random.Next(0, 10000);
                        record.Gender = genderChars[random.Next(genderChars.Length)];

                        validator.ValidateParameters(new FileCabinetEventArgs(record));

                        startId++;
                    }
                    catch (Exception)
                    {
                        running = true;
                    }
                }
                while (running);

                list.Add(record);
            }
        }

        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return list.AsReadOnly();
        }

        private static string GenerateString()
        {
            Random random = new Random();
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            char[] chars = new char[random.Next(10)];

            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = letters[random.Next(letters.Length)];
            }

            return new string(chars);
        }

        private static DateTime GenerateDateTime()
        {
            Random random = new Random();
            DateTime dateOfBirth = new DateTime();
            int day;
            int month;
            int year;
            bool running;

            do
            {
                running = false;

                try
                {
                    day = random.Next(32);
                    month = random.Next(13);
                    year = random.Next(1950, DateTime.Today.Year + 1);
                    dateOfBirth = new DateTime(year, month, day);
                }
                catch (ArgumentException)
                {
                    running = true;
                }

            }
            while (running);

            return dateOfBirth;
        }
    }
}
