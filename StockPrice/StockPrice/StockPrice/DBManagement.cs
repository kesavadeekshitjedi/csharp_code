using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using log4net;
using log4net.Config;
using System.Data;

namespace StockPrice
{
    class DBManagement
    {
        OracleConnection dbConn;
        private static readonly ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public OracleConnection createOracleConnection(string datasourceName, string userName, string password)
        {
            dbConn= new OracleConnection();
            OracleCommand osqlCommand = dbConn.CreateCommand();
            dbConn.ConnectionString = "User Id=" + userName + ";Password=" + password + ";Data Source=" + datasourceName;
            dbConn.Open();
            logger.Info("Connection state for " + datasourceName + ": " + dbConn.State);
            /*
            string get_sql = "select * from stock_info";
            osqlCommand.CommandText = get_sql;
            logger.Info(get_sql);
            OracleDataReader oReader = osqlCommand.ExecuteReader();
            while (oReader.Read())
            {
                string user = (string)oReader["STOCK_FULLNAME"];
                Console.WriteLine(user);
                //logger.Info("------------------------------");
                //logger.Info(user);
                //logger.Info("------------------------------");
            }
            */
            return dbConn;
        }

        public void insertNewStocks(OracleConnection oraConn)
        {
            string userResponse = "Yes";
            string stockString = "";
            List<string> stockList = new List<String>();
            while(userResponse!="No")
            {
                stockString = "";
                Console.WriteLine("Enter the full name of the Stock: ");
                string myStockName = Console.ReadLine();
                Console.WriteLine("Enter the average buy price: ");
                float buyPrice = float.Parse(Console.ReadLine());
                Console.WriteLine("Enter the number of units: ");
                int numShares = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter Buy Date: ");
                string shareBuyDate = Console.ReadLine();
                Console.WriteLine("Enter stock ticker");
                string myStockTicker = Console.ReadLine();
                stockString = myStockName + ":" + buyPrice.ToString() + ":" + numShares.ToString() + ":" + shareBuyDate + ":" + myStockTicker;
                logger.Info(stockString);
                if(!stockList.Contains(stockString))
                {
                    stockList.Add(stockString);
                    logger.Info("Stock " + myStockTicker + " added");
                }
                Console.WriteLine("Continue with more stocks?: ");
                userResponse=Console.ReadLine();

            }
            logger.Info(stockList);
            /*
            string stock_insert_sql = "insert into stock_info(stock_fullname,stock_buy_price,stock_units,stock_date,stock_ticker) values (@stock_fullName,@stock_buyPrice,@stockUnits,@stockBuyDate,@stockTicker)";
            OracleCommand oracleCmd = dbConn.CreateCommand();
            oracleCmd.CommandText = stock_insert_sql;

            OracleParameter stockNameParam = oracleCmd.Parameters.Add("@stock_fullName", SqlDbType.VarChar);
            OracleParameter stockBuyPrice = oracleCmd.Parameters.Add("@stock_buyPrice", SqlDbType.Float);
            OracleParameter stockUnits = oracleCmd.Parameters.Add("@stockUnits", SqlDbType.Int);
            OracleParameter stockBuyDate = oracleCmd.Parameters.Add("@stockBuyDate", SqlDbType.VarChar);
            OracleParameter stockTicker = oracleCmd.Parameters.Add("@stockTicker", SqlDbType.VarChar);
            */
            string insertStock = "";
            float insertStockPrice = 0.0F;
            int insertStockUnits = 0;
            string insertStockDate = "";
            string insertStockTicker = "";
            OracleCommand oracleCmd = dbConn.CreateCommand();
            foreach (string listItem in stockList)
            {
                String[] itemTuple = listItem.Split(':');
                insertStock = itemTuple[0];
                insertStockPrice = float.Parse(itemTuple[1]);
                insertStockUnits = int.Parse(itemTuple[2]);
                insertStockDate = itemTuple[3];
                insertStockTicker = itemTuple[4];
                string insert_stock = "insert into stock_info(stock_fullname,stock_buy_price,stock_units,stock_date,stock_ticker) values ('" + insertStock + "','" + insertStockPrice + "','" + insertStockUnits + "','" + insertStockDate + "','" + insertStockTicker + "')";
                logger.Debug(insert_stock);

                oracleCmd.CommandText = insert_stock;
                oracleCmd.ExecuteNonQuery();
            }
            
        }
    }
}
