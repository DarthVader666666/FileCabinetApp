using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents a snapshot of a FileCabinetService instance with particullar field.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="list">All file records to make a copy of.</param>
        public FileCabinetServiceSnapshot(List<FileCabinetRecord> list)
        {
            if (list is null)
            {
                throw new ArgumentNullException($"{list} is null");
            }

            this.records = list.ToArray();
        }

        /// <summary>
        /// Calls for stream write method.
        /// </summary>
        /// <param name="stream">StreamWriter instance opened to write to.</param>
        public void SaveToCsv(StreamWriter stream)
        {
            FileCabinetRecordCsvWriter csvWriter = new FileCabinetRecordCsvWriter(stream);
            csvWriter.Write(this.records);
        }

        /// <summary>
        /// Calls for xml write method.
        /// </summary>
        /// <param name="stream">XmlWriter instance opened to write to.</param>
        public void SaveToXml(XmlWriter stream)
        {
            FileCabinetRecordXmlWriter xmlWriter = new FileCabinetRecordXmlWriter(stream);
            xmlWriter.Write(this.records);
        }
    }
}
