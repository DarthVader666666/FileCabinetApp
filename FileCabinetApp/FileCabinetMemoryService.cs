using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// Class contains methods which handle the user commands.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Validators.CompositeValidator validator;
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">Validator object to implement.</param>
        public FileCabinetMemoryService(Validators.CompositeValidator validator)
        {
            this.validator = validator;
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
        /// Gets max record's id.
        /// </summary>
        /// <returns>Last record's id.</returns>
        public int GetMaxId()
        {
            if (this.list.Count == 0)
            {
                return 0;
            }

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
        /// Creates a record about a person.
        /// </summary>
        /// <param name="sender">Sender object is null.</param>
        /// <param name="e">Arguments for record creation.</param>
        public void CreateRecord(object sender, FileCabinetEventArgs e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e), "Record argument is null");
            }

            FileCabinetRecord record = (FileCabinetRecord)this.validator.ValidateParameters(e);
            this.list.Add(record);

            var dateOfBirthKey = $"{record.DateOfBirth.Year}-{record.DateOfBirth.Month}-{record.DateOfBirth.Day}";

            this.AddRecordToFirstNameDictionary(record, record.FirstName);
            this.AddRecordToLastNameDictionary(record, record.LastName);
            this.AddRecordToDateOfBirthDictionary(record, dateOfBirthKey);
        }

        /// <summary>
        /// Gets all file cabinet records.
        /// </summary>
        /// <returns>The array of all file cabinet records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.list);
        }

        /// <summary>
        /// Gets count of all file cabinet records.
        /// </summary>
        /// <returns>Count of all file cabinet records.</returns>
        public Tuple<int, int> GetStat()
        {
            return new Tuple<int, int>(this.list.Count, 0);
        }

        /// <summary>
        /// Updates a record about a person.
        /// </summary>
        /// <param name="sender">Sender object is null.</param>
        /// <param name="recordArgs">Record arguments about a person with new data.</param>
        public void UpdateRecord(object sender, FileCabinetEventArgs recordArgs)
        {
            if (recordArgs is null)
            {
                throw new ArgumentNullException(nameof(recordArgs), "Record is null");
            }

            if (recordArgs.Id > this.GetStat().Item1 || recordArgs.Id < 1)
            {
                throw new ArgumentException("No such record");
            }

            FileCabinetRecord record;

            try
            {
                record = (FileCabinetRecord)this.validator.ValidateParameters(recordArgs);
            }
            catch (ArgumentException message)
            {
                Console.WriteLine(message);
                return;
            }

            var oldRecord = Array.Find(this.list.ToArray(), i => i.Id == record.Id);
            var dateOfBirthKey = $"{oldRecord.DateOfBirth.Year}-{oldRecord.DateOfBirth.Month}-{oldRecord.DateOfBirth.Day}";

            this.RemoveRecordFromFirstNameDictionary(oldRecord.Id, oldRecord.FirstName);
            this.RemoveRecordFromLastNameDictionary(oldRecord.Id, oldRecord.LastName);
            this.RemoveRecordFromDateOfBirthDictionary(oldRecord.Id, dateOfBirthKey);

            oldRecord.FirstName = record.FirstName;
            oldRecord.LastName = record.LastName;
            oldRecord.DateOfBirth = record.DateOfBirth;
            oldRecord.JobExperience = record.JobExperience;
            oldRecord.MonthlyPay = record.MonthlyPay;
            oldRecord.Gender = record.Gender;

            dateOfBirthKey = $"{record.DateOfBirth.Year}-{record.DateOfBirth.Month}-{record.DateOfBirth.Day}";

            this.AddRecordToFirstNameDictionary(record, record.FirstName);
            this.AddRecordToLastNameDictionary(record, record.LastName);
            this.AddRecordToDateOfBirthDictionary(record, dateOfBirthKey);

            Console.WriteLine($"Record #{record.Id} is updated.");
        }

        /// <summary>
        /// Finds a records by FirstName.
        /// </summary>
        /// <param name="firstName">Person's first name which record should be found with.</param>
        /// <returns>Records which fit search requirements.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (!(firstName is null) && this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
            {
                return new RecordsFound(new MemoryIterator(this.firstNameDictionary[firstName.ToUpperInvariant()]));
            }
            else
            {
                Console.WriteLine("! No matches found");
            }

            return new List<FileCabinetRecord>();
        }

        /// <summary>
        /// Finds a records by LastName.
        /// </summary>
        /// <param name="lastName">Person's last name which record should be found with.</param>
        /// <returns>Records which fit search requirements.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (!(lastName is null) && this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
            {
                return new RecordsFound(new MemoryIterator(this.lastNameDictionary[lastName.ToUpperInvariant()]));
            }
            else
            {
                Console.WriteLine("! No matches found");
            }

            return new List<FileCabinetRecord>();
        }

        /// <summary>
        /// Finds a records by dateOfBirth.
        /// </summary>
        /// <param name="dateOfBirth">Person's date of birth which record should be found with.</param>
        /// <returns>Records which fit search requirements.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            if (!(dateOfBirth is null) && this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                return new RecordsFound(new MemoryIterator(this.dateOfBirthDictionary[dateOfBirth]));
            }
            else
            {
                Console.WriteLine("! No matches found");
            }

            return new List<FileCabinetRecord>();
        }

        /// <summary>
        /// Places records from csv file to current record list.
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
        }

        /// <summary>
        /// Deletes record from record list and all dictionaries.
        /// </summary>
        /// <param name="id">Record's id.</param>
        public void DeleteRecord(int id)
        {
            FileCabinetRecord record;
            record = this.list.Find(i => i.Id.Equals(id));

            if (record is null)
            {
                Console.WriteLine("Specified record doesn't exist. Can't remove.");
                return;
            }

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
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void PurgeFile()
        {
            // Method intentionally left empty.
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