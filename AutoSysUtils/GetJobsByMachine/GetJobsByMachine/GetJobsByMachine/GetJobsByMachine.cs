using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using log4net;
using log4net.Config;

namespace GetJobsByMachine
{
    class GetJobsByMachine
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        Dictionary<string, string> machineJobDef = new Dictionary<string, string>();
        List<string> machineList = new List<string>();
        List<string> jobList = new List<string>();
        StreamReader jilFileReader;
        StreamWriter machineFileWriter1, machineFileWriter2;
        // machineFileWriter1 writes to a file that matches the jobs defined to a machine
        // machineFileWriter2 writes to a file that matches everything else
        static void Main(string[] args)
        {
            GetJobsByMachine xJM = new GetJobsByMachine();
            Console.WriteLine("Enter the full path to the jil file: ");
            string jilFilePath = Console.ReadLine();
            Console.WriteLine("The file is at " + jilFilePath);
            if(File.Exists(jilFilePath))
            {
                xJM.jilFileReader = new StreamReader(jilFilePath);
                string currentJilLine = null;
                while((currentJilLine=xJM.jilFileReader.ReadLine())!=null)
                {
                    log.Info("Jil file found");
                    
                }
                Console.ReadLine();
            }
        }
    }
}
