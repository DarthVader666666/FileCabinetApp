using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for defining a record about person.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets record's number.
        /// </summary>
        /// <value>Number of record in file cabinet.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets person's first name.
        /// </summary>
        /// <value>Person's first name.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets person's last name.
        /// </summary>
        /// <value>Person's last name.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets person's day if birth.
        /// </summary>
        /// <value>Person's day of birth.</value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets quantity of years a person works on the current job.
        /// </summary>
        /// <value>The quantity of years person works.</value>
        public short JobExperience { get; set; }

        /// <summary>
        /// Gets or sets hom much a person earns per month.
        /// </summary>
        /// <value>The size of person's monthly salary.</value>
        public decimal MonthlyPay { get; set; }

        /// <summary>
        /// Gets or sets first letter of peson's gender.
        /// </summary>
        /// <value>Person's gender.</value>
        public char Gender { get; set; }

        /// <summary>
        /// Overrides an Object ToString Converter.
        /// </summary>
        /// <returns>line of properties in string representation.</returns>
        public override string ToString()
        {
            return $"{this.Id},{this.FirstName},{this.LastName},{this.DateOfBirth.Day:D2}/{this.DateOfBirth.Month:D2}/{this.DateOfBirth.Year}," +
                   $"{this.JobExperience},{this.MonthlyPay},{this.Gender}";
        }
    }
}
