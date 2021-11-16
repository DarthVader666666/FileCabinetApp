using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for Create and Edit record parameters.
    /// </summary>
    public class FileCabinetEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetEventArgs"/> class.
        /// </summary>
        /// <param name="record">FileCabinetRecord object.</param>
        public FileCabinetEventArgs(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record is null");
            }

            this.Id = record.Id;
            this.FirstName = record.FirstName;
            this.LastName = record.LastName;
            this.DateOfBirth = record.DateOfBirth;
            this.JobExperience = record.JobExperience;
            this.MonthlyPay = record.MonthlyPay;
            this.Gender = record.Gender;
        }

        /// <summary>
        /// Gets record's number.
        /// </summary>
        /// <value>Number of record in file cabinet.</value>
        public int Id { get; }

        /// <summary>
        /// Gets person's first name.
        /// </summary>
        /// <value>Person's first name.</value>
        public string FirstName { get; }

        /// <summary>
        /// Gets person's last name.
        /// </summary>
        /// <value>Person's last name.</value>
        public string LastName { get; }

        /// <summary>
        /// Gets person's day if birth.
        /// </summary>
        /// <value>Person's day of birth.</value>
        public DateTime DateOfBirth { get; }

        /// <summary>
        /// Gets quantity of years a person works on the current job.
        /// </summary>
        /// <value>The quantity of years person works.</value>
        public short JobExperience { get; }

        /// <summary>
        /// Gets hom much a person earns per month.
        /// </summary>
        /// <value>The size of person's monthly salary.</value>
        public decimal MonthlyPay { get; }

        /// <summary>
        /// Gets first letter of peson's gender.
        /// </summary>
        /// <value>Person's gender.</value>
        public char Gender { get; }
    }
}