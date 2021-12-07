using System;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles insert command.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Specific FileCabinet service.</param>
        public InsertCommandHandler(IFileCabinetService service)
            : base(service)
        {
            CreateRecordEvent += this.Service.CreateRecord;
        }

        /// <summary>
        /// Create record handler
        /// </summary>
        private static event EventHandler<FileCabinetEventArgs> CreateRecordEvent;

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

            if (request.Command.Equals("insert", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Insert(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private static int GetIndex(string[] recordFields, string fieldName)
        {
            return Array.FindIndex(recordFields, i => i.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase));
        }

        private void Insert(string parameters)
        {
            this.Service.ClearCache();

            string[] args = parameters.Split(new string[] { "values", "(", ")" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (args.Length != 2)
            {
                Console.WriteLine("Unrecognized insert parameters. Ex: insert (id, firstname, lastname, dateofbirth...) values ('1', 'John', 'Doe', '5/18/1986'...)");
                return;
            }

            string[] recordFieldNames = args[0].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] recordFieldValues = args[1].Split(new char[] { ',', '\'', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (recordFieldNames.Length != recordFieldValues.Length)
            {
                Console.WriteLine("Quantity of fields and values doesn't match.");
                return;
            }

            FileCabinetRecord record = new FileCabinetRecord();

            try
            {
                record.Id = int.Parse(recordFieldValues[GetIndex(recordFieldNames, nameof(record.Id))], CultureInfo.InvariantCulture);

                if (this.Service.RecordExists(record.Id))
                {
                    Console.WriteLine($"Records #{record.Id} already exists.");
                    return;
                }

                if (record.Id < 1)
                {
                    Console.WriteLine($"Id can't be negative.");
                    return;
                }

                if (Array.FindIndex(recordFieldNames, i => i.Equals(nameof(record.FirstName), StringComparison.InvariantCultureIgnoreCase)) == -1)
                {
                    record.FirstName = string.Empty;
                }
                else
                {
                    record.FirstName = recordFieldValues[GetIndex(recordFieldNames, nameof(record.FirstName))];
                }

                if (Array.FindIndex(recordFieldNames, i => i.Equals(nameof(record.LastName), StringComparison.InvariantCultureIgnoreCase)) == -1)
                {
                    record.LastName = string.Empty;
                }
                else
                {
                    record.LastName = recordFieldValues[GetIndex(recordFieldNames, nameof(record.LastName))];
                }

                if (Array.FindIndex(recordFieldNames, i => i.Equals(nameof(record.DateOfBirth), StringComparison.InvariantCultureIgnoreCase)) == -1)
                {
                    record.DateOfBirth = default(DateTime);
                }
                else
                {
                    record.DateOfBirth = DateTime.Parse(recordFieldValues[GetIndex(recordFieldNames, nameof(record.DateOfBirth))], CultureInfo.InvariantCulture);
                }

                if (Array.FindIndex(recordFieldNames, i => i.Equals(nameof(record.JobExperience), StringComparison.InvariantCultureIgnoreCase)) == -1)
                {
                    record.JobExperience = default(short);
                }
                else
                {
                    record.JobExperience = short.Parse(recordFieldValues[GetIndex(recordFieldNames, nameof(record.JobExperience))], CultureInfo.InvariantCulture);
                }

                if (Array.FindIndex(recordFieldNames, i => i.Equals(nameof(record.MonthlyPay), StringComparison.InvariantCultureIgnoreCase)) == -1)
                {
                    record.MonthlyPay = default(decimal);
                }
                else
                {
                    record.MonthlyPay = decimal.Parse(recordFieldValues[GetIndex(recordFieldNames, nameof(record.MonthlyPay))], CultureInfo.InvariantCulture);
                }

                if (Array.FindIndex(recordFieldNames, i => i.Equals(nameof(record.Gender), StringComparison.InvariantCultureIgnoreCase)) == -1)
                {
                    record.Gender = default(char);
                }
                else
                {
                    record.Gender = char.Parse(recordFieldValues[GetIndex(recordFieldNames, nameof(record.Gender))]);
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("One or more field names are incorrect or absent.");
                return;
            }
            catch (FormatException)
            {
                Console.WriteLine("One or more filed names are incorrect or absent.");
                return;
            }

            FileCabinetEventArgs recordArgs = new FileCabinetEventArgs(record);
            CreateRecordEvent(null, recordArgs);
            Console.WriteLine($"Record #{record.Id} was successfully incerted.");
        }
    }
}
