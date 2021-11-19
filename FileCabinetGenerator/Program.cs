using System;
using System.IO;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Class which operates FileCabinetGenerator application. 
    /// </summary>
    static class Program
    {
        private static string fileType;
        private static string filePath;
        private static int recordsAmount;
        private static int startId;
        private static StreamReader streamReader;


        static void Main(string[] args)
        {
            if (args.Length <= 8)
            {
                string wholeLine = string.Join(' ', args);
                args = wholeLine.Split(new char[] { ' ', '=' });
            }

            if ((args[0] == "--output-type" || args[0] == "-t") && (args[2] == "--output" || args[2] == "-o") &&
                (args[4] == "--records-amount" || args[4] == "-a") && (args[6] == "--start-id" || args[6] == "-i"))
            {
                fileType = args[1];
                filePath = args[3];

                if (!(fileType == "csv" || fileType == "xml"))
                {
                    Console.WriteLine("Wrong file type");
                    return;
                }

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("No such file.");
                    return;
                }

                try
                {
                    int.TryParse(args[5], out recordsAmount);
                }
                catch(ArgumentException)
                {
                    Console.WriteLine("Wrong records amount parameter.");
                    return;
                }

                try
                {
                    int.TryParse(args[7], out startId);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Wrong start Id parameter.");
                    return;
                }
            }

            streamReader = new StreamReader(filePath);
            streamReader.Close();
        }
    }
}
