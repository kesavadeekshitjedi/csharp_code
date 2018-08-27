using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using System.IO;

namespace JilFileAnalyzer
{
    class JilFileAnalyzer
    {
        private static readonly ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static List<String> machineList = new List<string>();
        static List<String> jobList = new List<string>();
        static List<String> calendarList = new List<string>();
        static List<String> ownerList = new List<string>();
        static Dictionary<String, List<String>> jobMachineMap = new Dictionary<string, List<string>>();
        static void Main(string[] args)
        {

            logger.Info("Reading jil file to extract unique data");
            JilFileAnalyzer jf = new JilFileAnalyzer();
            //jf.getMachines("C:\\Users\\kesav\\OneDrive - Robert Mark Technologies\\RMT-Arch\\SpectrumBrands\\job_definitions-01\\job_definitions-01.txt");
            jf.getJobsPerMachine("C:\\Users\\kesav\\OneDrive - Robert Mark Technologies\\RMT-Arch\\SpectrumBrands\\job_definitions-01\\job_definitions-01.txt");
           
        }

        public void getMachines(String jilFile)
        {
            logger.Info("Getting unique machine list");
            StreamReader jilReader = new StreamReader(jilFile);
            string currentJilLine = "";
            while((currentJilLine=jilReader.ReadLine())!=null)
            {
                if(currentJilLine.Contains("machine:") & !currentJilLine.Contains("#"))
                {
                    string machString = currentJilLine.Split(':')[1].Trim();
                    logger.Info("Identified machine: " + machString);
                    Console.WriteLine(machString);
                    if(!machineList.Contains(machString))
                    {
                        machineList.Add(machString);
                    }
                }
            }
            for(int i=0;i<machineList.Count;i++)
            {
                Console.WriteLine("insert_machine: " + machineList[i]);
            }
            Console.WriteLine();
            jilReader.Close();
        }
        public void getJobsPerCalendar(String jilFile)
        {
            List<String> jobNameTempList;
            StreamReader jilReader = new StreamReader(jilFile);
            StreamWriter calendarMapWriter = new StreamWriter("C:\\Users\\kesav\\OneDrive - Robert Mark Technologies\\RMT-Arch\\SpectrumBrands\\job_definitions-01\\CalendarJobList.txt");
        }
        public void getJobsPerMachine(String jilFile)
        {
            List<String> jobNameTempList = new List<string>();
            StreamReader jilReader = new StreamReader(jilFile);
            StreamWriter writer = new StreamWriter("C:\\Users\\kesav\\OneDrive - Robert Mark Technologies\\RMT-Arch\\SpectrumBrands\\job_definitions-01\\JobMachineList.txt");
            string currentJilLine = "";
            string jobName = "";
            string machineName = "";
            while ((currentJilLine = jilReader.ReadLine()) != null)
            {
                if (currentJilLine.Contains("insert_job:") & (!currentJilLine.Contains("#")))
                {
                    jobNameTempList = new List<string>();
                    jobName = "";
                    machineName = "";
                    Console.WriteLine("Job Definition found in " + currentJilLine);
                    string tempjobNameString = currentJilLine.Split(':')[1];
                    jobName = tempjobNameString.Substring(0, tempjobNameString.IndexOf("job_type")).Trim();
                    string jobType = currentJilLine.Split(':')[2].Trim();
                    if(jobType=="BOX")
                    {
                        jobName = "";
                    }
                }
                if (currentJilLine.Contains("machine:") & !currentJilLine.Contains("#"))
                {
                    machineName = currentJilLine.Split(':')[1].Trim();
                    logger.Info("Identified machine: " + machineName);
                    Console.WriteLine(machineName);
                }
                if(!jobMachineMap.Keys.Contains(machineName) & (jobName!="" && machineName!=""))
                {
                    jobNameTempList.Add(jobName);
                    jobMachineMap[machineName] = jobNameTempList;
                    
                }
                else if(jobMachineMap.Keys.Contains(machineName) & (jobName != "" && machineName != ""))
                {
                    jobNameTempList = jobMachineMap[machineName];
                    if (!jobNameTempList.Contains(jobName))
                    {
                        jobNameTempList.Add(jobName);
                        jobMachineMap[machineName] = jobNameTempList;
                    }
                   
                    
                }
            }

            Console.WriteLine("Done mapping jobs to machines...");
            Console.WriteLine("Now printing...");
            foreach(KeyValuePair<String,List<String>> kvp in jobMachineMap)
            {
                
                writer.WriteLine("Machine: " + kvp.Key);
                for(int i=0;i<kvp.Value.Count;i++)
                {
                    writer.WriteLine("  Job: " + kvp.Value[i]);
                }
                writer.WriteLine("\n");
            }
            writer.Close();
            jilReader.Close();
        }
    }
}
