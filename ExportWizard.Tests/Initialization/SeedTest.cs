using ExportWizard.DAL;
using ExportWizard.DAL.Exports;
using ExportWizard.DAL.Models;
using ExportWizard.DAL.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ExportWizard.Tests.Initialization
{
    [TestClass]
    public class SeedTest
    {
        HeaderModel header = new HeaderModel();
        List<ColumnModel> columns = new List<ColumnModel>();

        [TestCategory("Initialization")]
        [TestMethod]
        public void LoadDefinitions()
        {
            var definitions = Loader.GetDefinitions();
            var json = new JavaScriptSerializer().Serialize(definitions);

        }

        [TestCategory("Initialization")]
        [TestMethod]
        public void Seed()
        {

            var script = new DataVision().GetSettings();

            //var muckified = Muckify(ref script);


        }

        private static StringBuilder Muckify(ref string exportSerialized)
        {
            exportSerialized = exportSerialized.Replace('\r', ' ');
            exportSerialized = exportSerialized.Replace('\n', ' ');
            exportSerialized = exportSerialized.Replace("  ", " ");
            //exportSerialized = exportSerialized.Replace("  ", " ");
            //exportSerialized = exportSerialized.Replace("  ", " ");

            StringBuilder sb = new StringBuilder();
            int counter = 0;
            foreach (var chars in exportSerialized.ToCharArray())
            {
                sb.Append(chars);
                if (counter > 40)
                {

                    if (chars == ' ')
                    {
                        sb.Append("\n");
                        counter = 0;
                    }

                }

                counter++;
            }

            return sb;
        }


    }
}
