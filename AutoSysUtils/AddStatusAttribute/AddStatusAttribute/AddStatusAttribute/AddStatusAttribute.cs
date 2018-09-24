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
            if (File.Exists(jilFilePath))
            {
                asj.jilFileReader = new StreamReader(jilFilePath);
                string fileName = Path.GetFileNameWithoutExtension(jilFilePath);
                string folderName = Path.GetDirectoryName(jilFilePath);
                string copyFileName = folderName + "\\"+fileName + "_copy.txt";
                string statusFileName = folderName + "\\"+fileName + "_status.txt";
                asj.statusJilWriter = new StreamWriter(statusFileName);
                File.Copy(jilFilePath, copyFileName,true);
                string currentJilLine = null;
                while((currentJilLine=asj.jilFileReader.ReadLine())!=null)
                {
                    if(currentJilLine.Contains("insert_job:"))
                    {

                        asj.statusJilWriter.WriteLine(currentJilLine);
                        asj.statusJilWriter.WriteLine("status: ON_ICE");
                        continue;
                    }
                    asj.statusJilWriter.WriteLine(currentJilLine);
                    
                }
            }
            asj.statusJilWriter.Close();
            asj.jilFileReader.Close();
        }
    }
}
