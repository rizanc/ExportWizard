using ExportWizard.DAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportWizard.DAL.Utilities
{
    public class Loader
    {

        public static TableDefinitions GetDefinitions()
        {
            String line;

            String definitionsDir = Directory.GetCurrentDirectory() + "\\Definitions";

            String[] definitionFiles = Directory.GetFiles(definitionsDir);
            TableDefinitions tables = new TableDefinitions()
            {
                Tables = new Dictionary<string, Table>()
            };

            foreach (var filename in definitionFiles)
            {
                Table table = new Table();

                table.Name = Path.GetFileName(filename).Replace(".txt", "");

                tables.Tables.Add(table.Name, table);
                table.Columns = new Dictionary<string, TableColumn>();

                Logger.Debug("Reading definition for " + filename);
                StreamReader file = new StreamReader(filename);

                while ((line = file.ReadLine()) != null)
                {
                    String[] splitLine = line.Split('\t');


                    TableColumn column = new TableColumn()
                    {
                        Name = splitLine[0],
                        Type = splitLine[1]
                    };

                    table.Columns.Add(column.Name, column);
                }
            }
            return tables;
        }
    }
}
