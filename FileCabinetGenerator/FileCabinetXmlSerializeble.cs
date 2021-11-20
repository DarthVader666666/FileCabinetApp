using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    [XmlRoot("records")]
    public class FileCabinetXmlSerializeble
    {
        private readonly List<FileCabinetRecordXmlSerializeble> recordsForXml;

        [XmlElement("record")]
        public List<FileCabinetRecordXmlSerializeble> Records
        {
            get { return recordsForXml; }
        }

        public FileCabinetXmlSerializeble(List<FileCabinetRecord> list)
        {
            if (list is null)
            {
                throw new ArgumentNullException($"{list} is null");
            }

            recordsForXml = new List<FileCabinetRecordXmlSerializeble>();
            ConvertToSerializebleData(list);
        }

        public FileCabinetXmlSerializeble()
        {
        }

        private void ConvertToSerializebleData(List<FileCabinetRecord> list)
        {
            if (list is null)
            {
                throw new ArgumentNullException($"{list} is null");
            }

            foreach (FileCabinetRecord record in list)
            {
                Records.Add(new FileCabinetRecordXmlSerializeble(record));
            }
        }
    }
}
