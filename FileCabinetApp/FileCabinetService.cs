using System;
using System.Collections.Generic;

public class FileCabinetService
{
    private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

    public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short jobExperience, decimal monthlyPay, char gender)
    {
        // TODO: добавьте реализацию метода
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName), "argument is null");
            }
            else
            {
                throw new ArgumentException("firstName is invalid");
            }
        }

        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
        {
            if (lastName is null)
            {
                throw new ArgumentNullException(nameof(lastName), "argument is null");
            }
            else
            {
                throw new ArgumentException("lastName is invalid");
            }
        }

        if (dateOfBirth.CompareTo(new DateTime(1950, 1, 1)) < 0 || dateOfBirth.CompareTo(DateTime.Today) > 0)
        {
            throw new ArgumentException("dateOfBirth is invalid");
        }

        if (jobExperience < 0)
        {
            throw new ArgumentException("jobExperience is invalid");
        }

        if (monthlyPay < 0)
        {
            throw new ArgumentException("mothlyPay is invalid");
        }

        if (!(gender == 'M' || gender == 'm' || gender == 'F' || gender == 'f'))
        {
            throw new ArgumentException("Invalid gender");
        }

        var record = new FileCabinetRecord
        {
            Id = this.list.Count + 1,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            JobExperience = jobExperience,
            MonthlyPay = monthlyPay,
            Gender = char.ToUpper(gender, System.Globalization.CultureInfo.InvariantCulture),
        };

        this.list.Add(record);

        return record.Id;
    }

    public FileCabinetRecord[] GetRecords()
    {
        // TODO: добавьте реализацию метода
        return this.list.ToArray();
    }

    public int GetStat()
    {
        // TODO: добавьте реализацию метода
        return this.list.Count;
    }

    public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short jobExperience, decimal monthlyPay, char gender)
    {
        if (id > this.GetStat() && id < this.GetStat())
        {
            throw new ArgumentException("No such record");
        }

        this.list[id - 1].Id = id;
        this.list[id - 1].FirstName = firstName;
        this.list[id - 1].LastName = lastName;
        this.list[id - 1].DateOfBirth = dateOfBirth;
        this.list[id - 1].JobExperience = jobExperience;
        this.list[id - 1].MonthlyPay = monthlyPay;
        this.list[id - 1].Gender = char.ToUpper(gender, System.Globalization.CultureInfo.InvariantCulture);
    }
 }