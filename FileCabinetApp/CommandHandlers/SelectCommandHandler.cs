using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles 'select' command.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Record printer to be used in SelectCommandHandler.
        /// </summary>
        private readonly IRecordPrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service to be used in handler.</param>
        /// <param name="printer">Printer to be used in printer.</param>
        public SelectCommandHandler(IFileCabinetService service, IRecordPrinter printer)
            : base(service)
        {
            this.printer = printer;
        }

        /// <summary>
        /// Calls list method or next handler.
        /// </summary>
        /// <param name="request">Provides command and parameters.</param>
        public override void Handle(AddCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException($"{request} is null");
            }

            if (request.Command.Equals("select", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Select(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private static List<Tuple<PropertyInfo, string>> GetCriteriaList(List<PropertyInfo> properties, string[] fields, string[] values)
        {
            List<Tuple<PropertyInfo, string>> criteria = new List<Tuple<PropertyInfo, string>>();
            bool flag;

            foreach (string field in fields)
            {
                flag = false;

                foreach (PropertyInfo info in properties)
                {
                    if (field.Equals(info.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        criteria.Add(new Tuple<PropertyInfo, string>(info, values[Array.IndexOf(fields, field)]));
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    Console.WriteLine($"'{field}' is not a field of FileCabinetRecord.");
                    return new List<Tuple<PropertyInfo, string>>();
                }
            }

            return criteria;
        }

        private static void GetStrings(string arguments, out string[] fields, out string[] values)
        {
            string[] setWhere = arguments.Split(new string[] { "and", "=", "\'", "," }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            fields = Array.FindAll(setWhere, i => Array.IndexOf(setWhere, i) % 2 == 0);
            values = Array.FindAll(setWhere, i => Array.IndexOf(setWhere, i) % 2 != 0);
        }

        private static string CheckFields(string[] fields, List<PropertyInfo> properties)
        {
            bool flag;

            foreach (string field in fields)
            {
                flag = false;

                foreach (var property in properties)
                {
                    if (field.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    return field;
                }
            }

            return null;
        }

        private List<FileCabinetRecord> GetAndMatchedRecords(List<Tuple<PropertyInfo, string>> whereCriteria)
        {
            List<List<FileCabinetRecord>> listOfMatchedRecords = new List<List<FileCabinetRecord>>();
            List<FileCabinetRecord> matchedRecords;

            foreach (var couple in whereCriteria)
            {
                matchedRecords = this.SelectRecords(couple.Item1, couple.Item2).ToList();

                if (!matchedRecords.Any())
                {
                    return new List<FileCabinetRecord>();
                }

                listOfMatchedRecords.Add(matchedRecords);
            }

            listOfMatchedRecords.Sort(new FileCabinetRecordListSort());
            matchedRecords = listOfMatchedRecords[0];

            foreach (var list in listOfMatchedRecords)
            {
                if (list.Count < matchedRecords.Count)
                {
                    matchedRecords = list;
                }
            }

            foreach (var list in listOfMatchedRecords)
            {
                FileCabinetRecord[] extraRecords = Array.FindAll(matchedRecords.ToArray(), i => !list.Contains(i, new FileCabinetRecordComparer()));

                foreach (FileCabinetRecord record in extraRecords)
                {
                    matchedRecords.Remove(record);
                }
            }

            return matchedRecords;
        }

        private List<FileCabinetRecord> SelectRecords(PropertyInfo property, string value)
        {
            try
            {
                switch (property.Name.ToUpperInvariant())
                {
                    case "ID": return Array.FindAll(this.Service.GetRecords().ToArray(), i => i.Id.Equals(int.Parse(value, CultureInfo.InvariantCulture))).ToList();
                    case "FIRSTNAME": return this.Service.FindByFirstName(new string(value)).ToList();
                    case "LASTNAME": return this.Service.FindByLastName(new string(value)).ToList();
                    case "DATEOFBIRTH": return this.Service.FindByDateOfBirth(DateTime.Parse(value, CultureInfo.InvariantCulture)).ToList();
                    case "JOBEXPERIENCE": return this.Service.FindByJobExperience(short.Parse(value, CultureInfo.InvariantCulture)).ToList();
                    case "MONTHLYPAY": return this.Service.FindByMonthlyPay(decimal.Parse(value, CultureInfo.InvariantCulture)).ToList();
                    case "GENDER":
                        {
                            if (value.Length > 1)
                            {
                                throw new FormatException();
                            }

                            return this.Service.FindByGender(char.Parse(value)).ToList();
                        }

                    default:
                        {
                            Console.WriteLine($"Property {property.Name} is not accepted as a search criteria.");
                            return new List<FileCabinetRecord>();
                        }
                }
            }
            catch (FormatException)
            {
                return new List<FileCabinetRecord>();
            }
        }

        private void Select(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException($"{parameters} is null.");
            }

            string[] args;
            string[] selectFields = Array.Empty<string>();
            string[] whereOrParameters = Array.Empty<string>();
            string[] whereAndFields;
            string[] whereAndValues;

            if (parameters.Length > 0)
            {
                args = parameters.Split("where", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries | (StringSplitOptions)StringComparison.InvariantCultureIgnoreCase);

                if (args.Length == 1 && Array.FindIndex(args[0].ToCharArray(), i => i.Equals('=')) == -1)
                {
                    selectFields = args[0].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                }

                if (args.Length == 1 && Array.FindIndex(args[0].ToCharArray(), i => i.Equals('=')) != -1)
                {
                    whereOrParameters = args[0].Split("or", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries | (StringSplitOptions)StringComparison.InvariantCultureIgnoreCase);
                }

                if (args.Length > 1)
                {
                    selectFields = args[0].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    whereOrParameters = args[1].Split("or", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries | (StringSplitOptions)StringComparison.InvariantCultureIgnoreCase);
                }
            }

            List<PropertyInfo> allProperties = typeof(FileCabinetRecord).GetProperties().ToList();
            string field;

            if ((field = CheckFields(selectFields, allProperties)) != null)
            {
                Console.WriteLine($"Field '{field}' unrecognized.");
                return;
            }

            List<FileCabinetRecord> recordsFound = new List<FileCabinetRecord>();

            foreach (string whereAnd in whereOrParameters)
            {
                GetStrings(whereAnd, out whereAndFields, out whereAndValues);

                if (whereAndFields.Length != whereAndValues.Length)
                {
                    Console.WriteLine("Not all criteria parameters got values.");
                    return;
                }

                if ((field = CheckFields(whereAndFields, allProperties)) != null)
                {
                    Console.WriteLine($"Field '{field}' unrecognized.");
                    return;
                }

                List<Tuple<PropertyInfo, string>> andCriteria = GetCriteriaList(allProperties, whereAndFields, whereAndValues);

                if (!andCriteria.Any())
                {
                    Console.WriteLine("Wrong or absent update parameters.");
                    return;
                }

                recordsFound = recordsFound.Concat(this.GetAndMatchedRecords(andCriteria)).ToList();
            }

            for (int i = 0; i < recordsFound.Count; i++)
            {
                FileCabinetRecord[] extra = Array.FindAll(recordsFound.ToArray(), rec => rec.Id == recordsFound[i].Id);

                if (extra.Length > 1)
                {
                    for (int j = 1; j < extra.Length; j++)
                    {
                        recordsFound.Remove(extra[j]);
                    }
                }
            }

            recordsFound.Sort(new FileCabinetRecordSort());

            List<PropertyInfo> selectedProperties = new List<PropertyInfo>();

            foreach (var prop in allProperties)
            {
                if (Array.Find(selectFields, i => i.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    selectedProperties.Add(prop);
                }
            }

            if (selectFields.Length == 0 && whereOrParameters.Length > 0)
            {
                selectedProperties = allProperties;
            }

            if (selectFields.Length > 0 && whereOrParameters.Length == 0)
            {
                recordsFound = this.Service.GetRecords().ToList();
            }

            if (selectFields.Length == 0 && whereOrParameters.Length == 0)
            {
                selectedProperties = allProperties;
                recordsFound = this.Service.GetRecords().ToList();
                this.printer.Print(recordsFound.AsReadOnly(), selectedProperties);
                return;
            }

            recordsFound.Sort(new FileCabinetRecordSort());

            this.printer.Print(recordsFound.AsReadOnly(), selectedProperties);
        }
    }
}
