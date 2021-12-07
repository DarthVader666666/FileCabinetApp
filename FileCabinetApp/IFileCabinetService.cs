using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface for File service functionality extraction.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Creates new file record.
        /// </summary>
        /// <param name="sender">Object that sends the arguments is null.</param>
        /// <param name="e">File record arguments.</param>
        void CreateRecord(object sender, FileCabinetEventArgs e);

        /// <summary>
        /// Wraps local FileCabinetRecord List into ReadonlyCollection.
        /// </summary>
        /// <returns>ReadonlyCollection of file records.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Gets total count of file records.
        /// </summary>
        /// <returns>Number of file records.</returns>
        Tuple<int, int> GetStat();

        /// <summary>
        /// Updates file record.
        /// </summary>
        /// <param name="sender">Object that sends the arguments is null.</param>
        /// <param name="recordArgs">File record arguments.</param>
        void UpdateRecord(object sender, FileCabinetEventArgs recordArgs);

        /// <summary>
        /// Finds record by First Name.
        /// </summary>
        /// <param name="firstName">Person's first name.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        IEnumerable<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Finds record by Last Name.
        /// </summary>
        /// <param name="lastName">Person's last name.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        IEnumerable<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Finds record by dateOfBirth.
        /// </summary>
        /// <param name="dateOfBirth">Person's dateOfBirth.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth);

        /// <summary>
        /// Finds record by jobExperience.
        /// </summary>
        /// <param name="jobExperience">Person's jobExperience.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        IEnumerable<FileCabinetRecord> FindByJobExperience(short jobExperience);

        /// <summary>
        /// Finds record by monthlyPay.
        /// </summary>
        /// <param name="monthlyPay">Person's monthlyPay.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        IEnumerable<FileCabinetRecord> FindByMonthlyPay(decimal monthlyPay);

        /// <summary>
        /// Finds record by gender.
        /// </summary>
        /// <param name="gender">Person's gender.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        IEnumerable<FileCabinetRecord> FindByGender(char gender);

        /// <summary>
        /// Makes snapshot of FileCabinetService list of records.
        /// </summary>
        /// <returns>Object of snapshot instance with file records.</returns>
        FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Restores FileCabinetMemoryService record list with imported records.
        /// </summary>
        /// <param name="snapshot">FileCabinetMemoryService snapshot.</param>
        void Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Gets last record's id.
        /// </summary>
        /// <returns>Last record's id.</returns>
        int GetNewId();

        /// <summary>
        /// Removes specified record from FileCabinetService data storage.
        /// </summary>
        /// <param name="id">Number of record.</param>
        void DeleteRecord(int id);

        /// <summary>
        /// Deletes marked as IsDeleted from *.db file.
        /// </summary>
        void PurgeFile();

        /// <summary>
        /// Finds out if record exists in file servise list.
        /// </summary>
        /// <param name="id">User input id.</param>
        /// <returns>true - record exists, false - record doesn't exist.</returns>
        bool RecordExists(int id);

        /// <summary>
        /// Clears cache.
        /// </summary>
        void ClearCache();
    }
}
