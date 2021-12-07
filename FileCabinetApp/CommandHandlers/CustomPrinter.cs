using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides custom printer method.
    /// </summary>
    public class CustomPrinter : IRecordPrinter
    {
        /// <summary>
        /// Prints records using specified criteria.
        /// </summary>
        /// <param name="records">Records to be printed.</param>
        /// <param name="properties">Record's properties which fields to be printed.</param>
        public void Print(ReadOnlyCollection<FileCabinetRecord> records, List<PropertyInfo> properties)
        {
            if (records is null)
            {
                throw new ArgumentNullException($"{records} is null.");
            }

            if (properties is null)
            {
                throw new ArgumentNullException($"{properties} is null.");
            }

            int idmaxSize = "Id".Length;
            int firstNameMaxSize = "FirstName".Length;
            int lastNameMaxSize = "LastName".Length;
            int dateOfBirthMaxSize = "DateOfBirth".Length;
            int jobExperienceMaxSize = "JobExperience".Length;
            int monthlyPayMaxSize = "MonthlyPay".Length;
            int genderMaxSize = "Gender".Length;
            string line = "+";

            foreach (var property in properties)
            {
                IEnumerable<int> sizes;

                switch (property.Name.ToUpperInvariant())
                {
                    case "ID":
                        {
                            sizes = from rec in records where rec.Id.ToString(CultureInfo.InvariantCulture).Length > idmaxSize select rec.Id.ToString(CultureInfo.InvariantCulture).Length;
                            if (!sizes.Any())
                            {
                                line = string.Concat(line, GetLines(idmaxSize)) + "-+";
                                break;
                            }
                            else
                            {
                                sizes.ToList().Sort();
                                idmaxSize = sizes.ToList()[^1];
                            }

                            line = string.Concat(line, GetLines(idmaxSize)) + "-+";
                            break;
                        }

                    case "FIRSTNAME":
                        {
                            sizes = from rec in records where rec.FirstName.Length > firstNameMaxSize select rec.FirstName.Length;
                            if (!sizes.Any())
                            {
                                line = string.Concat(line, GetLines(firstNameMaxSize)) + "-+";
                                break;
                            }
                            else
                            {
                                sizes.ToList().Sort();
                                firstNameMaxSize = sizes.ToList()[^1];
                            }

                            line = string.Concat(line, GetLines(firstNameMaxSize)) + "-+";
                            break;
                        }

                    case "LASTNAME":
                        {
                            sizes = from rec in records where rec.LastName.Length > lastNameMaxSize select rec.LastName.Length;
                            if (!sizes.Any())
                            {
                                line = string.Concat(line, GetLines(lastNameMaxSize)) + "-+";
                                break;
                            }
                            else
                            {
                                sizes.ToList().Sort();
                                lastNameMaxSize = sizes.ToList()[^1];
                            }

                            line = string.Concat(line, GetLines(lastNameMaxSize)) + "-+";
                            break;
                        }

                    case "DATEOFBIRTH":
                        {
                            sizes = from rec in records where rec.DateOfBirth.ToShortDateString().Length > dateOfBirthMaxSize select rec.DateOfBirth.ToShortDateString().Length;
                            if (!sizes.Any())
                            {
                                line = string.Concat(line, GetLines(dateOfBirthMaxSize)) + "-+";
                                break;
                            }
                            else
                            {
                                sizes.ToList().Sort();
                                dateOfBirthMaxSize = sizes.ToList()[^1];
                            }

                            line = string.Concat(line, GetLines(dateOfBirthMaxSize)) + "-+";
                            break;
                        }

                    case "JOBEXPERIENCE":
                        {
                            sizes = from rec in records where rec.JobExperience.ToString(CultureInfo.InvariantCulture).Length > jobExperienceMaxSize select rec.JobExperience.ToString(CultureInfo.InvariantCulture).Length;
                            if (!sizes.Any())
                            {
                                line = string.Concat(line, GetLines(jobExperienceMaxSize)) + "-+";
                                break;
                            }
                            else
                            {
                                sizes.ToList().Sort();
                                jobExperienceMaxSize = sizes.ToList()[^1];
                            }

                            line = string.Concat(line, GetLines(jobExperienceMaxSize)) + "-+";
                            break;
                        }

                    case "MONTHLYPAY":
                        {
                            sizes = from rec in records where rec.MonthlyPay.ToString(CultureInfo.InvariantCulture).Length > monthlyPayMaxSize select string.Format(CultureInfo.InvariantCulture, $"{rec.MonthlyPay}:F2").Length;
                            if (!sizes.Any())
                            {
                                line = string.Concat(line, GetLines(monthlyPayMaxSize)) + "-+";
                                break;
                            }
                            else
                            {
                                sizes.ToList().Sort();
                                monthlyPayMaxSize = sizes.ToList()[^1];
                            }

                            line = string.Concat(line, GetLines(monthlyPayMaxSize)) + "-+";
                            break;
                        }

                    case "GENDER":
                        {
                            line = string.Concat(line, GetLines(genderMaxSize)) + "-+";
                            break;
                        }

                    default: throw new ArgumentException("None of the parameters match the conditions.");
                }
            }

            Console.WriteLine(line);
            Console.Write("|");

            foreach (var property in properties)
            {
                switch (property.Name.ToUpperInvariant())
                {
                    case "ID":
                        {
                            AddWhiteSpaces(idmaxSize, property.Name.Length);
                            Console.Write($" {property.Name} |");
                            break;
                        }

                    case "FIRSTNAME":
                        {
                            Console.Write($" {property.Name}");
                            AddWhiteSpaces(firstNameMaxSize, property.Name.Length);
                            Console.Write(" |");
                            break;
                        }

                    case "LASTNAME":
                        {
                            Console.Write($" {property.Name}");
                            AddWhiteSpaces(lastNameMaxSize, property.Name.Length);
                            Console.Write(" |");
                            break;
                        }

                    case "DATEOFBIRTH":
                        {
                            AddWhiteSpaces(dateOfBirthMaxSize, property.Name.Length);
                            Console.Write($" {property.Name} |");
                            break;
                        }

                    case "JOBEXPERIENCE":
                        {
                            AddWhiteSpaces(jobExperienceMaxSize, property.Name.Length);
                            Console.Write($" {property.Name} |");
                            break;
                        }

                    case "MONTHLYPAY":
                        {
                            AddWhiteSpaces(monthlyPayMaxSize, property.Name.Length);
                            Console.Write($" {property.Name} |");
                            break;
                        }

                    case "GENDER":
                        {
                            Console.Write($" {property.Name}");
                            AddWhiteSpaces(genderMaxSize, property.Name.Length);
                            Console.Write(" |");
                            break;
                        }

                    default: throw new ArgumentException("None of the parameters match the conditions.");
                }
            }

            Console.WriteLine();
            Console.WriteLine(line);

            foreach (var record in records)
            {
                Console.Write("|");

                foreach (var property in properties)
                {
                    switch (property.Name.ToUpperInvariant())
                    {
                        case "ID":
                            {
                                AddWhiteSpaces(idmaxSize, record.Id.ToString(CultureInfo.InvariantCulture).Length);
                                Console.Write($" {record.Id} |");
                                break;
                            }

                        case "FIRSTNAME":
                            {
                                Console.Write($" {record.FirstName}");
                                AddWhiteSpaces(firstNameMaxSize, record.FirstName.Length);
                                Console.Write(" |");
                                break;
                            }

                        case "LASTNAME":
                            {
                                Console.Write($" {record.LastName}");
                                AddWhiteSpaces(lastNameMaxSize, record.LastName.Length);
                                Console.Write(" |");
                                break;
                            }

                        case "DATEOFBIRTH":
                            {
                                AddWhiteSpaces(dateOfBirthMaxSize, record.DateOfBirth.ToShortDateString().Length);
                                Console.Write($" {record.DateOfBirth.Year}/{record.DateOfBirth.Month:D2}/{record.DateOfBirth.Day:D2} |");
                                break;
                            }

                        case "JOBEXPERIENCE":
                            {
                                AddWhiteSpaces(jobExperienceMaxSize, record.JobExperience.ToString(CultureInfo.InvariantCulture).Length);
                                Console.Write($" {record.JobExperience} |");
                                break;
                            }

                        case "MONTHLYPAY":
                            {
                                AddWhiteSpaces(monthlyPayMaxSize, $"{record.MonthlyPay:F2}".Length);
                                Console.Write($" {record.MonthlyPay:F2} |");
                                break;
                            }

                        case "GENDER":
                            {
                                Console.Write($" {record.Gender}");
                                AddWhiteSpaces(genderMaxSize, record.Gender.ToString(CultureInfo.InvariantCulture).Length);
                                Console.Write(" |");
                                break;
                            }

                        default: throw new ArgumentException("None of the parameters match the conditions.");
                    }
                }

                Console.WriteLine();
                Console.WriteLine(line);
            }
        }

        private static void AddWhiteSpaces(int maxSize, int stringLength)
        {
            for (int i = stringLength; i < maxSize; i++)
            {
                Console.Write(" ");
            }
        }

        private static string GetLines(int size)
        {
            string line = string.Empty;

            for (int i = 0; i < size; i++)
            {
                line = string.Concat(line, "-");
            }

            return line + "-";
        }
    }
}
