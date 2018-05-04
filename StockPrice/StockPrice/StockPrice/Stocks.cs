using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Oracle.ManagedDataAccess.Client;
using log4net.Config;

namespace StockPrice
{
    class Stocks
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            DBManagement dbms = new DBManagement();
            OracleConnection stockConn=dbms.createOracleConnection("RMT-FS-STOCK","stock_user","stock_user");
            GetStockPrice getStock = new GetStockPrice();
            //getStock.getStock_WSAsync("CA");
            //            dbms.insertNewStocks(stockConn);
            List<string> myStocks=dbms.getListofStocks(stockConn);
            foreach(string myStock in myStocks)
            {
                getStock.getStock_WSAsync(myStock);
            }

        }
    }
}
