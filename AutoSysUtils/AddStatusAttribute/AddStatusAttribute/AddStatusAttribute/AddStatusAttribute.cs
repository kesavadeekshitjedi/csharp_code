using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.IO;

namespace AddStatusAttribute
{
    class AddStatusAttribute
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        StreamReader jilFileReader;
        StreamWriter statusJilWriter;

        static void Main(string[] args)
        {
            AddStatusAttribute asj = new AddStatusAttribute();
            Console.WriteLine("Enter the full path to the jil file: ");
            string jilFilePath = Console.ReadLine();
            Console.WriteLine("The file is at " + jilFilePath);
            Console.WriteLine("Enter the string (ON_ICE, ON_HOLD, SUCCESS, FAILURE, INACTIVE, TERMINATED) for the status attribute: ");
            string statusAttribute = Console.ReadLine();
            string statusFileName = null;
            if (File.Exists(jilFilePath))
            {
                asj.jilFileReader = new StreamReader(jilFilePath);
                string fileName = Path.GetFileNameWithoutExtension(jilFilePath);
                string folderName = Path.GetDirectoryName(jilFilePath);
                string copyFileName = folderName + "\\"+fileName + "_copy.txt";
                statusFileName = folderName + "\\"+fileName + "_status.txt";
                asj.statusJilWriter = new StreamWriter(statusFileName);
                File.Copy(jilFilePath, copyFileName,true);
                string currentJilLine = null;
                while((currentJilLine=asj.jilFileReader.ReadLine())!=null)
                {
                    if(currentJilLine.Contains("insert_job:"))
                    {

                        asj.statusJilWriter.WriteLine(currentJilLine);
                        asj.statusJilWriter.WriteLine("status: "+statusAttribute);
                        continue;
                    }
                    asj.statusJilWriter.WriteLine(currentJilLine);
                    
                }
            }
            asj.statusJilWriter.Close();
            asj.jilFileReader.Close();
            AddStatusAttribute.addEmailAttributes(statusFileName);
        }

        public static void addEmailAttributes(string jilFilePath)
        {
            StreamReader jilFileReader;
            StreamWriter jilFileWriter;
            if(File.Exists(jilFilePath))
            {
                jilFileReader = new StreamReader(jilFilePath);
                string fileName = Path.GetFileNameWithoutExtension(jilFilePath);
                string folderName = Path.GetDirectoryName(jilFilePath);
                string copyFileName = folderName + "\\" + fileName + "_statuscopy.txt";
                string statusFileName = folderName + "\\" + fileName + "_WithEmailAttributes.txt";
                jilFileWriter = new StreamWriter(statusFileName);
                File.Copy(jilFilePath, copyFileName, true);
                string currentJilLine = null;
                while ((currentJilLine = jilFileReader.ReadLine()) != null)
                {
                    if (currentJilLine.Contains("insert_job:"))
                    {

                        jilFileWriter.WriteLine(currentJilLine);
                        jilFileWriter.WriteLine("alarm_if_fail: 1 ");
                        jilFileWriter.WriteLine("alarm_if_terminated: 1 ");
                        jilFileWriter.WriteLine("notification_template: email1 ");
                        jilFileWriter.WriteLine("notification_alarm_types: ALL ");
                        jilFileWriter.WriteLine("send_notification: F");
                        jilFileWriter.WriteLine("notification_emailaddress: iscomputeroperations@fastenal.com ");
                        continue;
                    }
                    jilFileWriter.WriteLine(currentJilLine);

                }
                jilFileWriter.Close();
                jilFileReader.Close();
            }
           
        
        }
    }
}
