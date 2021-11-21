// <copyright file="FileCabinetRecordGenerator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace FileCabinetGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Xml.Serialization;
    using FileCabinetApp;

    /// <summary>
    /// Generates record list.
    /// </summary>
    public class FileCabinetRecordGenerator
    {
        private readonly IRecordValidator validator;

        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordGenerator"/> class.
        /// </summary>
        public FileCabinetRecordGenerator()
        {
            this.validator = new FileCabinetApp.DefaultValidator();
        }

        /// <summary>
        /// Gets generated record list.
        /// </summary>
        public List<FileCabinetRecord> RecordList
        {
            get { return this.list; }
        }

        /// <summary>
        /// Generates new record list.
        /// </summary>
        /// <param name="startId">Start id record number.</param>
        /// <param name="recordsAmount">Amount of records to be generated.</param>
        public void GenerateRecordList(int startId, int recordsAmount)
        {
            FileCabinetRecord record;
            Random random = new Random();
            char[] genderChars = new char[] { 'F', 'M' };
            bool running;
            this.list.Clear();

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
                        record.MonthlyPay = (decimal)random.Next(30000, 300000) / 100;
                        record.Gender = genderChars[random.Next(2)];

                        this.validator.ValidateParameters(new FileCabinetEventArgs(record));

                        startId++;
                    }
                    catch (Exception)
                    {
                        running = true;
                    }
                }
                while (running);

                this.list.Add(record);
            }
        }

        /// <summary>
        /// Getsgenerated list of records.
        /// </summary>
        /// <returns>ReadOnlyCollection of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return this.list.AsReadOnly();
        }

        /// <summary>
        /// Gets snapshot entity.
        /// </summary>
        /// <returns>Snapshot object.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list);
        }

        /// <summary>
        /// Serializes recods into xml format.
        /// </summary>
        /// <param name="steramWriter">Stream fo searilization.</param>
        public void SerializeRecordsToXml(StreamWriter steramWriter)
        {
            FileCabinetXmlSerializeble data = new FileCabinetXmlSerializeble(this.list);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileCabinetGenerator.FileCabinetXmlSerializeble));
            xmlSerializer.Serialize(steramWriter, data);
        }

        /// <summary>
        /// Imports generated list of records.
        /// </summary>
        /// <param name="csvReader">Stream for file reading.</param>
        public void ImportCsv(StreamReader csvReader)
        {
            csvReader.ReadLine();
            string[] recordFields;
            string line;
            int index;
            FileCabinetRecord record;
            int count = 0;
            int replaced = 0;

            while ((line = csvReader.ReadLine()) != null)
            {
                recordFields = line.Split(',');

                try
                {
                    record = new FileCabinetRecord();
                    record.Id = int.Parse(recordFields[0]);
                    record.FirstName = recordFields[1];
                    record.LastName = recordFields[2];
                    record.DateOfBirth = DateTime.Parse(recordFields[3], CultureInfo.CreateSpecificCulture("en-GB"));
                    record.JobExperience = short.Parse(recordFields[4]);
                    record.MonthlyPay = decimal.Parse(recordFields[5], CultureInfo.CreateSpecificCulture("en-US"));
                    record.Gender = char.Parse(recordFields[6]);
                }
                catch (FormatException)
                {
                    throw new ArgumentException("Import data has wrong format.");
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException("Import data has wrong format.");
                }

                index = this.list.FindIndex(0, this.list.Count, i => i.Id.Equals(int.Parse(recordFields[0])));

                if (index != -1)
                {
                    this.list[index] = record;
                    replaced++;
                }
                else
                {
                    this.list.Add(record);
                }

                count++;
            }

            Console.WriteLine("{0} records were imported, {1} records were replaced.", count, replaced);
        }

        private static string GenerateString()
        {
            Random random = new Random();
            string[] names = new string[]
            {
                "Vadim", "Mark", "John", "Wick", "Neo", "Radagast", "Bilbo", "Baggins", "Sveta", "Tanya", "Colonel", "Beavis",
                "Gretta", "Turnberg", "Peater", "Lennon", "Trinity", "Gendalf", "Spock", "Katz", "Merkel", "Daiva", "Stark", "Comrade", "Parker", "Frodo",
                "Tauriel", "Lora", "Palmer", "Sarah", "Connor", "Vladimir", "Harconer", "Leto", "Atredis", "Poul", "Muaddib", "Batista", "Brad", "Pitt",
            };

            return names[random.Next(names.Length + 1)];
        }

        private static DateTime GenerateDateTime()
        {
            Random random = new Random();
            DateTime dateOfBirth = new ();
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
