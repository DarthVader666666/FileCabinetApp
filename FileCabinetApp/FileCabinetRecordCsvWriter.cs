using System;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// From stream to csv file writer.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter csvWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="textWriter">Initializes field csvWriter.</param>
        public FileCabinetRecordCsvWriter(TextWriter textWriter)
        {
            this.csvWriter = textWriter;
        }

        /// <summary>
        /// Serializes all records into TextWriter Stream and writes them into csv file.
        /// </summary>
        /// <param name="records">File Cabinet records.</param>
        public void Write(FileCabinetRecord[] records)
        {
            if (records is null)
            {
                throw new ArgumentNullException($"{records} is null.");
            }

            this.csvWriter.WriteLine("Id,First Name,Last Name,Date of Birth,Job Experience,Monthly Pay,Gender");

            foreach (FileCabinetRecord record in records)
            {
                this.csvWriter.WriteLine(record.ToString());
            }
        }
    }
}
