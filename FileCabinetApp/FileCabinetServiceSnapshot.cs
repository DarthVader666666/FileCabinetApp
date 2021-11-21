using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents a snapshot of a FileCabinetService instance with particullar field.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;

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
        /// Gets collection of records to implement restore.
        /// </summary>
        /// <value>
        /// Collection of records to implement Restore().
        /// </value>
        public ReadOnlyCollection<FileCabinetRecord> Records
        {
            get { return new ReadOnlyCollection<FileCabinetRecord>(this.records); }
        }

        /// <summary>
        /// Loads data from csv file and gets list of recods.
        /// </summary>
        /// <param name="streamReader">Stream of file to read.</param>
        public void LoadFromCsv(StreamReader streamReader)
        {
            FileCabinetRecordCsvReader csvReader = new FileCabinetRecordCsvReader(streamReader);
            this.records = csvReader.ReadAll().ToArray();
        }

        /// <summary>
        /// Calls for stream write method.
        /// </summary>
        /// <param name="streamWriter">StreamWriter instance opened to write to.</param>
        public void SaveToCsv(StreamWriter streamWriter)
        {
            FileCabinetRecordCsvWriter csvWriter = new FileCabinetRecordCsvWriter(streamWriter);
            csvWriter.Write(this.records);
        }

        /// <summary>
        /// Calls for xml write method.
        /// </summary>
        /// <param name="streamWriter">StreamWriter instance opened to write to.</param>
        public void SaveToXml(StreamWriter streamWriter)
        {
            FileCabinetRecordXmlWriter xmlWriter = new FileCabinetRecordXmlWriter(XmlWriter.Create(streamWriter));
            xmlWriter.Write(this.records);
        }
    }
}
