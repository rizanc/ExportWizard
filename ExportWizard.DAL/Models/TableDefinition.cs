using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportWizard.DAL.Models
{

    public class TableDefinitions
    {
        public Dictionary<String,Table> Tables { get; set; }
    }

    public class Table
    {
        public String Name;
        public Dictionary<String,TableColumn> Columns;
    }

    public class TableColumn
    {
        public String Name { get; set; }
        public String Type { get; set; }
    }
}
