using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    public class FileCabinetRecordGenerator
    {
        private readonly IRecordValidator validator;

        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public List<FileCabinetRecord> RecordList { get { return list; } }

        public FileCabinetRecordGenerator()
        {
            validator = new FileCabinetApp.DefaultValidator();            
        }

        public void GenerateRecordList(int startId, int recordsAmount)
        {
            FileCabinetRecord record;
            Random random = new Random();
            char[] genderChars = new char[] { 'F', 'M' };
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
                        record.MonthlyPay = (decimal)random.Next(30000, 300000) / 100;
                        record.Gender = genderChars[random.Next(2)];

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
            string[] names = new string[] { "Vadim", "Mark", "John", "Wick", "Neo", "Radagast", "Bilbo", "Baggins", "Sveta", "Tanya", "Colonel", "Beavis",
                "Gretta", "Turnberg", "Peater", "Lennon", "Trinity", "Gendalf", "Spock", "Katz", "Merkel", "Daiva", "Stark", "Comrade", "Parker", "Frodo",
                "Tauriel", "Lora", "Palmer", "Sarah", "Connor", "Vladimir", "Harconer", "Leto", "Atredis", "Poul", "Muaddib", "Batista", "Brad", "Pitt" };

            return names[random.Next(names.Length + 1)];
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

        /// <summary>
        /// Gets snapshot entity.
        /// </summary>
        /// <returns>Snapshot object.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list);
        }

        public void SerializeRecordsToXml(StreamWriter steramWriter)
        {
            FileCabinetXmlSerializeble data = new FileCabinetXmlSerializeble(list);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileCabinetGenerator.FileCabinetXmlSerializeble));
            xmlSerializer.Serialize(steramWriter, data);
        }

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
                    record.DateOfBirth = DateTime.Parse(recordFields[3]);
                    record.JobExperience = short.Parse(recordFields[4]);
                    record.MonthlyPay = decimal.Parse(recordFields[5], CultureInfo.CreateSpecificCulture("en-US"));
                    record.Gender = char.Parse(recordFields[6]);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException("Import data has wrong format.");
                }

                index = list.FindIndex(0, list.Count, i => i.Id.Equals(int.Parse(recordFields[0])));

                if (index != -1)
                {
                    list[index] = record;
                    replaced++;
                }
                else
                {
                    list.Add(record);
                }

                count++;
            }

            Console.WriteLine("{0} records were imported, {1} records were replaced.", count, replaced);
        }
    }
}
