using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Writes statistics to the log file.
    /// </summary>
    public class ServiceLogger : IFileCabinetService
    {
        private static FileStream fileStream;
        private static TextWriter logger;
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="service">FileCabinetService instance.</param>
        public ServiceLogger(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Creates new file record.
        /// </summary>
        /// <param name="sender">Object that sends the arguments is null.</param>
        /// <param name="e">File record arguments.</param>
        public void CreateRecord(object sender, FileCabinetEventArgs e)
        {
            CreateStreams();

            if (e is null)
            {
                logger.WriteLine(GetNowDateTime() + $"Calling CreateRecord() {e} argument is null.");
                DisposeStreams();
                throw new ArgumentNullException($"{e} argument is null.");
            }

            try
            {
                logger.WriteLine(
                    GetNowDateTime() + $"Calling CreateRecord() with FirstName = '{e.FirstName}', LastName = '{e.LastName}', " +
                    $"DateOfBirth = '{e.DateOfBirth.Day}/{e.DateOfBirth.Month}/{e.DateOfBirth.Year}'.");

                this.service.CreateRecord(sender, e);
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return;
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return;
            }

            logger.WriteLine(GetNowDateTime() + $"CreateRecord() added record #{e.Id} to record list.");
            DisposeStreams();
        }

        /// <summary>
        /// Wraps local FileCabinetRecord List into ReadonlyCollection.
        /// </summary>
        /// <returns>ReadonlyCollection of file records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            ReadOnlyCollection<FileCabinetRecord> record;
            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + "Calling GetRecords().");
                record = this.service.GetRecords();
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return new List<FileCabinetRecord>().AsReadOnly();
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return new List<FileCabinetRecord>().AsReadOnly();
            }

            logger.WriteLine(GetNowDateTime() + $"GetRecords() returned {record.Count} records wrapped in ReadOnlyCollection.");
            DisposeStreams();

            return record;
        }

        /// <summary>
        /// Gets total count of file records.
        /// </summary>
        /// <returns>Number of file records.</returns>
        public Tuple<int, int> GetStat()
        {
            Tuple<int, int> stat = Tuple.Create<int, int>(0, 0);

            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + "Calling GetStat().");
                stat = this.service.GetStat();
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return stat;
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return stat;
            }

            logger.WriteLine(GetNowDateTime() + $"GetStat() returned count of records: {stat.Item1} and count of marked as deleted: {stat.Item2}");
            DisposeStreams();

            return stat;
        }

        /// <summary>
        /// Updates file record.
        /// </summary>
        /// <param name="sender">Object that sends the arguments is null.</param>
        /// <param name="recordArgs">File record arguments.</param>
        public void UpdateRecord(object sender, FileCabinetEventArgs recordArgs)
        {
            CreateStreams();

            if (recordArgs is null)
            {
                logger.WriteLine(GetNowDateTime() + $"CallingEditRecord(): {recordArgs} is null.");
                DisposeStreams();
                throw new ArgumentNullException($"{recordArgs} is null.");
            }

            try
            {
                logger.WriteLine(
                    GetNowDateTime() + $"Calling EditRecord() with FirstName = '{recordArgs.FirstName}', LastName = '{recordArgs.LastName}', " +
                    $"DateOfBirth = '{recordArgs.DateOfBirth.Day}/{recordArgs.DateOfBirth.Month}/{recordArgs.DateOfBirth.Year}'.");
                this.service.UpdateRecord(sender, recordArgs);
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return;
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return;
            }

            logger.WriteLine(GetNowDateTime() + $"EditRecord() updated record #{recordArgs.Id}.");
            DisposeStreams();
        }

        /// <summary>
        /// Finds record by First Name.
        /// </summary>
        /// <param name="firstName">Person's first name.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            IEnumerable<FileCabinetRecord> recordsFound;

            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling FindByFirstName() with key = '{firstName}'.");
                recordsFound = this.service.FindByFirstName(firstName);
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return new List<FileCabinetRecord>();
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return new List<FileCabinetRecord>();
            }

            if (((List<FileCabinetRecord>)recordsFound).Count == 0)
            {
                logger.WriteLine(GetNowDateTime() + $"FindByFirstName() found no match with key = '{firstName}' and returned null.");
                DisposeStreams();
                return recordsFound;
            }

            logger.WriteLine(GetNowDateTime() + $"FindByFirstName() returned IRecordIterator which match key = '{firstName}'.");
            DisposeStreams();

            return recordsFound;
        }

        /// <summary>
        /// Finds record by Last Name.
        /// </summary>
        /// <param name="lastName">Person's last name.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            IEnumerable<FileCabinetRecord> recordsFound;

            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling FindByLastName() with key = '{lastName}'.");
                recordsFound = this.service.FindByLastName(lastName);
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return new List<FileCabinetRecord>();
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return new List<FileCabinetRecord>();
            }

            if (((List<FileCabinetRecord>)recordsFound).Count == 0)
            {
                logger.WriteLine(GetNowDateTime() + $"FindByFirstName() found no match with key = '{lastName}' and returned null.");
                DisposeStreams();
                return recordsFound;
            }

            logger.WriteLine(GetNowDateTime() + $"FindByLastName() returned IRecordIterator which match key = '{lastName}'.");
            DisposeStreams();

            return recordsFound;
        }

        /// <summary>
        /// Finds record by dateOfBirth.
        /// </summary>
        /// <param name="dateOfBirth">Person's dateOfBirth.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            IEnumerable<FileCabinetRecord> recordsFound;

            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling FindByDateOfBirth() with key = '{dateOfBirth}'.");
                recordsFound = this.service.FindByDateOfBirth(dateOfBirth);
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return new List<FileCabinetRecord>();
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return new List<FileCabinetRecord>();
            }

            if (recordsFound is null)
            {
                logger.WriteLine(GetNowDateTime() + $"FindByFirstName() found no match with key = '{dateOfBirth}' and returned null.");
                DisposeStreams();
                return recordsFound;
            }

            logger.WriteLine(GetNowDateTime() + $"FindByDateOfBirth() returned IRecordIterator which match key = '{dateOfBirth}'.");
            DisposeStreams();

            return recordsFound;
        }

        /// <summary>
        /// Finds record by jobExperience.
        /// </summary>
        /// <param name="jobExperience">Person's jobExperience.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        public IEnumerable<FileCabinetRecord> FindByJobExperience(short jobExperience)
        {
            IEnumerable<FileCabinetRecord> recordsFound;

            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling FindByJobExperienceh() with key = '{jobExperience}'.");
                recordsFound = this.service.FindByJobExperience(jobExperience);
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return new List<FileCabinetRecord>();
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return new List<FileCabinetRecord>();
            }

            if (recordsFound is null)
            {
                logger.WriteLine(GetNowDateTime() + $"FindByJobExperience() found no match with key = '{jobExperience}' and returned null.");
                DisposeStreams();
                return recordsFound;
            }

            logger.WriteLine(GetNowDateTime() + $"FindByJobExperience() returned IRecordIterator which match key = '{jobExperience}'.");
            DisposeStreams();

            return recordsFound;
        }

        /// <summary>
        /// Finds record by monthlyPay.
        /// </summary>
        /// <param name="monthlyPay">Person's monthlyPay.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        public IEnumerable<FileCabinetRecord> FindByMonthlyPay(decimal monthlyPay)
        {
            IEnumerable<FileCabinetRecord> recordsFound;

            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling FindByMonthlyPay() with key = '{monthlyPay}'.");
                recordsFound = this.service.FindByMonthlyPay(monthlyPay);
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return new List<FileCabinetRecord>();
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return new List<FileCabinetRecord>();
            }

            if (recordsFound is null)
            {
                logger.WriteLine(GetNowDateTime() + $"FindByMonthlyPay() found no match with key = '{monthlyPay}' and returned null.");
                DisposeStreams();
                return recordsFound;
            }

            logger.WriteLine(GetNowDateTime() + $"FindByMonthlyPay() returned IRecordIterator which match key = '{monthlyPay}'.");
            DisposeStreams();

            return recordsFound;
        }

        /// <summary>
        /// Finds record by gender.
        /// </summary>
        /// <param name="gender">Person's gender.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        public IEnumerable<FileCabinetRecord> FindByGender(char gender)
        {
            IEnumerable<FileCabinetRecord> recordsFound;

            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling FindByMonthlyPay() with key = '{gender}'.");
                recordsFound = this.service.FindByGender(gender);
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return new List<FileCabinetRecord>();
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return new List<FileCabinetRecord>();
            }

            if (recordsFound is null)
            {
                logger.WriteLine(GetNowDateTime() + $"FindByGender() found no match with key = '{gender}' and returned null.");
                DisposeStreams();
                return recordsFound;
            }

            logger.WriteLine(GetNowDateTime() + $"FindByGender() returned IRecordIterator which match key = '{gender}'.");
            DisposeStreams();

            return recordsFound;
        }

        /// <summary>
        /// Makes snapshot of FileCabinetService list of records.
        /// </summary>
        /// <returns>Object of snapshot instance with file records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            FileCabinetServiceSnapshot snapshot;
            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling MakeSnapshot().");
                snapshot = this.service.MakeSnapshot();
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return null;
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return null;
            }

            logger.WriteLine(GetNowDateTime() + $"MakeSnapshot() returned FileCabinetServiceSnapshot instance with now operating records.");
            DisposeStreams();

            return snapshot;
        }

        /// <summary>
        /// Restores FileCabinetMemoryService record list with imported records.
        /// </summary>
        /// <param name="snapshot">FileCabinetMemoryService snapshot.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            CreateStreams();

            if (snapshot is null)
            {
                logger.WriteLine(GetNowDateTime() + $"CallingRestore(): {snapshot} is null.");
                DisposeStreams();
                throw new ArgumentNullException($"{snapshot} is null.");
            }

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling Restore() with {snapshot.Records.Count} records.");
                this.service.Restore(snapshot);
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return;
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return;
            }

            logger.WriteLine(GetNowDateTime() + $"Restore() added {snapshot.Records.Count} records loaded from file.");
            DisposeStreams();
        }

        /// <summary>
        /// Gets new record's id.
        /// </summary>
        /// <returns>LaNewst record's id.</returns>
        public int GetNewId()
        {
            int maxId = 0;

            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling GetNewId().");
                maxId = this.service.GetNewId();
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return maxId;
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return maxId;
            }

            logger.WriteLine(GetNowDateTime() + $"GetNewId() returned id = '{maxId}'.");
            DisposeStreams();

            return maxId;
        }

        /// <summary>
        /// Deletes specified record from FileCabinetService data storage.
        /// </summary>
        /// <param name="id">Number of record.</param>
        public void DeleteRecord(int id)
        {
            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling RemoveRecord() with id = '{id}'.");
                this.service.DeleteRecord(id);
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return;
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return;
            }

            logger.WriteLine(GetNowDateTime() + $"RemoveRecord() removed record #{id}.");
            DisposeStreams();
        }

        /// <summary>
        /// Deletes marked as IsDeleted from *.db file.
        /// </summary>
        public void PurgeFile()
        {
            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling PurgeFile().");
                this.service.PurgeFile();
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return;
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return;
            }

            logger.WriteLine(GetNowDateTime() + $"PurgeFile() purged file.");
            DisposeStreams();
        }

        /// <summary>
        /// Finds out if record exists in file servise list.
        /// </summary>
        /// <param name="id">User input id.</param>
        /// <returns>true - record exists, false - record doesn't exist.</returns>
        public bool RecordExists(int id)
        {
            bool exists = false;

            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling RecordExists() with id = '{id}'.");
                exists = this.service.RecordExists(id);
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return exists;
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return exists;
            }

            logger.WriteLine(GetNowDateTime() + $"RecordExists() returned '{exists}'.");
            DisposeStreams();
            return exists;
        }

        /// <summary>
        /// Clears cache.
        /// </summary>
        public void ClearCache()
        {
            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling ClearCache().");
                this.service.ClearCache();
            }
            catch (ArgumentNullException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return;
            }
            catch (ArgumentException message)
            {
                logger.WriteLine(GetNowDateTime() + message);
                DisposeStreams();
                Console.WriteLine(message);
                return;
            }

            logger.WriteLine(GetNowDateTime() + $"ClearCache() cleared cache.");
            DisposeStreams();
        }

        private static void CreateStreams()
        {
            fileStream = new FileStream("log.csv", FileMode.OpenOrCreate, FileAccess.Write);
            fileStream.Seek(0, SeekOrigin.End);
            logger = new StreamWriter(fileStream);
        }

        private static void DisposeStreams()
        {
            logger.Close();
            fileStream.Close();
        }

        private static string GetNowDateTime()
        {
            return DateTime.Now.ToString(CultureInfo.CreateSpecificCulture("en-GB")) + " - ";
        }
    }
}
