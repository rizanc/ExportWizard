using ExportWizard.DAL.Exports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEWiz
{
    class Program
    {
        private const string OUT_EXTENSION = ".txt";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                    var files= Directory.GetFiles(".", "*.json");
                    if (files != null && files.Length > 0)
                    {
                        foreach (var file in files)
                        {
                            var rootFilename = Path.GetFileNameWithoutExtension(file);
                            GenerateFile(rootFilename, file);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("No json files found.");
                    }

                }
                else
                {
                    var rootFilename = Path.GetFileNameWithoutExtension(args[0]);
                    var filename = args[0];

                    GenerateFile(rootFilename, filename);

                }

            }
            catch (ArgumentException ex)
            {

                Console.Write(GetParameters());
            }
        }

        private static void GenerateFile(string rootFilename, string filename)
        {
            var settings = new GenericExport().GetSettings(filename);
            System.IO.File.WriteAllText(rootFilename + OUT_EXTENSION, settings);
            Console.WriteLine("Wrote data to " + rootFilename + OUT_EXTENSION);
        }

        public static string GetParameters()
        {
            StringBuilder par = new StringBuilder();
            par.AppendLine("Usage:");
            par.AppendLine("===============================");
            par.AppendLine("OEWiz config.json");
            par.AppendLine("or OEWiz (will find all .json files)");

            return par.ToString();

        }
    }
}
