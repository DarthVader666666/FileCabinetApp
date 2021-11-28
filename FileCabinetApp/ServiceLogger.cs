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
        /// Edits file record.
        /// </summary>
        /// <param name="sender">Object that sends the arguments is null.</param>
        /// <param name="recordArgs">File record arguments.</param>
        public void EditRecord(object sender, FileCabinetEventArgs recordArgs)
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
                this.service.EditRecord(sender, recordArgs);
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
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            ReadOnlyCollection<FileCabinetRecord> record;
            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling FindByFirstName() with key = '{firstName}'.");
                record = this.service.FindByFirstName(firstName);
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

            logger.WriteLine(GetNowDateTime() + $"FindByFirstName() returned {record.Count} records which match key = '{firstName}'.");
            DisposeStreams();

            return record;
        }

        /// <summary>
        /// Finds record by Last Name.
        /// </summary>
        /// <param name="lastName">Person's last name.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            ReadOnlyCollection<FileCabinetRecord> record;
            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling FindByLastName() with key = '{lastName}'.");
                record = this.service.FindByFirstName(lastName);
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

            logger.WriteLine(GetNowDateTime() + $"FindByLastName() returned {record.Count} records which match key = '{lastName}'.");
            DisposeStreams();

            return record;
        }

        /// <summary>
        /// Finds record by dateOfBirth.
        /// </summary>
        /// <param name="dateOfBirth">Person's dateOfBirth.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            ReadOnlyCollection<FileCabinetRecord> record;
            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling FindByDateOfBirth() with key = '{dateOfBirth}'.");
                record = this.service.FindByFirstName(dateOfBirth);
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

            logger.WriteLine(GetNowDateTime() + $"FindByDateOfBirth() returned {record.Count} records which match key = '{dateOfBirth}'.");
            DisposeStreams();

            return record;
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
        /// Gets last record's id.
        /// </summary>
        /// <returns>Last record's id.</returns>
        public int GetMaxId()
        {
            int maxId = 0;

            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling GetMaxId().");
                maxId = this.service.GetMaxId();
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

            logger.WriteLine(GetNowDateTime() + $"GetMaxId() returned id = '{maxId}'.");
            DisposeStreams();

            return maxId;
        }

        /// <summary>
        /// Removes specified record from FileCabinetService data storage.
        /// </summary>
        /// <param name="id">Number of record.</param>
        public void RemoveRecord(int id)
        {
            CreateStreams();

            try
            {
                logger.WriteLine(GetNowDateTime() + $"Calling RemoveRecord() with id = '{id}'.");
                this.service.RemoveRecord(id);
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
