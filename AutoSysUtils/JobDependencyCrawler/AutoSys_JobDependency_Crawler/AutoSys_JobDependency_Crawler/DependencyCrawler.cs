﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using log4net;
using log4net.Config;


namespace AutoSys_JobDependency_Crawler
{
    class DependencyCrawler
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string databaseHostName = null;
        string databaseName = null;
        string databasePort = null;
        string databaseUser = null;
        string databasePass = null;
        SqlConnection sqlConnection = null;
        List<string> masterDependencyList = new List<string>();
        int levelCounter = 0;
        static void Main(string[] args)
        {
            DependencyCrawler dc = new DependencyCrawler();
            logger.Info("Application Logging initialized...");
            List<string> dbProperties=dc.getDatabaseDetails();
            dc.databaseHostName = dbProperties[0];
            dc.databasePort = dbProperties[1];
            dc.databaseUser = dbProperties[2];
            dc.databasePass = dbProperties[3];
            dc.databaseName = dbProperties[4];

            string sqlConnectionString = "Data Source=" + dc.databaseHostName + "," + dc.databasePort + ";Initial Catalog=" + dc.databaseName + ";User ID=" + dc.databaseUser + ";Password=" + dc.databasePass+ ";MultipleActiveResultSets=true";
            logger.Debug(sqlConnectionString);
            dc.sqlConnection = new SqlConnection(sqlConnectionString);
            dc.sqlConnection.Open();
            logger.Info("Connected to SQL Server on :"+dc.databaseHostName+" \t SQL Server Version: "+dc.sqlConnection.ServerVersion);
            dc.getDependentJobList(dc.sqlConnection, "OTC_C_NA_DY_ipp003dysb");
            for (int i = 0; i <= dc.masterDependencyList.Count-1; i++)
            {
                string condJob = dc.masterDependencyList[i].Split(',')[1].Trim();
                logger.Debug("Getting Dependent job for : " + condJob);
                dc.getDependentJobList(dc.sqlConnection, condJob);
            }
            dc.closeDBConnections(dc.sqlConnection);
        }

        private int getJobID(SqlConnection sql,string jobName)
        {
            int joid = 0;
            string getJobID = "select joid from ujo_job where job_name=@job_name and is_active='1' and is_currver='1'";
            logger.Info("SQL Server connection state: " + sql.State);
            SqlCommand sqlCMD = new SqlCommand(getJobID, sql);
            sqlCMD.Parameters.AddWithValue("job_name", jobName);
            SqlDataReader joidReader = sqlCMD.ExecuteReader();
            if(joidReader.HasRows)
            {
                while (joidReader.Read())
                {
                    joid = joidReader.GetInt32(joidReader.GetOrdinal("joid"));
                    logger.Debug("Job ID for job: " + jobName + " is :" + joid);
                }
            }
            joidReader.Close();
            return joid;
        }
        private void getDependentJobList(SqlConnection sql,string jobName)
        {
            //int levelCounter = 0;
            DependencyCrawler dc1 = new DependencyCrawler();
            int myJobId=dc1.getJobID(sql, jobName);
            string getDependentJobsSQL = "select cond_job_name from ujo_job_cond where joid=@jobid and (type='s' or type='S' or type='f' or type='F' or type='t' or type='T' or type='n' or type='N' or type='d' or type='D' or type='e' or type='E')";
            SqlCommand sqlCmd = new SqlCommand(getDependentJobsSQL, sql);
            sqlCmd.Parameters.AddWithValue("jobid", myJobId);
            string depJobName = null;
            //List<string> dependentJobList = new List<string>();
            logger.Info("SQL Server connection state: " + sql.State);
            SqlDataReader sqlReader = sqlCmd.ExecuteReader();

            if(sqlReader.HasRows)
            {
                while(sqlReader.Read())
                {
                    depJobName = sqlReader.GetString(sqlReader.GetOrdinal("cond_job_name"));
                    logger.Debug("Dep Job found: " + depJobName);
                    masterDependencyList.Add(jobName+","+depJobName+",Level"+dc1.levelCounter);
                }
                
            }
            dc1.levelCounter++;
            /*foreach(string condJobString in masterDependencyList)
            {
                string condJob = condJobString.Split(',')[1].Trim();
                logger.Debug("Getting Dependent job for : " + condJob);
                getDependentJobList(sql, condJob);
            }*/

            

            sqlReader.Close();
            
        }

        private void closeDBConnections(SqlConnection sql)
        {
            sql.Close();
        }
        private List<string> getDatabaseDetails()
        {
            List<string> dbProperties = new List<string>();
            //logger = LogManager.GetLogger("Autosys_JobDependency_Crawler.getDatabaseDetails()");
            DependencyCrawler dc = new DependencyCrawler();
            logger.Info("Reading database.properties for AutoSys database information");
            StreamReader dbfileReader = new StreamReader("../../resources/database.properties");
            string currentDBLine = null;
            if(File.Exists("../../resources/database.properties"))
            {
                logger.Info("database.properties file found. Proceeding to gather info...");
                while ((currentDBLine = dbfileReader.ReadLine()) !=null)
                {
                    logger.Debug(currentDBLine);
                    if(currentDBLine.Contains("DB_HOST_NAME="))
                    {
                        string dbHostName = currentDBLine.Split('=')[1].Trim();
                        dbProperties.Add(dbHostName);
                        continue;
                    }
                    if(currentDBLine.Contains("DB_PORT_NUMBER="))
                    {
                        string dbPort = currentDBLine.Split('=')[1].Trim();
                        
                        dbProperties.Add(dbPort);
                        continue;
                    }
                    if (currentDBLine.Contains("DB_READ_USER="))
                    {
                        string dbUser = currentDBLine.Split('=')[1].Trim();
                        
                        dbProperties.Add(dbUser);
                        continue;
                    }
                    if (currentDBLine.Contains("DB_READ_PASS="))
                    {
                        string dbPass = currentDBLine.Split('=')[1].Trim();
                        
                        dbProperties.Add(dbPass);
                        continue;
                    }
                    if (currentDBLine.Contains("DB_NAME="))
                    {
                        string dbName = currentDBLine.Split('=')[1].Trim();
                        
                        dbProperties.Add(dbName);
                        continue;
                    }
                }
            }
            return dbProperties;
        }
    }
}
