using ExportWizard.DAL.Models.QuickExport;
using ExportWizard.DAL.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ExportWizard.DAL.Exports
{
    public class GenericExport
    {


        public Setup GetSetup(String filename)
        {
            Setup setup = null;

            String text = System.IO.File.ReadAllText(filename);
            Logger.Debug(text);

            setup = JsonConvert.DeserializeObject<Models.QuickExport.Setup>(text);
            if (setup == null)
            {
                throw new ArgumentException("Could not parse Setup located at " + filename);
            }

            return setup;

        }


        public String GetSettings(String filename, Setup setup, String overrideResort = null, String overrideCBR = null)
        {

            String text = System.IO.File.ReadAllText(filename);

            var exportConfig = JsonConvert.DeserializeObject<Models.QuickExport.Configuration>(text);
            if (exportConfig == null)
            {
                throw new ArgumentException("Could not parse Export config located at " + filename);
            }

            if (exportConfig.RunInNa == null)
            {
                exportConfig.RunInNa = false;
            }


            ExportRecord mainExport = GetExportRecord(exportConfig.FileName, exportConfig.CompanyName, exportConfig.ProgramName, (bool) exportConfig.RunInNa, exportConfig.MainExport);

            //=========================================================================
            // OVERRIDES
            //=========================================================================
            if (overrideResort != null)
            {
                exportConfig.Resort = overrideResort;
            }

            if (overrideCBR != null)
            {
                exportConfig.ChainBasedResort = overrideCBR;
            }
            //==========================================================================

            ExportModel export = new ExportModel()
            {
                ChainBasedResort = exportConfig.ChainBasedResort,
                Resort = exportConfig.Resort,
                MainExport = mainExport,
                RunInNa = (bool)exportConfig.RunInNa,
                ProgramName = exportConfig.ProgramName,
                CompanyName = exportConfig.CompanyName,
                SubExports = new List<ExportRecord>(),
                Setup = setup
            };

            if (exportConfig.SubExports != null)
            {
                foreach (var subExport in exportConfig.SubExports)
                {
                    export.SubExports.Add(GetExportRecord(null, exportConfig.CompanyName, exportConfig.ProgramName, (bool) exportConfig.RunInNa, subExport));
                }
            }

            string exportSerialized = new ExportSerializer().Serialize(export);

            return exportSerialized;

        }

        public ExportRecord GetExportRecord(
            String filename,
            String companyName,
            String programName,
            bool runInNa,
            Models.QuickExport.Export export)
        {

            var header = GetDefaultHeader(
                filename,
                export.Header.FileType,
                export.Header.FileDescription,
                export.Header.SourceViewCode,
                export.Header.WhereClause,
                companyName,
                programName,
                runInNa);

            String[] fields = export.Columns;

            var details = new List<ColumnModel>();
            int counter = 1;

            foreach (var field in fields)
            {
                details.Add(GetDetail(counter, field));
                counter++;
            }

            ExportRecord record = new ExportRecord()
            {
                ChainBased = export.ChainBased,
                header = header,
                columns = details
            };

            return record;

        }


        private HeaderModel GetDefaultHeader(String filename, String fileType, String description, String exportTable, String whereClause, String companyName, String programName, bool runInNa)
        {
            if (programName == null || programName == "" || companyName == null || companyName == "")
                throw new ArgumentException("ProgramName & CompanyName are mandatory parameters");

            return new HeaderModel()
            {
                FileGroupId = "MISC",
                FileType = fileType,
                FileDescription = description,
                SourceViewCode = exportTable,
                FileName = "'" + filename + "_'||PMS_P.resort||'_'||To_CHAR(pms_p.business_date,'YYYYMMDD')",
                FileExtension = "''csv''",
                ColSeparator = "TAB",
                WhereClause = whereClause,
                RunInNaYn = runInNa ? "Y" : "N",
                ProgramName = programName, // "DataVisionReports",
                Company = companyName // "Datavision"
            };
        }

        private ColumnModel GetDetail(int counter, String field)
        {
            String fieldName = field;
            String formattedName = field;

            if (field.Contains(":"))
            {
                string[] fields = field.Split(':');
                fieldName = fields[1];
                formattedName = fields[0];
            }

            return new ColumnModel()
            {
                ExpFileDtlId = counter,
                ColName = fieldName,
                ColFormatted = formattedName,
                ColLength = 4000,
                ColAlignment = "L",
                ColType = "VARCHAR2",
                OrderBy = counter

            };
        }

    }
}
