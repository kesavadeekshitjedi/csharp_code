using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;
using System.Xml;

namespace RestClient_Test_Console
{
    class Program
    {
        private const string baseURL = "https://lumos:9443/AEWS/";
        static Dictionary<string, List<string>> jobs = new Dictionary<string, List<string>>();
        static List<string> jobAttributes = new List<string>();
        static string attributeName = "";
        static void Main(string[] args)
        {

            StringBuilder output = new StringBuilder();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            HttpClient aeClient = new HttpClient();
            aeClient.BaseAddress = new Uri(baseURL);
            var byteArray = Encoding.ASCII.GetBytes("daddepalli:Deek5581");
            aeClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            aeClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            HttpResponseMessage aeResponse = aeClient.GetAsync(baseURL + "job").Result;
            if (aeResponse.IsSuccessStatusCode)
            {

                //var dataObjects = aeResponse.Content.ReadAsAsync<IEnumerable<Program>>().Result;
                //foreach (var d in dataObjects)
                //{
                //    Console.WriteLine("{0}", d);
                //}

                var responseValue = string.Empty;
                string job_name = "";
                Task task = aeResponse.Content.ReadAsStreamAsync().ContinueWith(t =>
                {
                    var stream = t.Result;
                    using (var reader = new StreamReader(stream))
                    {
                        responseValue = reader.ReadLine();
                        //Console.WriteLine(responseValue);
                        String xmlString = responseValue;
                        XmlReader xmlReader = XmlReader.Create(new StringReader(xmlString));
                        XmlWriterSettings ws = new XmlWriterSettings();
                        ws.Indent = true;
                        using (XmlWriter writer = XmlWriter.Create(output, ws))
                        {

                            // Parse the file and display each of the nodes.
                            while (xmlReader.Read())
                            {
                                
                                switch (xmlReader.NodeType)
                                {
                                   
                                    case XmlNodeType.Element:
                                        writer.WriteStartElement(xmlReader.Name);
                                        if (xmlReader.Name == "name")
                                        {
                                            Console.WriteLine(xmlReader.Name);
                                            attributeName = "job_name:";
                                        }
                                        else if(xmlReader.Name=="jobType")
                                        {
                                            attributeName = "job_type:";
                                        }
                                        else if(xmlReader.Name=="machine")
                                        {
                                            attributeName = "machine: ";
                                        }
                                        else if(xmlReader.Name=="status")
                                        {
                                            attributeName = "status:";
                                        }
                                        break;
                                    case XmlNodeType.Text:
                                        writer.WriteString(xmlReader.Value);
                                        Console.WriteLine(xmlReader.Value);
                                        string attributeValue = attributeName + xmlReader.Value;
                                        jobAttributes.Add(attributeName + xmlReader.Value);
                                        Console.WriteLine(attributeName + xmlReader.Value);
                                        if(attributeName=="job_name:")
                                        {
                                            job_name = xmlReader.Value;
                                        }
                                       
                                        if(jobs.ContainsKey(job_name))
                                        {
                                            List<string> jobAttrs = jobs[job_name];
                                            jobAttrs.Add(attributeName + xmlReader.Value);
                                            jobs[job_name] = jobAttrs;
                                        }
                                        else
                                        {
                                            List<string> jobAttrs = new List<string>();
                                            jobAttrs.Add(attributeName + xmlReader.Value);
                                            jobs.Add(job_name, jobAttrs);
                                        }
                                        //job_name = "";
                                        attributeName = "";
                                        break;
                                    case XmlNodeType.XmlDeclaration:
                                    
                                    case XmlNodeType.Comment:
                                        //Console.WriteLine(xmlReader.Value);
                                        writer.WriteComment(xmlReader.Value);
                                        break;
                                    
                                }
                            }
                            //jobs.Add(job_name, jobAttributes);
                        }
                    }
                });

                task.Wait();
                //Console.WriteLine(aeResponse.Content);
                //Console.WriteLine(responseValue);
                Console.ReadKey();
                
             }
        }
    }
}
