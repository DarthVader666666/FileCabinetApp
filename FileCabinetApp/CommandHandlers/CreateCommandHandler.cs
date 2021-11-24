using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class CreateCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Create record handler
        /// </summary>
        private static event EventHandler<FileCabinetEventArgs> CreateRecordEvent;

        public CreateCommandHandler()
        {
            CreateRecordEvent += Program.fileCabinetService.CreateRecord;
        }

        private static void Create(string parameters)
        {
            if (parameters.Length > 0)
            {
                Console.WriteLine("Unrecognized parameter after command 'create'");
                
                return;
            }

            FileCabinetRecord record = new FileCabinetRecord();
            Program.InputRecordProperties(record);
            record.Id = Program.fileCabinetService.GetMaxId() + 1;
            FileCabinetEventArgs recordArgs = new FileCabinetEventArgs(record);
            CreateRecordEvent(null, recordArgs);
            Console.WriteLine($"Record #{record.Id} is created.");
        }

        public void Handle(AddCommandRequest request)
        {
            if (request is null)
            {

                return;
            }

            if (request.Command.Equals("create", StringComparison.InvariantCultureIgnoreCase))
            {
                Create(request.Parameters);
            }
        }
    }
}
