using ExportWizard.DAL.Exports;
using ExportWizard.DAL.Models.QuickExport;
using ExportWizard.DAL.Utilities;
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
                            else if (argPair[0].ToLower().Equals("cbr"))
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
                    String cbr = null;

                    if (parameters.ContainsKey("resort"))
                    {
                        resort = parameters["resort"];
                    }

                    if (parameters.ContainsKey("cbr"))
                    {
                        cbr = parameters["cbr"];
                    }

                    if (parameters.ContainsKey("in"))
                    {
                        var rootFilename = Path.GetFileNameWithoutExtension(parameters["in"]);
                        var filename = parameters["in"];

                        GenerateFile(rootFilename, filename, resort, cbr);
                    }
                    else
                    {
                        ProcessAllFiles(resort,cbr);
                    }


                }

            }
            catch (ArgumentException ex)
            {
                Logger.Error(ex);
                ShowParameters(ex.Message);
            }
        }

        private static void ProcessAllFiles(String resort = null, string cbr = null)
        {
            var files = Directory.GetFiles(".", "*.json");
            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    var rootFilename = Path.GetFileNameWithoutExtension(file);
                    GenerateFile(rootFilename, file, resort, cbr);
                }
            }
            else
            {
                throw new ArgumentException("No json files found.");
            }
        }

        private static Setup LoadSetup()
        {
            Setup setup = null;
            string setupFile = "./setup.config";

            if( File.Exists(setupFile))
            {
                setup = new GenericExport().GetSetup(setupFile);
            }

            return setup;
        }

        private static void GenerateFile(string rootFilename, string filename, string resort = null, string cbr = null)
        {
            Logger.Debug("Working on " + filename);
            var settings = new GenericExport().GetSettings(
                filename, 
                LoadSetup(), 
                resort, 
                cbr);

            System.IO.File.WriteAllText(rootFilename + OUT_EXTENSION, settings);
            Console.WriteLine("Wrote data to " + rootFilename + OUT_EXTENSION);
            Logger.Debug("Wrote data to " + rootFilename + OUT_EXTENSION);
        }

        public static void ShowParameters(String error = null)
        {
            StringBuilder par = new StringBuilder();
            if (error != null)
            {
                par.AppendLine("=============================================================================");
                par.AppendLine(error);
            }
            par.AppendLine("=============================================================================");
            par.AppendLine("Usage:                                                                     ==");
            par.AppendLine("=============================================================================");
            par.AppendLine("OEGen ?     // Get This Help Screen");
            par.AppendLine("or");
            par.AppendLine("OEGen [in:config.json] [resort:*]");
            par.AppendLine("");
            par.AppendLine("[in:config.json]  File to process. If not provided, process all .json files");
            par.AppendLine("[resort:*]        When present, overrides the resort in the .json");
            par.AppendLine("[cbr:ABC]        When present, overrides the chain based resort in the .json");
            par.AppendLine("v.001=============================================================================");
            Console.WriteLine(  par.ToString() );

        }
    }


}
