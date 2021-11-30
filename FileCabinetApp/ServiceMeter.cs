using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FileCabinetApp
{
    /// <summary>
    /// Decorator class for FileCabinetService.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">FileCabinetService instance.</param>
        public ServiceMeter(IFileCabinetService service)
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
            this.stopwatch.Start();
            this.service.CreateRecord(sender, e);
            this.stopwatch.Stop();
            Console.WriteLine($"CreateRecord method execution duration is {this.stopwatch.ElapsedTicks} ticks");
            this.stopwatch.Reset();
        }

        /// <summary>
        /// Wraps local FileCabinetRecord List into ReadonlyCollection.
        /// </summary>
        /// <returns>ReadonlyCollection of file records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.stopwatch.Start();
            var records = this.service.GetRecords();
            this.stopwatch.Stop();
            Console.WriteLine($"GetRecords method execution duration is {this.stopwatch.ElapsedTicks} ticks");
            this.stopwatch.Reset();

            return records;
        }

        /// <summary>
        /// Gets total count of file records.
        /// </summary>
        /// <returns>Number of file records.</returns>
        public Tuple<int, int> GetStat()
        {
            this.stopwatch.Start();
            var statInfo = this.service.GetStat();
            this.stopwatch.Stop();
            Console.WriteLine($"GetStat method execution duration is {this.stopwatch.ElapsedTicks} ticks");
            this.stopwatch.Reset();
            return statInfo;
        }

        /// <summary>
        /// Edits file record.
        /// </summary>
        /// <param name="sender">Object that sends the arguments is null.</param>
        /// <param name="recordArgs">File record arguments.</param>
        public void EditRecord(object sender, FileCabinetEventArgs recordArgs)
        {
            this.stopwatch.Start();
            this.service.EditRecord(sender, recordArgs);
            this.stopwatch.Stop();
            Console.WriteLine($"EditRecord method execution duration is {this.stopwatch.ElapsedTicks} ticks");
            this.stopwatch.Reset();
        }

        /// <summary>
        /// Finds record by First Name.
        /// </summary>
        /// <param name="firstName">Person's first name.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.stopwatch.Start();
            var result = this.service.FindByFirstName(firstName);
            this.stopwatch.Stop();
            Console.WriteLine($"FindByFirstName method execution duration is {this.stopwatch.ElapsedTicks} ticks");
            this.stopwatch.Reset();

            return result;
        }

        /// <summary>
        /// Finds record by Last Name.
        /// </summary>
        /// <param name="lastName">Person's last name.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.stopwatch.Start();
            var result = this.service.FindByLastName(lastName);
            this.stopwatch.Stop();
            Console.WriteLine($"FindByLastName method execution duration is {this.stopwatch.ElapsedTicks} ticks");
            this.stopwatch.Reset();

            return result;
        }

        /// <summary>
        /// Finds record by dateOfBirth.
        /// </summary>
        /// <param name="dateOfBirth">Person's dateOfBirth.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            this.stopwatch.Start();
            var result = this.service.FindByDateOfBirth(dateOfBirth);
            this.stopwatch.Stop();
            Console.WriteLine($"FindDateOfBirth method execution duration is {this.stopwatch.ElapsedTicks} ticks");
            this.stopwatch.Reset();

            return result;
        }

        /// <summary>
        /// Makes snapshot of FileCabinetService list of records.
        /// </summary>
        /// <returns>Object of snapshot instance with file records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.stopwatch.Start();
            var result = this.service.MakeSnapshot();
            this.stopwatch.Stop();
            Console.WriteLine($"MakeSnapshot method execution duration is {this.stopwatch.ElapsedTicks} ticks");
            this.stopwatch.Reset();

            return result;
        }

        /// <summary>
        /// Restores FileCabinetMemoryService record list with imported records.
        /// </summary>
        /// <param name="snapshot">FileCabinetMemoryService snapshot.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.stopwatch.Start();
            this.service.Restore(snapshot);
            this.stopwatch.Stop();
            Console.WriteLine($"Restore method execution duration is {this.stopwatch.ElapsedTicks} ticks");
            this.stopwatch.Reset();
        }

        /// <summary>
        /// Gets last record's id.
        /// </summary>
        /// <returns>Last record's id.</returns>
        public int GetMaxId()
        {
            this.stopwatch.Start();
            var result = this.service.GetMaxId();
            this.stopwatch.Stop();
            Console.WriteLine($"GetMaxId method execution duration is {this.stopwatch.ElapsedTicks} ticks");
            this.stopwatch.Reset();

            return result;
        }

        /// <summary>
        /// Removes specified record from FileCabinetService data storage.
        /// </summary>
        /// <param name="id">Number of record.</param>
        public void RemoveRecord(int id)
        {
            this.stopwatch.Start();
            this.service.RemoveRecord(id);
            this.stopwatch.Stop();
            Console.WriteLine($"RemoveRecord method execution duration is {this.stopwatch.ElapsedTicks} ticks");
            this.stopwatch.Reset();
        }

        /// <summary>
        /// Deletes marked as IsDeleted from *.db file.
        /// </summary>
        public void PurgeFile()
        {
            this.stopwatch.Start();
            this.service.PurgeFile();
            this.stopwatch.Stop();
            Console.WriteLine($"PurgeFile method execution duration is {this.stopwatch.ElapsedTicks} ticks");
            this.stopwatch.Reset();
        }

        /// <summary>
        /// Finds out if record exists in file servise list.
        /// </summary>
        /// <param name="id">User input id.</param>
        /// <returns>true - record exists, false - record doesn't exist.</returns>
        public bool RecordExists(int id)
        {
            bool exists;

            this.stopwatch.Start();
            exists = this.service.RecordExists(id);
            this.stopwatch.Stop();
            Console.WriteLine($"RecordExists method execution duration is {this.stopwatch.ElapsedTicks} ticks");
            this.stopwatch.Reset();

            return exists;
        }
    }
}
