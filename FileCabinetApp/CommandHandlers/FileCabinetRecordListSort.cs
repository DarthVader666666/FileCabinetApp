using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides FIleCabinetRecord List Sort.
    /// </summary>
    public class FileCabinetRecordListSort : IComparer<List<FileCabinetRecord>>
    {
        /// <summary>
        /// Compares FileCabinetRecord lists.
        /// </summary>
        /// <param name="x">First FileCabinetRecord list.</param>
        /// <param name="y">Second FileCabinetRecord list.</param>
        /// <returns>int.</returns>
        public int Compare(List<FileCabinetRecord> x, List<FileCabinetRecord> y)
        {
            if (!(x is null || y is null))
            {
                if (x.Count == y.Count)
                {
                    return 0;
                }
                else
                {
                    if (x.Count > y.Count)
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
