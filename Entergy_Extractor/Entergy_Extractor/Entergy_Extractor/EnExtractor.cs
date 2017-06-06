using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices.ComTypes;


namespace Entergy_Extractor
{
   

    class EnExtractor
    {
        List<String> jobNameList = new List<string>();
        Dictionary<List<String>, List<String>> jobNameOwnerMap = new Dictionary<List<string>, List<string>>();
        List<String> jobOwners = new List<string>();
        List<string> jobStandardList = new List<string>();

        Dictionary<String, String> userInfoDictionary = new Dictionary<string, string>();
        List<string> userList = new List<string>();
        static void Main(string[] args)
        {
            EnExtractor enx = new EnExtractor();
            enx.readXMLFile("D:\\Entergy-Files\\WCC0004.xml");
            Console.WriteLine("Done");
            enx.readJilForAppGroups("D:\\Entergy-Files\\R11.3.5Jobs.jil");
            enx.readFileForContacts();
        }

        public void readXMLFile(string xmlInput)
        {
            String userName = "";
            string firstname = null, lastname = null;
            String fullName = "";
            XmlDataDocument xmlDoc = new XmlDataDocument();
            XmlNodeList xmlNodes;
            FileStream fs = new FileStream(xmlInput, FileMode.Open, FileAccess.Read);
            xmlDoc.Load(fs);
            xmlNodes = xmlDoc.GetElementsByTagName("GlobalUser");
            for (int i = 0; i <= xmlNodes.Count - 1; i++)
            {
                //Console.WriteLine(xmlNodes[i].Name);
                foreach (XmlNode childNode in xmlNodes[i].ChildNodes)
                {
                    //Console.WriteLine(childNode.Name);
                    if (childNode.Name == "UserName" || childNode.Name == "FirstName" || childNode.Name == "LastName")
                    {
                        
                        Console.WriteLine(childNode.InnerText.Trim());
                        if(childNode.Name=="UserName")
                        {
                            userName = childNode.InnerText.Trim();
                            userList.Add(userName);
                        }
                       
                            
                        if(childNode.Name == "FirstName")
                        {
                            firstname = childNode.InnerText.Trim();
                        }
                        if(childNode.Name == "LastName")
                        {
                            lastname= childNode.InnerText.Trim();
                            fullName = firstname +" "+ lastname;
                            userInfoDictionary[userName] = fullName;
                        }
                            
                        
                        

                    }
                   

                }
            }
            // Reading Dictionary
            Console.WriteLine("Done loop");
            writeToExcel("User List");



        }

        public void readJilForAppGroups(String jilFileInput)
        {
            int jilStartIndex = 0;
            int jilEndIndex = 0;
            string jobStandard = null;
            var lines = File.ReadLines(jilFileInput);
            foreach (string jilLine in lines)
            {
                if(jilLine.Contains("insert_job: "))
                {
                    String[] jobNameTuple = jilLine.Split(':');
                    //Console.WriteLine(jobNameTuple);
                    string jobName = jobNameTuple[1].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0];
                    Console.WriteLine("=======================");
                    Console.WriteLine("Job Name Entry: " + jobName);
                    
                    jobNameList.Add(jobName);
                    int totalUnderscores = 0;
                    int underscoreLastIndex = 0;
                    if (jobName.Contains("_"))
                    {
                        totalUnderscores = jobName.Split('_').Length - 1;
                        underscoreLastIndex = jobName.LastIndexOf('_');
                    }
                    Console.WriteLine("Splitting on last underscore...");

                    if (underscoreLastIndex != 0)
                    {
                         jobStandard = jobName.Substring(0, underscoreLastIndex);
                    }
                    else
                    {
                        jobStandard = jobName;
                    }
                    Console.WriteLine("Job Name Standard: " + jobStandard);
                    jobStandardList.Add(jobStandard);
                    Console.WriteLine("=======================");
                }
            }
            writeToExcel("Job List");
        }

        public void readFileForContacts(String jobDocFile)
        {
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            object miss = System.Reflection.Missing.Value;
            object path = @"D:\Entergy-Files\doc\pxwhgen.doc";
            object readOnly = true;

            Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss,ref readOnly,ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);
            string totaltext = "";
            for (int i = 0; i < docs.Paragraphs.Count; i++)
            {
                totaltext += " \r\n " + docs.Paragraphs[i + 1].Range.Text.ToString();
            }
            Console.WriteLine(totaltext);
            docs.Close();
            word.Quit();
        }

        public void writeToExcel(string sheetname)
        {
            string tfirstname = null;
            string tlastname = null;
            string userID = null;

            Microsoft.Office.Interop.Excel.Application myExcel;
            Microsoft.Office.Interop.Excel._Workbook myXLWorkbook;
            Microsoft.Office.Interop.Excel._Worksheet myXLWorksheet;
            Microsoft.Office.Interop.Excel.Range myXLRange;
            object missingValue = System.Reflection.Missing.Value;
            int rowIndex = 2;
            int colIndex = 1;
            int jobIndex = 1;
            try
            {
                myExcel = new Microsoft.Office.Interop.Excel.Application();
                myExcel.Visible = true;
                myExcel.DisplayAlerts = false;
                // New workbook
                myXLWorkbook = (Microsoft.Office.Interop.Excel._Workbook)(myExcel.Workbooks.Add(""));
                if (sheetname == "User List")
                {
                    myXLWorkbook = (Microsoft.Office.Interop.Excel._Workbook)(myExcel.Workbooks.Add(""));
                    myXLWorksheet = (Microsoft.Office.Interop.Excel._Worksheet)(myXLWorkbook.ActiveSheet);
                    myXLWorksheet.Cells[1, 1] = "First Name";
                    myXLWorksheet.Cells[1, 2] = "Last Name";
                    myXLWorksheet.Cells[1, 3] = "User ID";

                  
                    foreach (KeyValuePair<string, string> entry in userInfoDictionary)
                    {
                        userID = entry.Key.Trim();
                        string[] values = entry.Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        tfirstname = values[0];
                        tlastname = values[1];
                        myXLWorksheet.Cells[rowIndex, colIndex] = tfirstname;
                        myXLWorksheet.Cells[rowIndex, colIndex + 1] = tlastname;
                        myXLWorksheet.Cells[rowIndex, colIndex + 2] = userID;
                        rowIndex++;

                    }
                    myXLRange = myXLWorksheet.get_Range("A1", "D1");
                    myXLRange.EntireColumn.AutoFit();
                    myXLWorksheet.Name = sheetname;
                }
                if(sheetname=="Job List")
                {
                    myXLWorkbook = myExcel.Workbooks.Open(@"D:\Entergy-Files\MyExcel.xlsx", 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                    myXLWorkbook.Sheets.Add(After: myXLWorkbook.Sheets[myXLWorkbook.Sheets.Count]);
                    myXLWorksheet = (Microsoft.Office.Interop.Excel._Worksheet)(myXLWorkbook.ActiveSheet);
                    myXLWorksheet.Cells[1, 1] = "Job Name";
                    
                    foreach(string job in jobNameList)
                    {
                        myXLWorksheet.Cells[jobIndex,1] = job;
                        jobIndex++;
                    }
                    myXLRange = myXLWorksheet.get_Range("A1", "D1");
                    myXLRange.EntireColumn.AutoFit();
                    myXLWorksheet.Name = sheetname;
                }
               
                

                // Rename Sheet1 to Users Sheet

               

                myXLWorkbook.SaveAs(@"D:\Entergy-Files\MyExcel.xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing);
                myXLWorkbook.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public void readFileForContacts()
        {
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            object miss = System.Reflection.Missing.Value;
            object path = @"D:\Entergy-Files\doc\pxwhgen.doc";
            object readOnly = true;
            bool printMessage = false;
            Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);
            string totaltext = "";
            for (int i = 0; i < docs.Paragraphs.Count; i++)
            {
                if(printMessage==true)
                {
                    Console.WriteLine(totaltext);
                }
                totaltext =  docs.Paragraphs[i + 1].Range.Text.ToString();
                if(totaltext.Contains("SUBSYSTEM") || (totaltext.Contains("JOB FAILURE")))
                {
                    Console.WriteLine(totaltext);
                    printMessage = true;
                }
                else
                {
                    printMessage = false;
                }
               
            }
            //Console.WriteLine(totaltext);
            docs.Close();
            word.Quit();
        }
    }
}
