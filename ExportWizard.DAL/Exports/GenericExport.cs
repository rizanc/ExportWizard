using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportWizard.DAL.Exports
{
    public class GenericExport
    {
        public String GetSettings(String filename, String overrideResort = null)
        {
            String text = System.IO.File.ReadAllText(filename);

            var exportConfig = JsonConvert.DeserializeObject<Models.QuickExport.Configuration>(text);
            if (exportConfig == null)
            {
                throw new ArgumentException("Could not parse Export config located at " + filename);
            }

            ExportRecord mainExport = GetExportRecord(exportConfig.FileName, exportConfig.MainExport);

            //=========================================================================
            // OVERRIDES
            //=========================================================================
            if (overrideResort != null)
            {
                exportConfig.Resort = overrideResort;
            }
            //==========================================================================

            ExportModel export = new ExportModel()
            {
                Resort = exportConfig.Resort,
                MainExport = mainExport,
                SubExports = new List<ExportRecord>()
            };

            if (exportConfig.SubExports != null)
            {
                foreach (var subExport in exportConfig.SubExports)
                {
                    export.SubExports.Add(GetExportRecord(null,subExport));
                }
            }

            string exportSerialized = new ExportSerializer().Serialize(export);

            return exportSerialized;

        }

        public ExportRecord GetExportRecord(String filename, Models.QuickExport.Export export)
        {

            var header = GetDefaultHeader(
                filename,
                export.Header.FileType,
                export.Header.FileDescription,
                export.Header.SourceViewCode,
                export.Header.WhereClause);

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
                header = header,
                columns = details
            };

            return record;

        }


        private HeaderModel GetDefaultHeader(String filename, String fileType, String description, String exportTable, String whereClause)
        {
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
                RunInNaYn = "Y",
                ProgramName = "DataVisionReports",
                Company = "Datavision"
            };
        }

        private ColumnModel GetDetail(int counter, String field)
        {

            return new ColumnModel()
            {
                ExpFileDtlId = counter,
                ColName = field,
                ColLength = 4000,
                ColAlignment = "L",
                ColType = "VARCHAR2",
                OrderBy = counter

            };
        }

    }
}
