using System;
using System.Collections.Generic;
using System.Globalization;

public class FileCabinetService
{
    private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
    private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
    private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
    private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();

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

        if (!this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
        {
            this.firstNameDictionary.Add(firstName.ToUpperInvariant(), new List<FileCabinetRecord>());
        }

        this.firstNameDictionary[firstName.ToUpperInvariant()].Add(record);

        if (!this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
        {
            this.lastNameDictionary.Add(lastName.ToUpperInvariant(), new List<FileCabinetRecord>());
        }

        this.lastNameDictionary[lastName.ToUpperInvariant()].Add(record);

        if (!this.dateOfBirthDictionary.ContainsKey($"{dateOfBirth.Year}-{dateOfBirth.Month}-{dateOfBirth.Day}"))
        {
            this.dateOfBirthDictionary.Add($"{dateOfBirth.Year}-{dateOfBirth.Month}-{dateOfBirth.Day}", new List<FileCabinetRecord>());
        }

        this.dateOfBirthDictionary[$"{dateOfBirth.Year}-{dateOfBirth.Month}-{dateOfBirth.Day}"].Add(record);

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

        if ((firstName is null) || (lastName is null))
        {
            throw new ArgumentNullException($"{firstName} or {lastName} is null");
        }

        FileCabinetRecord recToRemove = this.firstNameDictionary[this.list[id - 1].FirstName.ToUpperInvariant()].Find(i => i.Id == id);

        if (!(recToRemove is null))
        {
            this.firstNameDictionary[this.list[id - 1].FirstName.ToUpperInvariant()].Remove(recToRemove);
        }

        recToRemove = this.lastNameDictionary[this.list[id - 1].LastName.ToUpperInvariant()].Find(i => i.Id == id);

        if (!(recToRemove is null))
        {
            this.lastNameDictionary[this.list[id - 1].LastName.ToUpperInvariant()].Remove(recToRemove);
        }

        recToRemove = this.dateOfBirthDictionary[this.list[id - 1].DateOfBirth.ToShortDateString()].Find(i => i.Id == id);

        if (!(recToRemove is null))
        {
            this.dateOfBirthDictionary[this.list[id - 1].DateOfBirth.ToShortDateString()].Remove(recToRemove);
        }

        this.list[id - 1].Id = id;
        this.list[id - 1].FirstName = firstName;
        this.list[id - 1].LastName = lastName;
        this.list[id - 1].DateOfBirth = dateOfBirth;
        this.list[id - 1].JobExperience = jobExperience;
        this.list[id - 1].MonthlyPay = monthlyPay;
        this.list[id - 1].Gender = char.ToUpper(gender, System.Globalization.CultureInfo.InvariantCulture);

        if (!this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
        {
            this.firstNameDictionary.Add(firstName.ToUpperInvariant(), new List<FileCabinetRecord>());
        }

        this.firstNameDictionary[firstName.ToUpperInvariant()].Add(this.list[id - 1]);

        if (!this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
        {
            this.lastNameDictionary.Add(lastName.ToUpperInvariant(), new List<FileCabinetRecord>());
        }

        this.lastNameDictionary[lastName.ToUpperInvariant()].Add(this.list[id - 1]);

        if (!this.dateOfBirthDictionary.ContainsKey($"{dateOfBirth.Year}-{dateOfBirth.Month}-{dateOfBirth.Day}"))
        {
            this.dateOfBirthDictionary.Add($"{dateOfBirth.Year}-{dateOfBirth.Month}-{dateOfBirth.Day}", new List<FileCabinetRecord>());
        }

        this.dateOfBirthDictionary[$"{dateOfBirth.Year}-{dateOfBirth.Month}-{dateOfBirth.Day}"].Add(this.list[id - 1]);
    }

    public FileCabinetRecord[] FindByFirstName(string firstName)
    {
        if (!(firstName is null) && this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
        {
            return this.firstNameDictionary[firstName.ToUpperInvariant()].ToArray();
        }

        return Array.Empty<FileCabinetRecord>();
    }

    public FileCabinetRecord[] FindByLastName(string lastName)
    {
        if (!(lastName is null) && this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
        {
            return this.lastNameDictionary[lastName.ToUpperInvariant()].ToArray();
        }

        return Array.Empty<FileCabinetRecord>();
    }

    public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
    {
        if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
        {
            return this.dateOfBirthDictionary[dateOfBirth].ToArray();
        }

        return Array.Empty<FileCabinetRecord>();
    }
}