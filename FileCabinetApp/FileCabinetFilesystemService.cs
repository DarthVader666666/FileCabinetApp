using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Class contains methods which handle the user commands.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int BufferSize = 278;

        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly IRecordValidator validator = new DefaultValidator();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="filePath">Path to *.db file.</param>
        public FileCabinetFilesystemService(string filePath)
        {
            this.FilePath = filePath;
            this.CheckFile();
            this.GetRecords();
            this.RecordsCount = this.GetStat().Item1;
            this.FillAllDictionaries();
        }

        private string FilePath { get; set; }

        private int RecordsCount { get; set; }

        /// <summary>
        /// Gets max record's id.
        /// </summary>
        /// <returns>Last record's id.</returns>
        public int GetMaxId()
        {
            int maxId = this.list[0].Id;

            foreach (var record in this.list)
            {
                maxId = record.Id > maxId ? record.Id : maxId;
            }

            return maxId;
        }

        /// <summary>
        /// Gets snapshot entity.
        /// </summary>
        /// <returns>Snapshot object.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list);
        }

        /// <summary>
        /// Creates a record about a person.
        /// </summary>
        /// <param name="sender">Sender object is null.</param>
        /// <param name="e">Arguments for record creation.</param>
        public void CreateRecord(object sender, FileCabinetEventArgs e)
        {
            if (e is null)
            {
                throw new ArgumentNullException($"{e} argument is null.");
            }

            FileCabinetRecord record = this.validator.ValidateParameters(e);

            FileStream fileStream = new FileStream(this.FilePath, FileMode.Append, FileAccess.Write);
            WriteRecordToFile(record, fileStream);
            fileStream.Close();

            this.AddRecordToFirstNameDictionary(record, record.FirstName);
            this.AddRecordToLastNameDictionary(record, record.LastName);
            string dateOfBirthKey = $"{record.DateOfBirth.Year}-{record.DateOfBirth.Month}-{record.DateOfBirth.Day}";
            this.AddRecordToDateOfBirthDictionary(record, dateOfBirthKey);

            this.RecordsCount = this.GetStat().Item1;
        }

        /// <summary>
        /// Gets all file cabinet records.
        /// </summary>
        /// <returns>The array of all file cabinet records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            FileStream fileStream = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read);
            fileStream.Position = 0;

            this.list.Clear();

            FileCabinetRecord record;
            byte[] buffer;
            char[] charArray;

            while (fileStream.Position < fileStream.Length)
            {
                if (!IsDeleted(fileStream.Position, fileStream))
                {
                    fileStream.Seek(sizeof(short), SeekOrigin.Current);
                    record = new FileCabinetRecord();

                    try
                    {
                        buffer = new byte[sizeof(int)];
                        fileStream.Read(buffer, 0, sizeof(int));
                        record.Id = BitConverter.ToInt32(buffer);

                        buffer = new byte[120];
                        fileStream.Read(buffer, 0, 120);
                        charArray = Encoding.Unicode.GetChars(buffer);
                        record.FirstName = new string(charArray[0..Array.FindIndex(charArray, 0, charArray.Length, i => i.Equals('\0'))]);

                        fileStream.Read(buffer, 0, 120);
                        charArray = Encoding.Unicode.GetChars(buffer);
                        record.LastName = new string(charArray[0..Array.FindIndex(charArray, 0, charArray.Length, i => i.Equals('\0'))]);

                        buffer = new byte[sizeof(int)];
                        fileStream.Read(buffer, 0, sizeof(int));
                        int year = BitConverter.ToInt32(buffer);
                        fileStream.Read(buffer, 0, sizeof(int));
                        int month = BitConverter.ToInt32(buffer);
                        fileStream.Read(buffer, 0, sizeof(int));
                        int day = BitConverter.ToInt32(buffer);
                        record.DateOfBirth = new DateTime(year, month, day);

                        buffer = new byte[sizeof(short)];
                        fileStream.Read(buffer, 0, sizeof(short));
                        record.JobExperience = BitConverter.ToInt16(buffer);

                        buffer = new byte[sizeof(decimal)];
                        fileStream.Read(buffer, 0, sizeof(decimal));

                        MemoryStream memoryStream = new MemoryStream(buffer);
                        BinaryReader binaryReader = new BinaryReader(memoryStream);
                        record.MonthlyPay = binaryReader.ReadDecimal();

                        memoryStream.Close();
                        binaryReader.Close();

                        buffer = new byte[sizeof(char)];
                        fileStream.Read(buffer, 0, sizeof(char));
                        record.Gender = BitConverter.ToChar(buffer);
                    }
                    catch (ArgumentException)
                    {
                        fileStream.Close();
                        throw new ArgumentException("buffer overflown");
                    }

                    this.list.Add(record);
                }
                else
                {
                    fileStream.Seek(BufferSize, SeekOrigin.Current);
                }
            }

            fileStream.Close();

            return new ReadOnlyCollection<FileCabinetRecord>(this.list);
        }

        /// <summary>
        /// Gets count of all file cabinet records.
        /// </summary>
        /// <returns>Count of all file cabinet records.</returns>
        public Tuple<int, int> GetStat()
        {
            FileStream fileStream = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read);
            int count = 0;
            fileStream.Position = 0;

            while (fileStream.Position < fileStream.Length)
            {
                count++;
                fileStream.Seek(BufferSize, SeekOrigin.Current);
            }

            fileStream.Close();

            return new Tuple<int, int>(count, this.CountDeleted());
        }

        /// <summary>
        /// Edits a record about a person.
        /// </summary>
        /// <param name="sender">Sender object is null.</param>
        /// <param name="recordArgs">Record arguments about a person with new data.</param>
        public void EditRecord(object sender, FileCabinetEventArgs recordArgs)
        {
            if (recordArgs is null)
            {
                throw new ArgumentNullException(nameof(recordArgs), "Record is null");
            }

            FileStream fileStream = new FileStream(this.FilePath, FileMode.Open, FileAccess.ReadWrite);
            long offset;

            if ((offset = SeekRecordPosition(recordArgs.Id, fileStream)) == -1 || recordArgs.Id < 1)
            {
                Console.WriteLine($"Record #{recordArgs.Id} not found");
                fileStream.Close();
                return;
            }

            if (IsDeleted(offset, fileStream))
            {
                Console.WriteLine($"Record #{recordArgs.Id} marked as Deleted. Can't edit.");
                fileStream.Close();
                return;
            }

            FileCabinetRecord record = this.validator.ValidateParameters(recordArgs);
            var oldRecord = this.list[record.Id - 1];
            string dateOfBirthKey = $"{oldRecord.DateOfBirth.Year}-{oldRecord.DateOfBirth.Month}-{oldRecord.DateOfBirth.Day}";

            this.RemoveRecordFromFirstNameDictionary(oldRecord.Id, oldRecord.FirstName);
            this.RemoveRecordFromLastNameDictionary(oldRecord.Id, oldRecord.LastName);
            this.RemoveRecordFromDateOfBirthDictionary(oldRecord.Id, dateOfBirthKey);

            oldRecord.FirstName = record.FirstName;
            oldRecord.LastName = record.LastName;
            oldRecord.DateOfBirth = record.DateOfBirth;
            oldRecord.JobExperience = record.JobExperience;
            oldRecord.MonthlyPay = record.MonthlyPay;
            oldRecord.Gender = record.Gender;

            fileStream.Seek(offset, SeekOrigin.Begin);
            WriteRecordToFile(record, fileStream);
            fileStream.Close();

            this.AddRecordToFirstNameDictionary(record, record.FirstName);
            this.AddRecordToLastNameDictionary(record, record.LastName);
            dateOfBirthKey = $"{record.DateOfBirth.Year}-{record.DateOfBirth.Month}-{record.DateOfBirth.Day}";
            this.AddRecordToDateOfBirthDictionary(record, dateOfBirthKey);

            Console.WriteLine($"Record #{record.Id} is updated.");
        }

        /// <summary>
        /// Finds a record about a person.
        /// </summary>
        /// <param name="firstName">Person's first name which record should be found with.</param>
        /// <returns>Records which fit search requirements.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (!(firstName is null) && this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(this.firstNameDictionary[firstName.ToUpperInvariant()]);
            }
            else
            {
                Console.WriteLine("! No matches found");
            }

            return new List<FileCabinetRecord>().AsReadOnly();
        }

        /// <summary>
        /// Finds a record about a person.
        /// </summary>
        /// <param name="lastName">Person's last name which record should be found with.</param>
        /// <returns>Records which fit search requirements.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (!(lastName is null) && this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(this.lastNameDictionary[lastName.ToUpperInvariant()]);
            }
            else
            {
                Console.WriteLine("! No matches found");
            }

            return new List<FileCabinetRecord>().AsReadOnly();
        }

        /// <summary>
        /// Finds a record about a person.
        /// </summary>
        /// <param name="dateOfBirth">Person's date of birth which record should be found with.</param>
        /// <returns>Records which fit search requirements.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(this.dateOfBirthDictionary[dateOfBirth]);
            }
            else
            {
                Console.WriteLine("! No matches found");
            }

            return new List<FileCabinetRecord>().AsReadOnly();
        }

        /// <summary>
        /// Places records from csv file to current *.db file.
        /// </summary>
        /// <param name="snapshot">Loaded records from csv file.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException($"{snapshot} is null.");
            }

            this.GetRecords();

            int index = -1;
            long offset;

            foreach (FileCabinetRecord record in snapshot.Records)
            {
                index = this.list.FindIndex(i => i.Id.Equals(record.Id));

                if (index != -1)
                {
                    this.list[index] = record;
                }
                else
                {
                    this.list.Add(record);
                }
            }

            this.FillAllDictionaries();

            FileStream fileStream = new FileStream(this.FilePath, FileMode.Open, FileAccess.ReadWrite);

            foreach (FileCabinetRecord record in this.list)
            {
                if ((offset = SeekRecordPosition(record.Id, fileStream)) == -1)
                {
                    fileStream.Seek(0, SeekOrigin.End);
                }
                else
                {
                    fileStream.Seek(offset, SeekOrigin.Begin);
                }

                WriteRecordToFile(record, fileStream);
            }

            fileStream.Close();

            this.RecordsCount = this.GetStat().Item1;
        }

        /// <summary>
        /// Removes record from *.db file.
        /// </summary>
        /// <param name="id">Record's id.</param>
        public void RemoveRecord(int id)
        {
            FileStream fileStream = new FileStream(this.FilePath, FileMode.Open, FileAccess.ReadWrite);
            long position = SeekRecordPosition(id, fileStream);

            if (position == -1)
            {
                Console.WriteLine("Specified record doesn't exist. Can't remove.");
                fileStream.Close();
                return;
            }

            SetIsDeletedBit(position, fileStream);
            fileStream.Close();

            FileCabinetRecord record = this.list.Find(i => i.Id.Equals(id));

            this.list.Remove(record);

            foreach (KeyValuePair<string, List<FileCabinetRecord>> pair in this.firstNameDictionary)
            {
                pair.Value.Remove(record);
            }

            foreach (KeyValuePair<string, List<FileCabinetRecord>> pair in this.lastNameDictionary)
            {
                pair.Value.Remove(record);
            }

            foreach (KeyValuePair<string, List<FileCabinetRecord>> pair in this.dateOfBirthDictionary)
            {
                pair.Value.Remove(record);
            }

            this.RecordsCount = this.GetStat().Item1;

            Console.WriteLine($"Record #{id} removed.");
        }

        /// <summary>
        /// Purges *.db file.
        /// </summary>
        public void PurgeFile()
        {
            int deleted = this.CountDeleted();
            this.GetRecords();

            FileStream fileStream = new FileStream(this.FilePath, FileMode.Truncate, FileAccess.Write);

            foreach (FileCabinetRecord record in this.list)
            {
                WriteRecordToFile(record, fileStream);
            }

            fileStream.Close();

            Console.WriteLine($"Data file processing is completed: {deleted} of {this.RecordsCount} records were purged.");
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

        private static long SeekRecordPosition(int id, FileStream fileStream)
        {
            fileStream.Position = 0;
            byte[] buffer;
            int id_file;

            while (fileStream.Position < fileStream.Length)
            {
                fileStream.Seek(sizeof(short), SeekOrigin.Current);

                buffer = new byte[sizeof(int)];
                fileStream.Read(buffer, 0, sizeof(int));
                id_file = BitConverter.ToInt32(buffer);

                if (id_file == id)
                {
                    return fileStream.Position - sizeof(short) - sizeof(int);
                }
                else
                {
                    fileStream.Seek(BufferSize - sizeof(short) - sizeof(int), SeekOrigin.Current);
                }
            }

            return -1;
        }

        private static void SetIsDeletedBit(long position, FileStream fileStream)
        {
            fileStream.Position = position;
            byte[] reserved = new byte[1];
            fileStream.Read(reserved, 0, 1);
            reserved[0] |= 1 << 2;
            fileStream.Position -= 1;
            fileStream.Write(reserved);
        }

        private static void WriteRecordToFile(FileCabinetRecord record, FileStream fileStream)
        {
            Encoding unicode = Encoding.Unicode;
            byte[] byteArray = new byte[BufferSize];
            byte[] bufferBytes;

            try
            {
                new byte[2].CopyTo(byteArray, 0);
                BitConverter.GetBytes(record.Id).CopyTo(byteArray, 2);

                bufferBytes = new byte[120];
                unicode.GetBytes(record.FirstName.ToCharArray(), 0, record.FirstName.Length, bufferBytes, 0);
                bufferBytes.CopyTo(byteArray, 6);

                bufferBytes = new byte[120];
                unicode.GetBytes(record.LastName.ToCharArray(), 0, record.LastName.Length, bufferBytes, 0);
                bufferBytes.CopyTo(byteArray, 126);

                BitConverter.GetBytes(record.DateOfBirth.Year).CopyTo(byteArray, 246);
                BitConverter.GetBytes(record.DateOfBirth.Month).CopyTo(byteArray, 250);
                BitConverter.GetBytes(record.DateOfBirth.Day).CopyTo(byteArray, 254);

                BitConverter.GetBytes(record.JobExperience).CopyTo(byteArray, 258);

                MemoryStream memoryStream = new MemoryStream();
                BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.Write(record.MonthlyPay);
                memoryStream.ToArray().CopyTo(byteArray, 260);

                memoryStream.Close();
                binaryWriter.Close();

                bufferBytes = new byte[sizeof(char)];
                unicode.GetBytes(new char[] { record.Gender }, 0, 1, bufferBytes, 0);
                bufferBytes.CopyTo(byteArray, 276);

                fileStream.Write(byteArray, 0, sizeof(short));
                fileStream.Write(byteArray, 2, sizeof(int));
                fileStream.Write(byteArray, 6, 120);
                fileStream.Write(byteArray, 126, 120);
                fileStream.Write(byteArray, 246, sizeof(int));
                fileStream.Write(byteArray, 250, sizeof(int));
                fileStream.Write(byteArray, 254, sizeof(int));
                fileStream.Write(byteArray, 258, sizeof(short));
                fileStream.Write(byteArray, 260, sizeof(decimal));
                fileStream.Write(byteArray, 276, sizeof(char));
            }
            catch (ArgumentException)
            {
                fileStream.Close();
                throw new ArgumentException("buffer overflown");
            }
        }

        private int CountDeleted()
        {
            int count = 0;
            FileStream fileStream = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);

            while (fileStream.Position < fileStream.Length)
            {
                if (IsDeleted(fileStream.Position, fileStream))
                {
                    count++;
                }

                fileStream.Seek(BufferSize, SeekOrigin.Current);
            }

            fileStream.Close();

            return count;
        }

        private void FillAllDictionaries()
        {
            string dateOfBirthKey;
            this.firstNameDictionary.Clear();
            this.lastNameDictionary.Clear();
            this.dateOfBirthDictionary.Clear();

            foreach (FileCabinetRecord record in this.list)
            {
                this.AddRecordToFirstNameDictionary(record, record.FirstName);
                this.AddRecordToLastNameDictionary(record, record.LastName);
                dateOfBirthKey = $"{record.DateOfBirth.Year}-{record.DateOfBirth.Month}-{record.DateOfBirth.Day}";
                this.AddRecordToDateOfBirthDictionary(record, dateOfBirthKey);
            }
        }

        private void CheckFile()
        {
            FileStream fileStream = new FileStream(this.FilePath, FileMode.OpenOrCreate);
            fileStream.Close();
        }

        private void AddRecordToFirstNameDictionary(FileCabinetRecord record, string firstNameKey)
        {
            if (firstNameKey is null)
            {
                throw new ArgumentNullException(nameof(firstNameKey), "Dictionary key is null");
            }

            if (firstNameKey.Length == 0)
            {
                throw new ArgumentException("The method gets no key!");
            }

            if (!this.firstNameDictionary.ContainsKey(firstNameKey.ToUpperInvariant()))
            {
                this.firstNameDictionary.Add(firstNameKey.ToUpperInvariant(), new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[firstNameKey.ToUpperInvariant()].Add(record);
        }

        private void AddRecordToLastNameDictionary(FileCabinetRecord record, string lastNameKey)
        {
            if (lastNameKey is null)
            {
                throw new ArgumentNullException(nameof(lastNameKey), "Dictionary key is null");
            }

            if (lastNameKey.Length == 0)
            {
                throw new ArgumentException("The method gets no key!");
            }

            if (!this.lastNameDictionary.ContainsKey(lastNameKey.ToUpperInvariant()))
            {
                this.lastNameDictionary.Add(lastNameKey.ToUpperInvariant(), new List<FileCabinetRecord>());
            }

            this.lastNameDictionary[lastNameKey.ToUpperInvariant()].Add(record);
        }

        private void AddRecordToDateOfBirthDictionary(FileCabinetRecord record, string dateOfBirthKey)
        {
            if (dateOfBirthKey is null)
            {
                throw new ArgumentNullException(nameof(dateOfBirthKey), "Dictionary key is null");
            }

            if (dateOfBirthKey.Length == 0)
            {
                throw new ArgumentException("The method gets no key!");
            }

            if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirthKey.ToUpperInvariant()))
            {
                this.dateOfBirthDictionary.Add(dateOfBirthKey.ToUpperInvariant(), new List<FileCabinetRecord>());
            }

            this.dateOfBirthDictionary[dateOfBirthKey.ToUpperInvariant()].Add(record);
        }

        private void RemoveRecordFromFirstNameDictionary(int id, string firstNameKey)
        {
            if (firstNameKey is null)
            {
                throw new ArgumentNullException(nameof(firstNameKey), "Dictionary key is null");
            }

            if (firstNameKey.Length == 0)
            {
                throw new ArgumentException("The method gets no key!");
            }

            FileCabinetRecord recordToRemove = this.firstNameDictionary[firstNameKey.ToUpperInvariant()].Find(i => i.Id == id);

            if (!(recordToRemove is null))
            {
                this.firstNameDictionary[firstNameKey.ToUpperInvariant()].Remove(recordToRemove);
            }
        }

        private void RemoveRecordFromLastNameDictionary(int id, string lastNameKey)
        {
            if (lastNameKey is null)
            {
                throw new ArgumentNullException(nameof(lastNameKey), "Dictionary key is null");
            }

            if (lastNameKey.Length == 0)
            {
                throw new ArgumentException("The method gets no key!");
            }

            FileCabinetRecord recordToRemove = this.lastNameDictionary[lastNameKey.ToUpperInvariant()].Find(i => i.Id == id);

            if (!(recordToRemove is null))
            {
                this.lastNameDictionary[lastNameKey.ToUpperInvariant()].Remove(recordToRemove);
            }
        }

        private void RemoveRecordFromDateOfBirthDictionary(int id, string dateOfBirthKey)
        {
            if (dateOfBirthKey is null)
            {
                throw new ArgumentNullException(nameof(dateOfBirthKey), "Dictionary key is null");
            }

            if (dateOfBirthKey.Length == 0)
            {
                throw new ArgumentException("The method gets no key!");
            }

            FileCabinetRecord recordToRemove = this.dateOfBirthDictionary[dateOfBirthKey.ToUpperInvariant()].Find(i => i.Id == id);

            if (!(recordToRemove is null))
            {
                this.dateOfBirthDictionary[dateOfBirthKey.ToUpperInvariant()].Remove(recordToRemove);
            }
        }
    }
}