using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides FileCabinetRecord Sort.
    /// </summary>
    public class FileCabinetRecordSort : IComparer<FileCabinetRecord>
    {
        /// <summary>
        /// Sorts records by Id.
        /// </summary>
        /// <param name="x">First comparable record.</param>
        /// <param name="y">Second comaparable record.</param>
        /// <returns>int.</returns>
        public int Compare(FileCabinetRecord x, FileCabinetRecord y)
        {
            if (!(x is null || y is null))
            {
                if (x.Id == y.Id)
                {
                    return 0;
                }
                else
                {
                    if (x.Id > y.Id)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }

            return 2;
        }
    }
}
