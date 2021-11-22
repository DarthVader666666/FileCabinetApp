using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// From file to stream xml reader.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private readonly XmlReader xmlReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="fileStream">Stream to read from xml file.</param>
        public FileCabinetRecordXmlReader(StreamReader fileStream)
        {
            this.xmlReader = XmlReader.Create(fileStream);
        }

        /// <summary>
        /// Reads csv file and returns data in IList format.
        /// </summary>
        /// <returns>List of records.</returns>
        public List<FileCabinetRecord> ReadAll()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileCabinetXmlSerializeble));
            FileCabinetXmlSerializeble records = (FileCabinetXmlSerializeble)xmlSerializer.Deserialize(this.xmlReader);

            return records.GetRecordsFromXml();
        }
    }
}
