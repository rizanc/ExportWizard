using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportWizard.DAL.Models.QuickExport
{
    public class Configuration
    {
        public String ChainBasedResort { get; set; }
        public String Resort { get; set; }       
        public Export MainExport { get; set; }
        public Export[] SubExports { get; set; }
    }

    public class Export
    {
        // This field is chain based. Not resort specific.
        public String ChainBased { get; set; }

        public ExportHeader Header { get; set; }
        public String[] Columns { get; set; }
    }

    public class ExportHeader
    {
        public String FileType { get; set; }
        public String FileDescription { get; set; }
        public String SourceViewCode { get; set; }

    }


}
