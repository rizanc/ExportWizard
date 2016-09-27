using ExportWizard.DAL.Exports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEGen
{
    class Program
    {
        private const string OUT_EXTENSION = ".txt";

        static Dictionary<String, String> parameters = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 1 && args[0] == "?")
                {
                    ShowParameters();
                    return;
                }

                if (args.Length < 1)
                {
                    ProcessAllFiles();

                }
                else
                {

                    foreach (var arg in args)
                    {
                        var argPair = arg.Split(':');
                        if (argPair == null || argPair.Length != 2)
                        {
                            ShowParameters();
                            return;
                        }
                        else
                        {
                            if (argPair[0].ToLower().Equals("in"))
                            {
                                parameters.Add(argPair[0], argPair[1]);
                            }
                            else if (argPair[0].ToLower().Equals("resort"))
                            {
                                parameters.Add(argPair[0], argPair[1]);
                            }
                            else
                            {
                                ShowParameters();
                                return;
                            }
                        }

                    }

                    String resort = null;
                    if (parameters.ContainsKey("resort"))
                    {
                        resort = parameters["resort"];
                    }

                    if (parameters.ContainsKey("in"))
                    {
                        var rootFilename = Path.GetFileNameWithoutExtension(parameters["in"]);
                        var filename = parameters["in"];

                        GenerateFile(rootFilename, filename, resort);
                    }
                    else
                    {
                        ProcessAllFiles(resort);
                    }


                }

            }
            catch (ArgumentException ex)
            {

                ShowParameters();
            }
        }

        private static void ProcessAllFiles(String resort = null)
        {
            var files = Directory.GetFiles(".", "*.json");
            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    var rootFilename = Path.GetFileNameWithoutExtension(file);
                    GenerateFile(rootFilename, file, resort);
                }
            }
            else
            {
                throw new ArgumentException("No json files found.");
            }
        }

        private static void GenerateFile(string rootFilename, string filename, string resort = null)
        {
            var settings = new GenericExport().GetSettings(filename, resort);
            System.IO.File.WriteAllText(rootFilename + OUT_EXTENSION, settings);
            Console.WriteLine("Wrote data to " + rootFilename + OUT_EXTENSION);
        }

        public static void ShowParameters()
        {
            StringBuilder par = new StringBuilder();
            par.AppendLine("=============================================================================");
            par.AppendLine("Usage:                                                                     ==");
            par.AppendLine("=============================================================================");
            par.AppendLine("OEGen ?     // Get This Help Screen");
            par.AppendLine("or");
            par.AppendLine("OEGen [in:config.json] [resort:*]");
            par.AppendLine("");
            par.AppendLine("[in:config.json]  File to process. If not provided, process all .json files");
            par.AppendLine("[resort:*]        When present, overrides the resort in the .json");
            par.AppendLine("v.001=============================================================================");
            Console.WriteLine(  par.ToString() );

        }
    }


}
