using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides FileCabinetRecord comparison.
    /// </summary>
    public class FileCabinetRecordComparer : IEqualityComparer<FileCabinetRecord>
    {
        /// <summary>
        /// Finds out if records Ids are equal.
        /// </summary>
        /// <param name="x">First record to check for equality.</param>
        /// <param name="y">Second record to check for equality.</param>
        /// <returns>bool.</returns>
        public bool Equals(FileCabinetRecord x, FileCabinetRecord y)
        {
            if (x is null || y is null)
            {
                return false;
            }

            if (x.Id == y.Id)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets Hash Code of the record.
        /// </summary>
        /// <param name="obj">FileCabinetRecord.</param>
        /// <returns>Record hash code.</returns>
        public int GetHashCode([DisallowNull] FileCabinetRecord obj)
        {
            return 0;
        }
    }
}
