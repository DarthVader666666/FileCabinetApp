using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides MemoryIterator.
    /// </summary>
    public class MemoryIterator : IRecordIterator
    {
        private readonly List<FileCabinetRecord> list;
        private int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="list">List of key specified records.</param>
        public MemoryIterator(List<FileCabinetRecord> list)
        {
            this.list = list;
        }

        /// <summary>
        /// Gets record of specific position.
        /// </summary>
        /// <returns>FileCabinetRecord.</returns>
        public FileCabinetRecord GetNext()
        {
            if (this.list.Count == 0)
            {
                return null;
            }

            return this.list[this.index++];
        }

        /// <summary>
        /// Finds out if there are more positions in list.
        /// </summary>
        /// <returns>bool.</returns>
        public bool HasMore()
        {
            if (this.index < this.list.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
