using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace FileCabinetApp
{
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
        /// <value>List of Xml Serializeble Records.</value>
        [XmlElement("record")]
#pragma warning disable CA2227 // Collection properties should be read only
        public List<FileCabinetXmlSerializebleRecord> Records { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

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
