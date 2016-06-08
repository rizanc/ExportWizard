using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportWizard.DAL
{

    public class ExportModel
    {
        public ExportModel()
        {
            //FileType = "NAEXPORT";
            Resort = "*";
            
        }

        public string Resort { get; set; }

        public ExportRecord MainExport { get; set; }
        public List<ExportRecord> SubExports { get; set; }

    }

    public class ExportRecord
    {

        public HeaderModel header { get; set; }
        public List<ColumnModel> columns { get; set; }
    }

    public class HeaderModel
    {
        public int Id { get; set; }
        public string FileType { get; set; }
        public string FileDescription { get; set; }
        public string FileGroupId { get; set; }
        public string Resort { get; set; }
        public string SourceViewCode { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string ColSeparator { get; set; }
        public string WhereClause { get; set; }
        public string RunInNaYn { get; set; }
        public string InactiveYn { get; set; }
        public DateTime InsertDate { get; set; }
        public int InsertUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUser { get; set; }

    }

    public class ColumnModel
    {
        public int ExpFileId { get; set; }
        public int ExpFileDtlId { get; set; }
        public string ColName { get; set; }
        public int ColLength { get; set; }
        public string ColAlignment { get; set; }
        public int OrderBy { get; set; }
        public string ColType { get; set; }
        public char DatabaseYn { get; set; }
        public char GenDataType { get; set; }
        public DateTime InsertDate { get; set; }
        public int InsertUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUser { get; set; }
        public char? IgnoreLengthYn { get; set; }

    }

    public class DeliveryModel
    {
        public int ExpFileId { get; set; }
        public string CommType { get; set; }
        public string HostUrl { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public DateTime InsertDate { get; set; }
        public int InsertUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUser { get; set; }
    }

    public enum OrderByType
    {
        Ascending = 0,
        Descending = 1
    }

    public class Configuration
    {
        public string[] Resorts { get; set; }
        public int UserId { get; set; }
        public string FileType { get; set; }
        public string FileTypeDesc { get; set; }

    }
}
