using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// From file to stream csv reader.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private readonly StreamReader csvReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="fileStream">Stream to read from csv file.</param>
        public FileCabinetRecordCsvReader(StreamReader fileStream)
        {
            this.csvReader = fileStream;
        }

        /// <summary>
        /// Reads csv file and returns data in IList format.
        /// </summary>
        /// <returns>List of records.</returns>
        public List<FileCabinetRecord> ReadAll()
        {
            string[] recordFields;
            string line;
            int id = 0;
            FileCabinetRecord record;
            List<FileCabinetRecord> list = new List<FileCabinetRecord>();

            while ((line = this.csvReader.ReadLine()) != null)
            {
                try
                {
                    recordFields = line.Split(',');

                    record = new FileCabinetRecord();
                    id = record.Id = int.Parse(recordFields[0], CultureInfo.InvariantCulture);
                    record.FirstName = recordFields[1];
                    record.LastName = recordFields[2];
                    record.DateOfBirth = DateTime.Parse(recordFields[3], CultureInfo.CreateSpecificCulture("en-GB"));
                    record.JobExperience = short.Parse(recordFields[4], CultureInfo.InvariantCulture);
                    record.MonthlyPay = decimal.Parse(recordFields[5], CultureInfo.CreateSpecificCulture("en-US"));
                    record.Gender = char.Parse(recordFields[6]);

                    list.Add(record);
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Import data has wrong format. Line #{id} skipped.");
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Import data has wrong format. Line skipped.");
                }
            }

            this.csvReader.Close();
            return list;
        }
    }
}
