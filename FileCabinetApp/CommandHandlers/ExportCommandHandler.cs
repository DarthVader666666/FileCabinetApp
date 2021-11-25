using System;
using System.Globalization;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles export command.
    /// </summary>
    public class ExportCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">FileCabinetService instance.</param>
        public ExportCommandHandler(IFileCabinetService service)
        {
            this.fileCabinetService = service;
        }

        /// <summary>
        /// Calls export method or next handler.
        /// </summary>
        /// <param name="request">Provides command and parameters.</param>
        public override void Handle(AddCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException($"{request} is null");
            }

            if (request.Command.Equals("export", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Export(request.Parameters);
            }
            else
            {
                this.NextHandler.Handle(request);
            }
        }

        private void Export(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentException("Parameters argument is null");
            }

            string[] exportParams = parameters.Split(' ');
            char unswer = ' ';
            bool run;

            if (File.Exists(exportParams[1]))
            {
                Console.WriteLine($"File exists - rewrite {exportParams[1]}? [Y/n]");
                do
                {
                    try
                    {
                        unswer = char.ToUpper(char.Parse(Console.ReadLine()), CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        unswer = ' ';
                    }

                    if (!(unswer == 'Y' || unswer == 'N'))
                    {
                        Console.WriteLine("Type Y or N");
                        run = true;
                    }
                    else
                    {
                        run = false;
                    }
                }
                while (run);
            }
            else
            {
                StreamWriter stream = new StreamWriter(exportParams[1]);
                stream.Close();
            }

            switch (unswer)
            {
                case 'Y': this.ExportToFile(exportParams[0], exportParams[1]); break;
                case 'N': break;
                default: this.ExportToFile(exportParams[0], exportParams[1]); break;
            }
        }

        private void ExportToFile(string format, string path)
        {
            FileStream file;
            FileCabinetServiceSnapshot snapshot;
            StreamWriter streamWriter;

            try
            {
                file = new FileStream(path, FileMode.Open);
                file.Dispose();
                file.Close();
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Export failed: can't open file {path}");
                return;
            }

            switch (format.ToUpper(CultureInfo.InvariantCulture))
            {
                case "CSV":
                    snapshot = this.fileCabinetService.MakeSnapshot();
                    streamWriter = new StreamWriter(path);
                    snapshot.SaveToCsv(streamWriter);
                    streamWriter.Close();
                    Console.WriteLine($"All records are exported to file {path}");
                    break;
                case "XML":
                    snapshot = this.fileCabinetService.MakeSnapshot();
                    streamWriter = new StreamWriter(path);
                    snapshot.SaveToXml(streamWriter);
                    streamWriter.Close();
                    Console.WriteLine($"All records are exported to file {path}");
                    break;
                default: Console.WriteLine("Unsupported file format"); break;
            }
        }
    }
}
