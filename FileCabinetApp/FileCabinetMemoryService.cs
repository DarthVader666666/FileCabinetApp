using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

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
        private readonly Dictionary<string, List<FileCabinetRecord>> jobExperienceDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> monthlyPayDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> genderDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameCache = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameCache = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthCache = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> jobExperienceCache = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> monthlyPayCache = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> genderCache = new Dictionary<string, List<FileCabinetRecord>>();

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
        /// Gets new record's id.
        /// </summary>
        /// <returns>New record's id.</returns>
        public int GetNewId()
        {
            IEnumerable<int> ids = from record in this.list select record.Id;
            ids.ToList().Sort();
            int id = 1;

            for (; id <= ids.Count(); id++)
            {
                if (!ids.Contains(id))
                {
                    return id;
                }
            }

            return id;
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

            this.UpdateDictionaries();
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

            oldRecord.FirstName = record.FirstName;
            oldRecord.LastName = record.LastName;
            oldRecord.DateOfBirth = record.DateOfBirth;
            oldRecord.JobExperience = record.JobExperience;
            oldRecord.MonthlyPay = record.MonthlyPay;
            oldRecord.Gender = record.Gender;

            this.UpdateDictionaries();

            Console.WriteLine($"Record #{record.Id} is updated.");
        }

        /// <summary>
        /// Finds a records by FirstName.
        /// </summary>
        /// <param name="firstName">Person's first name which record should be found with.</param>
        /// <returns>Records which fit search requirements.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (firstName is null)
            {
                Console.WriteLine("Find method didn't get a key.");
                return new List<FileCabinetRecord>();
            }

            if (this.firstNameCache.ContainsKey(firstName.ToUpperInvariant()))
            {
                return this.firstNameCache[firstName.ToUpperInvariant()];
            }

            if (this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
            {
                if (!this.firstNameCache.ContainsKey(firstName.ToUpperInvariant()))
                {
                    this.firstNameCache.Add(firstName.ToUpperInvariant(), new RecordsFound(new MemoryIterator(this.firstNameDictionary[firstName.ToUpperInvariant()])).ToList());
                }

                return this.firstNameCache[firstName.ToUpperInvariant()];
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
            if (lastName is null)
            {
                Console.WriteLine("Find method didn't get a key.");
                return new List<FileCabinetRecord>();
            }

            if (this.lastNameCache.ContainsKey(lastName.ToUpperInvariant()))
            {
                return this.lastNameCache[lastName.ToUpperInvariant()];
            }

            if (this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
            {
                if (!this.lastNameCache.ContainsKey(lastName.ToUpperInvariant()))
                {
                    this.lastNameCache.Add(lastName.ToUpperInvariant(), new RecordsFound(new MemoryIterator(this.lastNameDictionary[lastName.ToUpperInvariant()])).ToList());
                }

                return this.lastNameCache[lastName.ToUpperInvariant()];
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
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            string dateOfBirthKey = $"{dateOfBirth.Year}-{dateOfBirth.Month}-{dateOfBirth.Day}";

            if (this.dateOfBirthCache.ContainsKey(dateOfBirthKey))
            {
                return this.dateOfBirthCache[dateOfBirthKey];
            }

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirthKey))
            {
                if (!this.dateOfBirthCache.ContainsKey(dateOfBirthKey))
                {
                    this.dateOfBirthCache.Add(dateOfBirthKey, new RecordsFound(new MemoryIterator(this.dateOfBirthDictionary[dateOfBirthKey])).ToList());
                }

                return this.dateOfBirthCache[dateOfBirthKey];
            }
            else
            {
                Console.WriteLine("! No matches found");
            }

            return new List<FileCabinetRecord>();
        }

        /// <summary>
        /// Finds records by jobExperience.
        /// </summary>
        /// <param name="jobExperience">Person's jobExperience.</param>
        /// <returns>Records which fit search requirements.</returns>
        public IEnumerable<FileCabinetRecord> FindByJobExperience(short jobExperience)
        {
            string jobExperienceKey = jobExperience.ToString(CultureInfo.InvariantCulture);

            if (this.jobExperienceCache.ContainsKey(jobExperienceKey))
            {
                return this.jobExperienceCache[jobExperienceKey];
            }

            if (this.jobExperienceDictionary.ContainsKey(jobExperienceKey))
            {
                if (!this.jobExperienceCache.ContainsKey(jobExperienceKey))
                {
                    this.jobExperienceCache.Add(jobExperienceKey, new RecordsFound(new MemoryIterator(this.jobExperienceDictionary[jobExperienceKey])).ToList());
                }

                return this.jobExperienceCache[jobExperienceKey];
            }
            else
            {
                Console.WriteLine("! No matches found");
            }

            return new List<FileCabinetRecord>();
        }

        /// <summary>
        /// Finds records by monthlyPay.
        /// </summary>
        /// <param name="monthlyPay">Person's monthlyPay.</param>
        /// <returns>Records which fit search requirements.</returns>
        public IEnumerable<FileCabinetRecord> FindByMonthlyPay(decimal monthlyPay)
        {
            string monthlyPayKey = monthlyPay.ToString(CultureInfo.InvariantCulture);

            if (this.monthlyPayCache.ContainsKey(monthlyPayKey))
            {
                return this.monthlyPayCache[monthlyPayKey];
            }

            if (this.monthlyPayDictionary.ContainsKey(monthlyPayKey))
            {
                if (!this.monthlyPayCache.ContainsKey(monthlyPayKey))
                {
                    this.monthlyPayCache.Add(monthlyPayKey, new RecordsFound(new MemoryIterator(this.monthlyPayDictionary[monthlyPayKey])).ToList());
                }

                return this.monthlyPayCache[monthlyPayKey];
            }
            else
            {
                Console.WriteLine("! No matches found");
            }

            return new List<FileCabinetRecord>();
        }

        /// <summary>
        /// Finds records by gender.
        /// </summary>
        /// <param name="gender">Person's gender.</param>
        /// <returns>Records which fit search requirements.</returns>
        public IEnumerable<FileCabinetRecord> FindByGender(char gender)
        {
            string genderKey = gender.ToString(CultureInfo.InvariantCulture);

            if (this.genderCache.ContainsKey(genderKey))
            {
                return this.genderCache[genderKey];
            }

            if (this.genderDictionary.ContainsKey(genderKey))
            {
                if (!this.genderCache.ContainsKey(genderKey))
                {
                    this.genderCache.Add(genderKey, new RecordsFound(new MemoryIterator(this.genderDictionary[genderKey])).ToList());
                }

                return this.genderCache[genderKey];
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

            this.UpdateDictionaries();
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
            this.UpdateDictionaries();
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void PurgeFile()
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// Clears FileService cache.
        /// </summary>
        public void ClearCache()
        {
            this.firstNameCache.Clear();
            this.lastNameCache.Clear();
            this.dateOfBirthCache.Clear();
            this.jobExperienceCache.Clear();
            this.monthlyPayCache.Clear();
            this.genderCache.Clear();
        }

        private void UpdateDictionaries()
        {
            this.firstNameDictionary.Clear();
            this.lastNameDictionary.Clear();
            this.dateOfBirthDictionary.Clear();
            this.jobExperienceDictionary.Clear();
            this.monthlyPayDictionary.Clear();
            this.genderDictionary.Clear();

            foreach (FileCabinetRecord record in this.list)
            {
                this.AddRecordToFirstNameDictionary(record, record.FirstName);
                this.AddRecordToLastNameDictionary(record, record.LastName);
                this.AddRecordToDateOfBirthDictionary(record, record.DateOfBirth);
                this.AddRecordToJobExperienceDictionary(record, record.JobExperience);
                this.AddRecordToMonthlyPayDictionary(record, record.MonthlyPay);
                this.AddRecordToGenderDictionary(record, record.Gender);
            }
        }

        private void AddRecordToFirstNameDictionary(FileCabinetRecord record, string firstNameKey)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record is null");
            }

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
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record is null");
            }

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

        private void AddRecordToDateOfBirthDictionary(FileCabinetRecord record, DateTime dateOfBirth)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record is null");
            }

            if (dateOfBirth.Equals(default(DateTime)))
            {
                throw new ArgumentException("Dictionary key is invalid");
            }

            string dateOfBirthKey = $"{dateOfBirth.Year}-{dateOfBirth.Month}-{dateOfBirth.Day}";

            if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirthKey.ToUpperInvariant()))
            {
                this.dateOfBirthDictionary.Add(dateOfBirthKey.ToUpperInvariant(), new List<FileCabinetRecord>());
            }

            this.dateOfBirthDictionary[dateOfBirthKey.ToUpperInvariant()].Add(record);
        }

        private void AddRecordToJobExperienceDictionary(FileCabinetRecord record, short jobExperience)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record is null");
            }

            string jobExperienceKey = jobExperience.ToString(CultureInfo.InvariantCulture);

            if (!this.jobExperienceDictionary.ContainsKey(jobExperienceKey))
            {
                this.jobExperienceDictionary.Add(jobExperienceKey, new List<FileCabinetRecord>());
            }

            this.jobExperienceDictionary[jobExperienceKey].Add(record);
        }

        private void AddRecordToMonthlyPayDictionary(FileCabinetRecord record, decimal monthlyPay)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record is null");
            }

            string monthlyPayKey = monthlyPay.ToString(CultureInfo.InvariantCulture);

            if (!this.monthlyPayDictionary.ContainsKey(monthlyPayKey))
            {
                this.monthlyPayDictionary.Add(monthlyPayKey, new List<FileCabinetRecord>());
            }

            this.monthlyPayDictionary[monthlyPayKey].Add(record);
        }

        private void AddRecordToGenderDictionary(FileCabinetRecord record, char gender)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record is null");
            }

            string genderKey = gender.ToString(CultureInfo.InvariantCulture);

            if (!this.genderDictionary.ContainsKey(genderKey))
            {
                this.genderDictionary.Add(genderKey, new List<FileCabinetRecord>());
            }

            this.genderDictionary[genderKey].Add(record);
        }
    }
}