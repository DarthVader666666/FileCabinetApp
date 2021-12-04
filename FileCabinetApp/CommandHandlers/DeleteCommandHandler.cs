using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles 'delete' command.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Specific FileCabinet service.</param>
        public DeleteCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

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

            if (request.Command.Equals("delete", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Delete(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private void Delete(string parameters)
        {
            string[] args;
            string field;
            string value;

            try
            {
                args = parameters.Split(new string[] { "where", "=" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                field = args[0].ToUpperInvariant();
                value = args[1].Split('\'', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0];
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Wrong 'delete' command parameters. Ex: delete firstname = 'Vadim'");
                return;
            }

            PropertyInfo[] listOfProperties = typeof(FileCabinetRecord).GetProperties();
            List<int> ids = new List<int>();

            foreach (var property in listOfProperties)
            {
                if (field.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    ids = this.SelectRecordIds(property, value).ToList();
                    break;
                }
            }

            if (ids is null || ids.Count == 0)
            {
                Console.WriteLine("Found no records which match parameters.");
                return;
            }

            string recordIds = string.Empty;

            foreach (int id in ids)
            {
                this.fileCabinetService.DeleteRecord(id);
                recordIds = string.Concat(recordIds, $"#{id}, ");
            }

            Console.WriteLine($"Records {recordIds[.. ^2]} are deleted.");
        }

        private IEnumerable<int> SelectRecordIds(PropertyInfo property, string value)
        {
            var records = this.fileCabinetService.GetRecords().ToList();

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
