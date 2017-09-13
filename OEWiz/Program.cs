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
        private const string OUT_EXTENSION = ".sql";
        private bool allMandatoryParametersFound = false;

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
                    ShowParameters();
                    return;
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
                    else
                    {
                        resort = "*";
                    }

                    if (parameters.ContainsKey("cbr"))
                    {
                        cbr = parameters["cbr"];
                    }
                    else
                    {
                        ShowParameters("cbr is mandatory");
                        return;
                    }

                    if (parameters.ContainsKey("in"))
                    {
                        var rootFilename = Path.GetFileNameWithoutExtension(parameters["in"]);
                        var filename = parameters["in"];

                        GenerateFile(rootFilename, filename, resort, cbr);
                    }
                    else
                    {
                        ProcessAllFiles(resort, cbr);
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
            Setup setup = LoadSetup();

            StringBuilder runAll = new StringBuilder();
            GenerateCleanFile(setup.Company, setup.ProgramName);

            runAll.AppendLine("--Optional, clean existing reports");
            runAll.AppendLine("@clean.sql\n/");

            runAll.AppendLine("--Create reports");
            var files = Directory.GetFiles(".", "*.json");
            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    if (!file.Contains("setup.config"))
                    {
                        runAll.AppendLine("@" + Path.GetFileNameWithoutExtension(file) + OUT_EXTENSION + "\n/");

                        var rootFilename = Path.GetFileNameWithoutExtension(file);
                        GenerateFile(rootFilename, file, resort, cbr);

                    }
                }

                runAll.AppendLine("-- Commit");
                runAll.AppendLine("commit;\n/");
                File.WriteAllText("runAll.sql", runAll.ToString());
            }
            else
            {
                throw new ArgumentException("No json files found.");
            }
        }

        private static void GenerateCleanFile(String companyName, String programName)
        {
            StringBuilder cleanFile = new StringBuilder();
            cleanFile.Append(@"

declare 

  in_resort varchar2(50) := '*';
  in_companyName varchar2(200) := '" + companyName + @"';
  in_programName varchar2(200):='" + programName + @"';
  
  -- Delete only the data. Leave the report itself intact.
  in_dataOnly boolean := false;
 
  
procedure clean_reports( in_resort Varchar2 , in_exp_file_id Number, in_dataOnly Boolean := true) is
begin

-- DELETE DATA
delete from exp_temp_data
where exp_file_id = in_exp_file_id
  and resort = in_resort;

delete
from exp_data_file dataFile
where dataFile.exp_data_id in (
  select exp_data_id 
    from exp_data_hdr dataHdr
   where dataHdr.exp_file_id = in_exp_file_id
     and dataHdr.resort = in_resort
);

delete
from exp_data_dtl dataDtl
where dataDtl.exp_data_id in (
 select exp_data_id 
    from exp_data_hdr dataHdr
   where dataHdr.exp_file_id = in_exp_file_id
);


delete
from exp_data_hdr dataHdr
where dataHdr.exp_file_id = in_exp_file_id
  and dataHdr.resort = in_resort;


if in_dataOnly = false then

-- DELETE REPORT

delete
from exp_file_delivery 
where exp_file_id = in_exp_file_id;

delete 
from exp_file_dtl
where exp_file_id = in_exp_file_id;


delete
from exp_data_hdr dataHdr
where exp_file_id = in_exp_file_id
  and resort = in_resort;

delete 
from exp_file_hdr
where exp_file_id = in_exp_file_id
  and resort = in_resort;

  dbms_output.put_line('Cleaned Report');
else
  dbms_output.put_line('Cleaned OLD Data');
  
end if;

END;



begin
dbms_output.put_line('Properties to clean:'||in_Resort);
FOR rec IN (
                SELECT c.resort,c.exp_file_id,c.file_type_desc
                FROM    exp_file_hdr c
                WHERE   ( c.resort = in_resort or in_resort = '*' )
                AND     c.program_name = in_programName
                AND     c.company = in_companyName 
                AND     c.exp_file_id > 0)

        LOOP BEGIN 

          dbms_output.put_line('Cleaning:' || rec.Resort ||' '||rec.exp_file_id||' '||rec.file_type_desc);
          clean_reports(rec.Resort, rec.exp_file_id, in_dataOnly);
        END;
        
    end LOOP;

    commit;  
end;




            ");

            File.WriteAllText("clean.sql", cleanFile.ToString());
        }

        private static Setup LoadSetup()
        {
            Setup setup = null;
            string setupFile = "./setup.config.json";

            if (File.Exists(setupFile))
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
                par.AppendLine("!!!!!!!" + error + "!!!!!");
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
            par.AppendLine("cbr:ABC           Mandatory, the chain based resort. The data shared among resorts (ex. profiles) will only be generated by this resort.");
            par.AppendLine("v.001=============================================================================");
            Console.WriteLine(par.ToString());

        }
    }


}
