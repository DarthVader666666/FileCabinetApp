using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// From stream to csv file writer.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly XmlWriter xmlWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">Initializes field xmlWriter.</param>
        public FileCabinetRecordXmlWriter(XmlWriter writer)
        {
            this.xmlWriter = writer;
        }

        /// <summary>
        /// Serializes all records into XmlStream and writes them into xml file.
        /// </summary>
        /// <param name="records">File Cabinet records.</param>
        public void Write(FileCabinetRecord[] records)
        {
            if (records is null)
            {
                throw new ArgumentNullException($"{records} is null.");
            }

            this.xmlWriter.WriteStartElement("records");

            foreach (FileCabinetRecord record in records)
            {
                this.xmlWriter.WriteStartElement("record");
                this.xmlWriter.WriteAttributeString("id", $"{record.Id}");

                this.xmlWriter.WriteStartElement("name");
                this.xmlWriter.WriteAttributeString("last", record.LastName);
                this.xmlWriter.WriteAttributeString("first", record.FirstName);
                this.xmlWriter.WriteEndElement();

                this.xmlWriter.WriteElementString("dateOfBirth", $"{record.DateOfBirth.Day:D2}/{record.DateOfBirth.Month:D2}/{record.DateOfBirth.Year}");
                this.xmlWriter.WriteElementString("jobExperience", $"{record.JobExperience}");
                this.xmlWriter.WriteElementString("monthlyPay", string.Format(CultureInfo.CreateSpecificCulture("en-US"), "{0:F2}", record.MonthlyPay));
                this.xmlWriter.WriteElementString("gender", $"{record.Gender}");
                this.xmlWriter.WriteEndElement();
            }

            this.xmlWriter.WriteEndElement();

            this.xmlWriter.Close();
        }
    }
}
