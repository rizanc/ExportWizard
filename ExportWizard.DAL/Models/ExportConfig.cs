using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportWizard.DAL.Models.QuickExport
{
    public class Configuration
    {
        public String Resort { get; set; }
        public String ChainBasedResort { get; set; }
        public String FileName { get; set; }
        public String CompanyName { get; set; }
        public String ProgramName { get; set; }
        public bool? RunInNa { get; set; }
        public Export MainExport { get; set; }
        public Export[] SubExports { get; set; }
    }

    public class Export
    {
        // This table is chain based. Not resort specific.
        public bool ChainBased { get; set; }

        public ExportHeader Header { get; set; }
        public String[] Columns { get; set; }
    }

    public class ExportHeader
    {
        public String FileType { get; set; }
        public String FileDescription { get; set; }
        public String SourceViewCode { get; set; }
        public String WhereClause { get; set; }

    }

    /*
     * 
     * "host":"207.237.172.248",
	 "username": "mhg.dvexport",
	 "password":"@3135@AE699251B9625B90C8332DD4419420B7",
	 "directory":"/"
     */
    public class Delivery
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Directory { get; set; }
    }

    public class Setup
    {
        public Delivery Delivery { get; set; }
    }

}
