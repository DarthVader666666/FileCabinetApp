using System;
using System.Collections.Generic;

public class FileCabinetService
{
    private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

    public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth)
    {
        // TODO: добавьте реализацию метода
        var record = new FileCabinetRecord
        {
            Id = this.list.Count + 1,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
        };

        this.list.Add(record);

        return record.Id;
    }

    public FileCabinetRecord[] GetRecords()
    {
        // TODO: добавьте реализацию метода
        return Array.Empty<FileCabinetRecord>();
    }

    public int GetStat()
    {
        // TODO: добавьте реализацию метода
        return this.list.Count;
    }
}