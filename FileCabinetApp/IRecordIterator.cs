using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides File Records iterator.
    /// </summary>
    public interface IRecordIterator
    {
        /// <summary>
        /// Gets next file record.
        /// </summary>
        /// <returns>FileCabinetRecord</returns>
        FileCabinetRecord GetNext();

        /// <summary>
        /// Finds out if there are more positions in list.
        /// </summary>
        /// <returns>bool.</returns>
        bool HasMore();
    }
}
