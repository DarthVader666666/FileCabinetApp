// <copyright file="FileCabinetXmlSerializeble.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace FileCabinetApp
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml.Serialization;

    /// <summary>
    /// Serializes records to xml format.
    /// </summary>
    [XmlRoot("records")]
    public class FileCabinetXmlSerializebleData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetXmlSerializebleData"/> class.
        /// </summary>
        /// <param name="list">List of records to import into xml file.</param>
        public FileCabinetXmlSerializebleData(List<FileCabinetRecord> list)
        {
            if (list is null)
            {
                throw new ArgumentNullException($"{list} is null");
            }

            this.Records = new List<FileCabinetXmlSerializebleRecord>();
            this.ConvertToSerializebleData(list);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetXmlSerializebleData"/> class.
        /// </summary>
        public FileCabinetXmlSerializebleData()
        {
        }

        /// <summary>
        /// Gets or sets records for xml serialization.
        /// </summary>
        /// <value></value>
        [XmlElement("record")]
        public List<FileCabinetXmlSerializebleRecord> Records { get; set; }

        /// <summary>
        /// Converts xmlSerializeble records from xml file to FileCabinetRecords.
        /// </summary>
        /// <returns>List of FileCabinetRecords.</returns>
        public List<FileCabinetRecord> GetRecordsFromXml()
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            FileCabinetRecord record;
            int id = 0;

            foreach (FileCabinetXmlSerializebleRecord xmlRecord in this.Records)
            {
                record = new FileCabinetRecord();

                try
                {
                    id = record.Id = xmlRecord.Id;
                    record.FirstName = xmlRecord.Name.First;
                    record.LastName = xmlRecord.Name.Last;
                    record.DateOfBirth = DateTime.Parse(xmlRecord.DateOfBirth, CultureInfo.CreateSpecificCulture("en-GB"));
                    record.JobExperience = xmlRecord.JobExperience;
                    record.MonthlyPay = decimal.Parse(xmlRecord.MonthlyPay, CultureInfo.InvariantCulture);
                    record.Gender = char.Parse(xmlRecord.Gender);

                    records.Add(record);
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Record #{id} has wrong format. Line skipped.");
                }
            }

            return records;
        }

        private void ConvertToSerializebleData(List<FileCabinetRecord> list)
        {
            if (list is null)
            {
                throw new ArgumentNullException($"{list} is null");
            }

            foreach (FileCabinetRecord record in list)
            {
                this.Records.Add(new FileCabinetXmlSerializebleRecord(record));
            }
        }
    }
}
