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
                    throw new ArgumentException("No configuration provided.");

                var rootFilename = Path.GetFileNameWithoutExtension(args[0]);

                var settings = new GenericExport().GetSettings(args[0]);

                System.IO.File.WriteAllText(rootFilename +OUT_EXTENSION, settings);

                Console.WriteLine("Wrote data to " + rootFilename + OUT_EXTENSION);
                //Console.WriteLine(settings);
            }
            catch (ArgumentException ex)
            {

                Console.Write(GetParameters());
            }   
        }


        public static string GetParameters()
        {
            StringBuilder par = new StringBuilder();
            par.AppendLine("Usage:");
            par.AppendLine("===============================");
            par.AppendLine("OEWiz config.json");
            par.AppendLine("");

            return par.ToString();

        }
    }
}
