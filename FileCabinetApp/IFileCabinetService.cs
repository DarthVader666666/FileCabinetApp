using System;
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
        int GetStat();

        /// <summary>
        /// Edits file record.
        /// </summary>
        /// <param name="sender">Object that sends the arguments is null.</param>
        /// <param name="recordArgs">File record arguments.</param>
        void EditRecord(object sender, FileCabinetEventArgs recordArgs);

        /// <summary>
        /// Finds record by First Name.
        /// </summary>
        /// <param name="firstName">Person's first name.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Finds record by Last Name.
        /// </summary>
        /// <param name="lastName">Person's last name.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Finds record by dateOfBirth.
        /// </summary>
        /// <param name="dateOfBirth">Person's dateOfBirth.</param>
        /// <returns>ReadonlyCollection of file records found.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth);
    }
}
