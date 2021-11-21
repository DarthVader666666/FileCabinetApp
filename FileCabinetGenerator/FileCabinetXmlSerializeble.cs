// <copyright file="FileCabinetXmlSerializeble.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace FileCabinetGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using FileCabinetApp;

    /// <summary>
    /// Serializes records to xml format.
    /// </summary>
    [XmlRoot("records")]
    public class FileCabinetXmlSerializeble
    {
        private readonly List<FileCabinetRecordXmlSerializeble> recordsForXml;

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
        /// Gets records for xml serialization.
        /// </summary>
        [XmlElement("record")]
        public List<FileCabinetRecordXmlSerializeble> Records
        {
            get { return this.recordsForXml; }
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
