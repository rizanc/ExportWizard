using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExportWizard.DAL.Exports
{
    public class DataVision
    {
        public String GetSettings()
        {
            ExportRecord mainExport = GetApplicationUser();
            ExportRecord secondaryExport = GetCashiers();

            ExportModel export = new ExportModel()
            {
                MainExport = mainExport,
                SubExports = new List<ExportRecord>() { secondaryExport }
            };

            string exportSerialized = new ExportSerializer().Serialize(export);

            //StringBuilder sb = Muckify(ref exportSerialized);

            //exportSerialized = sb.ToString();

            return exportSerialized;
        }

        public ExportRecord GetApplicationUser()
        {

            var header = GetDefaultHeader("EXP_APPLICATION_USER", "Export for Application$_User", "APPLICATION$_USER");

            String[] fields = { "app_user", "app_user_id", "insert_date", "update_date" };

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

        public ExportRecord GetCashiers()
        {
            var header = GetDefaultHeader("EXP_CASHIER", "Export for CASHIERS", "CASHIERS");

            String[] fields = { "cashier_id", "resort", "title", "insert_date", "update_date" };


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

        private HeaderModel GetDefaultHeader(String fileType, String description, String exportTable)
        {
            return new HeaderModel()
            {
                FileGroupId = "MISC",
                FileType = fileType,
                FileDescription = description,
                SourceViewCode = exportTable,
                FileName = "'" + exportTable + "_'||PMS_P.resort||'_'||To_CHAR(pms_p.business_date,'YYYYMMDD')",
                FileExtension = "''csv''",
                ColSeparator = "TAB",
                WhereClause = "",
                RunInNaYn = "Y"
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
