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
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        Dictionary<string, string> machineJobDef = new Dictionary<string, string>();
        List<string> machineList = new List<string>();
        List<string> jobList = new List<string>();
        StreamReader jilFileReader;
        StreamWriter machineFileWriter1, machineFileWriter2;
        string jobDefString, goodJobDefString;
        string newJobDefinitionString = null;
        // machineFileWriter1 writes to a file that matches the jobs defined to a machine
        // machineFileWriter2 writes to a file that matches everything else
        static void Main(string[] args)
        {
            GetJobsByMachine xJM = new GetJobsByMachine();
            Console.WriteLine("Enter the full path to the jil file: ");
            string jilFilePath = Console.ReadLine();
            Console.WriteLine("The file is at " + jilFilePath);
            Console.WriteLine("Enter the machine for which jobs have to be extracted.");
            string xMachine = Console.ReadLine().Trim();
            if (File.Exists(jilFilePath))
            {
                xJM.machineFileWriter1 = new StreamWriter("d:\\goodjobs.txt");
                xJM.machineFileWriter2 = new StreamWriter("d:\\restjobs.txt");
                logger.Info("Jil file found");
                bool jobFound = false;
                xJM.jilFileReader = new StreamReader(jilFilePath);
                string currentJilLine = null;
                string currentJobName = null;
                string newJobName = null;
                while ((currentJilLine = xJM.jilFileReader.ReadLine()) != null)
                {
                    

                   
                   
                    //logger.Info(xJM.jobDefString);
                    if(currentJilLine.Contains("insert_job:"))
                    {
                        if(currentJobName==null & newJobName==null) //This is the absolute first time a job def is found.
                        {
                           
                            logger.Info("This is the first job in the file");
                            
                            
                            string tempjobNameString = currentJilLine.Split(':')[1];
                            currentJobName = tempjobNameString.Substring(0, tempjobNameString.IndexOf("job_type")).Trim();
                            xJM.jobDefString += currentJilLine + "\n";
                            continue;
                        }
                        if(currentJobName!=null & newJobName==null) //This case is where a prev job has already been captured and a new job is found
                        {
                            
                            logger.Info("New Job found");

                            string tempjobNameString = currentJilLine.Split(':')[1];
                            newJobName = tempjobNameString.Substring(0, tempjobNameString.IndexOf("job_type")).Trim();
                            xJM.newJobDefinitionString += xJM.jobDefString;
                            if (xJM.jobDefString.Contains("machine: " + xMachine))
                            {
                                logger.Info("Search machine identified");
                                xJM.machineFileWriter1.WriteLine(xJM.jobDefString);
                            }
                            else
                            {
                                xJM.machineFileWriter2.WriteLine(xJM.jobDefString);
                            }
                            xJM.jobDefString = null;
                            xJM.jobDefString += currentJilLine + "\n";
                            continue;
                        }
                        if(currentJobName!=null & newJobName!=null)
                        {
                            currentJobName = null;
                            newJobName = null;
                            string tempjobNameString = currentJilLine.Split(':')[1];
                            newJobName = tempjobNameString.Substring(0, tempjobNameString.IndexOf("job_type")).Trim();
                            xJM.newJobDefinitionString += xJM.jobDefString;
                            
                            if(xJM.jobDefString.Contains("machine: "+xMachine))
                            {
                                logger.Info("Search machine identified");
                                xJM.machineFileWriter1.WriteLine(xJM.jobDefString);
                            }
                            else
                            {
                                xJM.machineFileWriter2.WriteLine(xJM.jobDefString);
                            }
                            xJM.jobDefString = null;
                            xJM.jobDefString += currentJilLine + "\n";
                            continue;
                        }
                      
                        
                    }
                    if (!currentJilLine.Contains("/*"))
                    {
                        xJM.jobDefString += currentJilLine + "\n";
                    }
                    /* if(currentJobName!=null & newJobName==null)
                     {

                         logger.Info("Dealing with the same job: " + currentJobName);
                         xJM.jobDefString += currentJilLine + "\n";
                         if(currentJilLine.Contains("insert_job:") & (currentJobName!=null) & (newJobName==null) & (jobFound!=true))
                         {
                             logger.Info("New Job found");
                             string tempjobNameString = currentJilLine.Split(':')[1];
                             newJobName = tempjobNameString.Substring(0, tempjobNameString.IndexOf("job_type")).Trim();
                             jobFound = false;
                         }
                     }
                     if(currentJobName==null & newJobName!=null)
                     {
                         logger.Info("Dealing with the new job identified: " + newJobName);
                         xJM.jobDefString += currentJilLine + "\n";
                     }
                     if (!xJM.machineJobDef.ContainsKey(xMachine))
                     {
                         xJM.machineJobDef[xMachine] = xJM.goodJobDefString;

                     }
                     else
                     {
                         xJM.goodJobDefString = xJM.goodJobDefString + xJM.jobDefString;
                         xJM.machineJobDef[xMachine] = xJM.goodJobDefString;
                     }*/
                    
                }
                if (xJM.jobDefString.Contains("machine: " + xMachine))
                {
                    logger.Info("Search machine identified");
                    xJM.machineFileWriter1.WriteLine(xJM.jobDefString);
                }
                else
                {
                    xJM.machineFileWriter2.WriteLine(xJM.jobDefString);
                }
                xJM.machineFileWriter1.Close();
                xJM.machineFileWriter2.Close();
                Console.ReadLine();
            }
            else
            {
                logger.Error(jilFilePath + " not found");
                Console.ReadLine();
            }
        }
    }
}
