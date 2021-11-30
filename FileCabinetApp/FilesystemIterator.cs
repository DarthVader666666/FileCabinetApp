using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides FilesystemIterator.
    /// </summary>
    public class FilesystemIterator : IRecordIterator
    {
        private const int ReservedOffset = sizeof(short);
        private const int IdOffset = sizeof(int);
        private const int FirstNameOffset = 120;
        private const int LastNameOffset = FirstNameOffset;
        private const int YearOffset = sizeof(int);
        private const int MonthOffset = YearOffset;
        private const int DayOffset = YearOffset;
        private const int JobExperienceOffset = sizeof(short);
        private const int MonthlyPayOffset = sizeof(decimal);
        private const int GenderOffset = sizeof(char);

        private readonly string filePath;
        private readonly List<long> positions;
        private int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="positions">List of record position which contain specific key.</param>
        /// <param name="path">Path to the file of recods.</param>
        public FilesystemIterator(List<long> positions, string path)
        {
            this.positions = positions;
            this.filePath = path;
        }

        /// <summary>
        /// Gets record of specific position.
        /// </summary>
        /// <returns>FileCabinetRecord.</returns>
        public FileCabinetRecord GetNext()
        {
            FileCabinetRecord record;
            FileStream fileStream = new FileStream(this.filePath, FileMode.Open, FileAccess.Read);

            byte[] buffer;
            char[] charArray;

            fileStream.Position = this.positions[this.index];

            record = new FileCabinetRecord();

            while (IsDeleted(this.positions[this.index], fileStream) && this.positions[this.index] <= this.positions.Count)
            {
                this.index++;
            }

            fileStream.Seek(ReservedOffset, SeekOrigin.Current);

            try
            {
                buffer = new byte[IdOffset];
                fileStream.Read(buffer, 0, IdOffset);
                record.Id = BitConverter.ToInt32(buffer);

                buffer = new byte[FirstNameOffset];
                fileStream.Read(buffer, 0, FirstNameOffset);
                charArray = Encoding.Unicode.GetChars(buffer);
                record.FirstName = ConvertCharArray(charArray);

                fileStream.Read(buffer, 0, LastNameOffset);
                charArray = Encoding.Unicode.GetChars(buffer);
                record.LastName = ConvertCharArray(charArray);

                buffer = new byte[YearOffset];
                fileStream.Read(buffer, 0, YearOffset);
                int year = BitConverter.ToInt32(buffer);
                fileStream.Read(buffer, 0, MonthOffset);
                int month = BitConverter.ToInt32(buffer);
                fileStream.Read(buffer, 0, DayOffset);
                int day = BitConverter.ToInt32(buffer);
                record.DateOfBirth = new DateTime(year, month, day);

                buffer = new byte[JobExperienceOffset];
                fileStream.Read(buffer, 0, JobExperienceOffset);
                record.JobExperience = BitConverter.ToInt16(buffer);

                buffer = new byte[MonthlyPayOffset];
                fileStream.Read(buffer, 0, MonthlyPayOffset);

                MemoryStream memoryStream = new MemoryStream(buffer);
                BinaryReader binaryReader = new BinaryReader(memoryStream);
                record.MonthlyPay = binaryReader.ReadDecimal();

                memoryStream.Close();
                binaryReader.Close();

                buffer = new byte[GenderOffset];
                fileStream.Read(buffer, 0, GenderOffset);
                record.Gender = BitConverter.ToChar(buffer);

                this.index++;
            }
            catch (ArgumentException)
            {
                fileStream.Close();
                throw new ArgumentException("buffer overflown");
            }

            fileStream.Close();
            return record;
        }

        /// <summary>
        /// Finds out if there are more positions in list.
        /// </summary>
        /// <returns>bool.</returns>
        public bool HasMore()
        {
            if (this.index < this.positions.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string ConvertCharArray(char[] charArray)
        {
            return new string(charArray[0..Array.FindIndex(charArray, 0, charArray.Length, i => i.Equals('\0'))]);
        }

        private static bool IsDeleted(long position, FileStream fileStream)
        {
            fileStream.Position = position;
            byte[] buffer = new byte[1];

            try
            {
                fileStream.Read(buffer);
                fileStream.Position--;
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"{buffer} gone reading out of file ranges.");
            }

            return ((buffer[0] >> 2) & 1) == 1;
        }
    }
}
