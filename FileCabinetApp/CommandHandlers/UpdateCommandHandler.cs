using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles 'update' command.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Specified file cabinet service.</param>
        public UpdateCommandHandler(IFileCabinetService service)
            : base(service)
        {
            UpdateRecordEvent += this.Service.UpdateRecord;
        }

        /// <summary>
        /// Update record handler
        /// </summary>
        private static event EventHandler<FileCabinetEventArgs> UpdateRecordEvent;

        /// <summary>
        /// Calls find method or next handler.
        /// </summary>
        /// <param name="request">Provides command and parameters.</param>
        public override void Handle(AddCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException($"{request} is null");
            }

            if (request.Command.Equals("update", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Update(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private static List<FileCabinetRecord> GetUpdatedRecords(List<FileCabinetRecord> recordsToUpdate, PropertyInfo[] listOfProperties, List<Tuple<PropertyInfo, string>> setCriteria)
        {
            List<FileCabinetRecord> updatedRecords = new List<FileCabinetRecord>();

            foreach (var record in recordsToUpdate)
            {
                FileCabinetRecord updatedRecord = new FileCabinetRecord();

                foreach (var property in listOfProperties)
                {
                    switch (property.Name.ToUpperInvariant())
                    {
                        case "ID": updatedRecord.Id = record.Id; break;
                        case "FIRSTNAME": updatedRecord.FirstName = new string(record.FirstName); break;
                        case "LASTNAME": updatedRecord.LastName = new string(record.LastName); break;
                        case "DATEOFBIRTH": updatedRecord.DateOfBirth = record.DateOfBirth; break;
                        case "JOBEXPERIENCE": updatedRecord.JobExperience = record.JobExperience; break;
                        case "MONTHLYPAY": updatedRecord.MonthlyPay = record.MonthlyPay; break;
                        case "GENDER": updatedRecord.Gender = record.Gender; break;
                        default: throw new ArgumentException("None of the parameters match the conditions.");
                    }
                }

                updatedRecords.Add(updatedRecord);
            }

            foreach (var record in updatedRecords)
            {
                foreach (var couple in setCriteria)
                {
                    switch (couple.Item1.Name.ToUpperInvariant())
                    {
                        case "FIRSTNAME": record.FirstName = new string(couple.Item2); break;
                        case "LASTNAME": record.LastName = new string(couple.Item2); break;
                        case "DATEOFBIRTH": record.DateOfBirth = DateTime.Parse(couple.Item2, CultureInfo.InvariantCulture); break;
                        case "JOBEXPERIENCE": record.JobExperience = short.Parse(couple.Item2, CultureInfo.InvariantCulture); break;
                        case "MONTHLYPAY": record.MonthlyPay = decimal.Parse(couple.Item2, CultureInfo.InvariantCulture); break;
                        case "GENDER":
                            {
                                if (couple.Item2.Length > 1)
                                {
                                    throw new FormatException();
                                }

                                record.Gender = couple.Item2[0];
                            }

                            break;

                        default: throw new ArgumentException("None of the parameters match the conditions.");
                    }
                }
            }

            return updatedRecords;
        }

        private static List<Tuple<PropertyInfo, string>> GetCriteriaList(PropertyInfo[] properties, string[] fields, string[] values)
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

        private void Update(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("'update' didn't get parameters (Ex: update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith')");
                return;
            }

            this.Service.ClearCache();

            string[] args = Array.Empty<string>();
            string[] setFields;
            string[] setValues;
            string[] whereFields;
            string[] whereValues;

            try
            {
                args = parameters.Split("set", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                args = args[0].Split("where", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("'set' or 'where' parameters wrong or absent.");
                return;
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("'set' or 'where' parameters wrong or absent.");
                return;
            }

            GetStrings(args[0], out setFields, out setValues);
            GetStrings(args[1], out whereFields, out whereValues);

            PropertyInfo[] listOfProperties = typeof(FileCabinetRecord).GetProperties();

            List<Tuple<PropertyInfo, string>> whereCriteria = GetCriteriaList(listOfProperties, whereFields, whereValues);
            List<Tuple<PropertyInfo, string>> setCriteria = GetCriteriaList(listOfProperties, setFields, setValues);

            if (!(whereCriteria.Any() && setCriteria.Any()))
            {
                Console.WriteLine("Wrong or absent update parameters.");
                return;
            }

            List<int> ids = this.GetAndMatchedFieldIds(whereCriteria);

            if (!ids.Any())
            {
                Console.WriteLine("No records matching given parameters where found.");
                return;
            }

            var allRecords = this.Service.GetRecords();
            List<FileCabinetRecord> recordsToUpdate = new List<FileCabinetRecord>();

            foreach (var id in ids)
            {
                recordsToUpdate.Add(Array.Find(allRecords.ToArray(), i => i.Id.Equals(id)));
            }

            List<FileCabinetRecord> updatedRecords = GetUpdatedRecords(recordsToUpdate, listOfProperties, setCriteria);
            List<FileCabinetEventArgs> updatedArguments = new List<FileCabinetEventArgs>();

            foreach (var record in updatedRecords)
            {
                updatedArguments.Add(new FileCabinetEventArgs(record));
            }

            foreach (var arg in updatedArguments)
            {
                UpdateRecordEvent(null, arg);
            }
        }

        private List<int> GetAndMatchedFieldIds(List<Tuple<PropertyInfo, string>> whereCriteria)
        {
            List<List<int>> listOfIdList = new List<List<int>>();
            List<int> matchedIds = new List<int>();

            foreach (var couple in whereCriteria)
            {
                matchedIds = this.SelectRecordIds(couple.Item1, couple.Item2).ToList();

                if (!matchedIds.Any())
                {
                    return new List<int>();
                }

                listOfIdList.Add(matchedIds);
            }

            foreach (var list in listOfIdList)
            {
                if (list.Count < matchedIds.Count)
                {
                    matchedIds = list;
                }
            }

            int[] ids;

            foreach (var list in listOfIdList)
            {
                ids = Array.FindAll(matchedIds.ToArray(), i => !list.Contains(i));

                foreach (int i in ids)
                {
                    matchedIds.Remove(i);
                }
            }

            return matchedIds;
        }

        private IEnumerable<int> SelectRecordIds(PropertyInfo property, string value)
        {
            var records = this.Service.GetRecords().ToList();

            try
            {
                switch (property.Name.ToUpperInvariant())
                {
                    case "ID": return from record in records where record.Id == int.Parse(value, CultureInfo.InvariantCulture) select record.Id;
                    case "FIRSTNAME": return from record in records where record.FirstName.Equals(value, StringComparison.InvariantCultureIgnoreCase) select record.Id;
                    case "LASTNAME": return from record in records where record.LastName.Equals(value, StringComparison.InvariantCultureIgnoreCase) select record.Id;
                    case "DATEOFBIRTH": return from record in records where record.DateOfBirth.Equals(DateTime.Parse(value, CultureInfo.InvariantCulture)) select record.Id;
                    case "JOBEXPERIENCE": return from record in records where record.JobExperience == short.Parse(value, CultureInfo.InvariantCulture) select record.Id;
                    case "MONTHLYPAY": return from record in records where record.MonthlyPay == decimal.Parse(value, CultureInfo.InvariantCulture) select record.Id;
                    case "GENDER":
                        {
                            if (value.Length > 1)
                            {
                                throw new FormatException();
                            }

                            return
                                from record in records
                                where char.ToUpper(record.Gender, CultureInfo.InvariantCulture).Equals(char.ToUpper(value[0], CultureInfo.InvariantCulture))
                                select record.Id;
                        }

                    default: throw new ArgumentException("None of the parameters match the conditions.");
                }
            }
            catch (FormatException)
            {
                return new List<int>();
            }
        }
    }
}
