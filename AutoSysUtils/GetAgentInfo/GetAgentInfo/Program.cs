using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GetAgentInfo
{
    class Program
    {
        
            // private static ILog logger = LogManager.GetLogger("ACCE.MassImportUtilities.DBConnection");
            StreamReader databasePropertiesFileReader = null;
            String dbname = null;
            String dbPort = null;
            String dbUser = null;
            String dbPasswod = null;
            String dbHost = null;
            String urlTupleTemp = null;
            public void readDBPropertiesFile(String databasePropertiesFile)
            {
                String propertiesFileName = databasePropertiesFile;
                //logger.Info("Reading " + databasePropertiesFile + " file to get DB Details");

                databasePropertiesFileReader = new StreamReader(propertiesFileName);
                String line = "";
                while ((line = databasePropertiesFileReader.ReadLine()) != null)
                {
                    if (line.Contains("jdbc.url"))
                    {
                        var builder = new SqlConnectionStringBuilder(line);


                    }
                }

            }
    
        static void Main(string[] args)
        {

        }
    }
}
