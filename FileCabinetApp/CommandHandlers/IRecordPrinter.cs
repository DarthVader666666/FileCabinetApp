using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

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
        /// <param name="records">Records to be printed.</param>
        /// <param name="properties">Properties to be printed.</param>
        void Print(ReadOnlyCollection<FileCabinetRecord> records, List<PropertyInfo> properties);
    }
}
