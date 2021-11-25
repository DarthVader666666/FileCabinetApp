using System;
using System.Collections.ObjectModel;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides extention for Print records method.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Defines Print records method.
        /// </summary>
        /// <param name="recordList">Records to be printed.</param>
        void Print(ReadOnlyCollection<FileCabinetRecord> records);
    }
}
