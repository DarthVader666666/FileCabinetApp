using System;
using System.Globalization;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for defining a record about person.
    /// </summary>
    public class FileCabinetXmlSerializebleRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetXmlSerializebleRecord"/> class.
        /// </summary>
        /// <param name="record">File cabinet record to be changed for xml serialisation.</param>
        public FileCabinetXmlSerializebleRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException($"{record} is null");
            }

            this.Name = new NameClass();
            this.Id = record.Id;
            this.Name.First = record.FirstName;
            this.Name.Last = record.LastName;
            this.DateOfBirth = $"{record.DateOfBirth.Day:D2}/{record.DateOfBirth.Month:D2}/{record.DateOfBirth.Year}";
            this.JobExperience = record.JobExperience;
            this.MonthlyPay = string.Format(CultureInfo.CreateSpecificCulture("en-US"), "{0:F2}", record.MonthlyPay);
            this.Gender = $"{record.Gender}";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetXmlSerializebleRecord"/> class.
        /// </summary>
        public FileCabinetXmlSerializebleRecord()
        {
        }

        /// <summary>
        /// Gets or sets record's number.
        /// </summary>
        /// <value>Number of record in file cabinet.</value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Name property.
        /// </summary>
        /// <value>Xml serialiseble Name.</value>
        [XmlElement("name")]
        public NameClass Name { get; set; }

        /// <summary>
        /// Gets or sets person's day if birth.
        /// </summary>
        /// <value>Person's day of birth.</value>
        [XmlElement("dateOfBirth")]
        public string DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets quantity of years a person works on the current job.
        /// </summary>
        /// <value>The quantity of years person works.</value>
        [XmlElement("jobExperience")]
        public short JobExperience { get; set; }

        /// <summary>
        /// Gets or sets hom much a person earns per month.
        /// </summary>
        /// <value>The size of person's monthly salary.</value>
        [XmlElement("monthlyPay")]
        public string MonthlyPay { get; set; }

        /// <summary>
        /// Gets or sets first letter of peson's gender.
        /// </summary>
        /// <value>Person's gender.</value>
        [XmlElement("gender")]
        public string Gender { get; set; }
    }
}
