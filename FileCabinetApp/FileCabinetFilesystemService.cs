using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Validators.CompositeValidator validator = new Validators.ValidatorBuilder().CreateDefault();

        private readonly Dictionary<string, List<long>> firstNameIndex = new Dictionary<string, List<long>>();
        private readonly Dictionary<string, List<long>> lastNameIndex = new Dictionary<string, List<long>>();
        private readonly Dictionary<string, List<long>> dateOfBirthIndex = new Dictionary<string, List<long>>();

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
            this.UpdateIndexes();
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
        /// Finds out if record exists in file servise list.
        /// </summary>
        /// <param name="id">User input id.</param>
        /// <returns>true - record exists, false - record doesn't exist.</returns>
        public bool RecordExists(int id)
        {
            if (this.list.Find(i => i.Id.Equals(id)) is null)
            {
                return false;
            }
            else
            {
                return true;
            }
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

            FileCabinetRecord record = (FileCabinetRecord)this.validator.ValidateParameters(e);

            FileStream fileStream = new FileStream(this.FilePath, FileMode.Append, FileAccess.Write);
            WriteRecordToFile(record, fileStream);
            fileStream.Close();

            this.UpdateIndexes();

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
                    fileStream.Seek(ReservedOffset, SeekOrigin.Current);
                    record = new FileCabinetRecord();

                    try
                    {
                        buffer = new byte[sizeof(int)];
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

            FileCabinetRecord record = (FileCabinetRecord)this.validator.ValidateParameters(recordArgs);
            var oldRecord = this.list[record.Id - 1];

            oldRecord.FirstName = record.FirstName;
            oldRecord.LastName = record.LastName;
            oldRecord.DateOfBirth = record.DateOfBirth;
            oldRecord.JobExperience = record.JobExperience;
            oldRecord.MonthlyPay = record.MonthlyPay;
            oldRecord.Gender = record.Gender;

            fileStream.Seek(offset, SeekOrigin.Begin);
            WriteRecordToFile(record, fileStream);
            fileStream.Close();

            this.UpdateIndexes();

            Console.WriteLine($"Record #{record.Id} is updated.");
        }

        /// <summary>
        /// Gets iterator based on firstName key.
        /// </summary>
        /// <param name="firstName">Person's first name which record should be found with.</param>
        /// <returns>Records which fit search requirements.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException($"{firstName} is null");
            }

            List<long> positions;

            if (!this.firstNameIndex.TryGetValue(firstName.ToUpper(CultureInfo.InvariantCulture), out positions))
            {
                Console.WriteLine("! No matches found");
                return new List<FileCabinetRecord>();
            }

            return new RecordsFound(new FilesystemIterator(positions, this.FilePath));
        }

        /// <summary>
        /// Gets iterator based on lastName key.
        /// </summary>
        /// <param name="lastName">Person's last name which record should be found with.</param>
        /// <returns>Records which fit search requirements.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (lastName is null)
            {
                throw new ArgumentNullException($"{lastName} is null");
            }

            List<long> positions;

            if (!this.lastNameIndex.TryGetValue(lastName.ToUpper(CultureInfo.InvariantCulture), out positions))
            {
                Console.WriteLine("! No matches found");
                return new List<FileCabinetRecord>();
            }

            return new RecordsFound(new FilesystemIterator(positions, this.FilePath));
        }

        /// <summary>
        /// Gets iterator based on dateOfBirth key.
        /// </summary>
        /// <param name="dateOfBirth">Person's date of birth which record should be found with.</param>
        /// <returns>Records which fit search requirements.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            if (dateOfBirth is null)
            {
                throw new ArgumentNullException($"{dateOfBirth} is null");
            }

            List<long> positions;

            if (!this.dateOfBirthIndex.TryGetValue(dateOfBirth.ToUpper(CultureInfo.InvariantCulture), out positions))
            {
                Console.WriteLine("! No matches found");
                return new List<FileCabinetRecord>();
            }

            return new RecordsFound(new FilesystemIterator(positions, this.FilePath));
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

            this.UpdateIndexes();

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

            this.UpdateIndexes();

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

        private static string ConvertCharArray(char[] charArray)
        {
            return new string(charArray[0..Array.FindIndex(charArray, 0, charArray.Length, i => i.Equals('\0'))]);
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

        private void UpdateIndexes()
        {
            byte[] buffer;
            char[] charArray;
            string stringKey;

            this.firstNameIndex.Clear();
            this.lastNameIndex.Clear();
            this.dateOfBirthIndex.Clear();

            FileStream fileStream = new FileStream(this.FilePath, FileMode.OpenOrCreate, FileAccess.Read);

            try
            {
                while (fileStream.Position < fileStream.Length)
                {
                    if (!IsDeleted(fileStream.Position, fileStream))
                    {
                        fileStream.Seek(ReservedOffset + IdOffset, SeekOrigin.Current);

                        buffer = new byte[FirstNameOffset];
                        fileStream.Read(buffer, 0, FirstNameOffset);
                        charArray = Encoding.Unicode.GetChars(buffer);
                        stringKey = ConvertCharArray(charArray).ToUpper(CultureInfo.InvariantCulture);

                        if (!this.firstNameIndex.ContainsKey(stringKey))
                        {
                            this.firstNameIndex.Add(stringKey, new List<long>());
                        }

                        this.firstNameIndex[stringKey].Add(fileStream.Position - FirstNameOffset - IdOffset - ReservedOffset);

                        buffer = new byte[LastNameOffset];
                        fileStream.Read(buffer, 0, LastNameOffset);
                        charArray = Encoding.Unicode.GetChars(buffer);
                        stringKey = ConvertCharArray(charArray).ToUpper(CultureInfo.InvariantCulture);

                        if (!this.lastNameIndex.ContainsKey(stringKey))
                        {
                            this.lastNameIndex.Add(stringKey, new List<long>());
                        }

                        this.lastNameIndex[stringKey].Add(fileStream.Position - LastNameOffset - FirstNameOffset - IdOffset - ReservedOffset);

                        buffer = new byte[YearOffset];
                        fileStream.Read(buffer, 0, YearOffset);
                        int year = BitConverter.ToInt32(buffer);
                        fileStream.Read(buffer, 0, MonthOffset);
                        int month = BitConverter.ToInt32(buffer);
                        fileStream.Read(buffer, 0, DayOffset);
                        int day = BitConverter.ToInt32(buffer);
                        stringKey = $"{year}-{month}-{day}";

                        if (!this.dateOfBirthIndex.ContainsKey(stringKey))
                        {
                            this.dateOfBirthIndex.Add(stringKey, new List<long>());
                        }

                        this.dateOfBirthIndex[stringKey].Add(fileStream.Position - YearOffset - MonthOffset - DayOffset - LastNameOffset - FirstNameOffset - IdOffset - ReservedOffset);

                        fileStream.Seek(JobExperienceOffset + MonthlyPayOffset + GenderOffset, SeekOrigin.Current);
                    }
                    else
                    {
                        fileStream.Seek(BufferSize, SeekOrigin.Current);
                    }
                }
            }
            catch (ArgumentException message)
            {
                Console.WriteLine(message);
                this.firstNameIndex.Clear();
                this.lastNameIndex.Clear();
                this.dateOfBirthIndex.Clear();
                fileStream.Close();
                return;
            }

            fileStream.Close();
        }

        private void CheckFile()
        {
            FileStream fileStream = new FileStream(this.FilePath, FileMode.OpenOrCreate);
            fileStream.Close();
        }
    }
}