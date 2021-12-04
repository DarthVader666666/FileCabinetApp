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
            CreateRecordEvent += this.fileCabinetService.CreateRecord;
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
            string[] args = parameters.Split(new string[] { "value", "(", ")" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

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

                if (this.fileCabinetService.RecordExists(record.Id))
                {
                    Console.WriteLine($"Records #{record.Id} already exists.");
                    return;
                }

                record.FirstName = recordFieldValues[GetIndex(recordFieldNames, nameof(record.FirstName))];
                record.LastName = recordFieldValues[GetIndex(recordFieldNames, nameof(record.LastName))];
                record.DateOfBirth = DateTime.Parse(recordFieldValues[GetIndex(recordFieldNames, nameof(record.DateOfBirth))], CultureInfo.InvariantCulture);
                record.JobExperience = short.Parse(recordFieldValues[GetIndex(recordFieldNames, nameof(record.JobExperience))], CultureInfo.InvariantCulture);
                record.MonthlyPay = decimal.Parse(recordFieldValues[GetIndex(recordFieldNames, nameof(record.MonthlyPay))], CultureInfo.InvariantCulture);
                record.Gender = char.Parse(recordFieldValues[GetIndex(recordFieldNames, nameof(record.Gender))]);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("One or more filed names are incorrect or absent.");
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
