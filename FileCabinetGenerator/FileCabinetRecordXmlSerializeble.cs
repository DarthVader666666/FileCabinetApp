// <copyright file="FileCabinetRecordXmlSerializeble.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace FileCabinetGenerator
{
    using System;
    using System.Globalization;
    using System.Xml.Serialization;
    using FileCabinetApp;

    /// <summary>
    /// Class for defining a record about person.
    /// </summary>
    public class FileCabinetRecordXmlSerializeble
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlSerializeble"/> class.
        /// </summary>
        /// <param name="record">File cabinet record to be changed for xml serialisation.</param>
        public FileCabinetRecordXmlSerializeble(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException($"{record} is null");
            }

            this.Id = record.Id;
            this.FirstName = record.FirstName;
            this.LastName = record.LastName;
            this.DateOfBirth = $"{record.DateOfBirth.Day:D2}/{record.DateOfBirth.Month:D2}/{record.DateOfBirth.Year}";
            this.JobExperience = record.JobExperience;
            this.MonthlyPay = string.Format(CultureInfo.CreateSpecificCulture("en-US"), "{0:F2}", record.MonthlyPay);
            this.Gender = $"{record.Gender}";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlSerializeble"/> class.
        /// </summary>
        public FileCabinetRecordXmlSerializeble()
        {
        }

        /// <summary>
        /// Gets or sets record's number.
        /// </summary>
        /// <value>Number of record in file cabinet.</value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets person's first name.
        /// </summary>
        /// <value>Person's first name.</value>
        [XmlElement("first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets person's last name.
        /// </summary>
        /// <value>Person's last name.</value>
        [XmlElement("last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets person's day if birth.
        /// </summary>
        /// <value>Person's day of birth.</value>
        [XmlElement("date_Of_Birth")]
        public string DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets quantity of years a person works on the current job.
        /// </summary>
        /// <value>The quantity of years person works.</value>
        [XmlElement("Job_Experience")]
        public short JobExperience { get; set; }

        /// <summary>
        /// Gets or sets hom much a person earns per month.
        /// </summary>
        /// <value>The size of person's monthly salary.</value>
        [XmlElement("Monthly_Pay")]
        public string MonthlyPay { get; set; }

        /// <summary>
        /// Gets or sets first letter of peson's gender.
        /// </summary>
        /// <value>Person's gender.</value>
        [XmlElement("gender")]
        public string Gender { get; set; }
    }
}
