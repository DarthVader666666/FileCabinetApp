using System;
using System.Collections;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides class for records found enumeration.
    /// </summary>
    public class RecordsFound : IEnumerable<FileCabinetRecord>
    {
        private readonly IRecordIterator iterator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordsFound"/> class.
        /// </summary>
        /// <param name="iterator">Iterator to fill records found collection.</param>
        public RecordsFound(IRecordIterator iterator)
        {
            this.iterator = iterator;
        }

        /// <summary>
        /// Implements IEnumerable GetEnumerator() method.
        /// </summary>
        /// <returns>IEnumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets Found Recors enumerator.
        /// </summary>
        /// <returns>RecordsFoundEnum.</returns>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            while (this.iterator.HasMore())
            {
                yield return this.iterator.GetNext();
            }
        }
    }
}
