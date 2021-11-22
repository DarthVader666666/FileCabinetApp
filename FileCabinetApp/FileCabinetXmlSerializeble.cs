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
    public class FileCabinetXmlSerializeble
    {
        private List<FileCabinetRecordXmlSerializeble> recordsForXml;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetXmlSerializeble"/> class.
        /// </summary>
        /// <param name="list">List of records to import into xml file.</param>
        public FileCabinetXmlSerializeble(List<FileCabinetRecord> list)
        {
            if (list is null)
            {
                throw new ArgumentNullException($"{list} is null");
            }

            this.recordsForXml = new List<FileCabinetRecordXmlSerializeble>();
            this.ConvertToSerializebleData(list);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetXmlSerializeble"/> class.
        /// </summary>
        public FileCabinetXmlSerializeble()
        {
        }

        /// <summary>
        /// Gets or sets records for xml serialization.
        /// </summary>
        /// <value></value>
        [XmlElement("record")]
        public List<FileCabinetRecordXmlSerializeble> Records
        {
            get { return this.recordsForXml; }
            set { this.recordsForXml = value; }
        }

        /// <summary>
        /// Converts xmlSerializeble records from xml file to FileCabinetRecords.
        /// </summary>
        /// <returns>List of FileCabinetRecords.</returns>
        public List<FileCabinetRecord> GetRecordsFromXml()
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            FileCabinetRecord record;
            int id = 0;

            foreach (FileCabinetRecordXmlSerializeble xmlRecord in this.Records)
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
                this.Records.Add(new FileCabinetRecordXmlSerializeble(record));
            }
        }
    }
}
