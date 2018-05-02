using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Newtonsoft.Json;
using System.Net.Http;
namespace StockPrice
{
    class GetStockPrice
    {
        static string apiKey="7V5RNT4HHZAP6FM6";
        public string alphavantageURL = "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=";
        private static readonly ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static Dictionary<string, List<string>> stockPriceData = new Dictionary<string, List<string>>();
        static List<string> stockInfoList = new List<string>();
        public async void getStock_WSAsync(string stockName)
        {
            string todaysDate = DateTime.Now.ToString("yyyy-MM-dd");
            logger.Info("Get Stock info for :" + stockName);
            HttpClient stock_client = new HttpClient();
            stock_client.BaseAddress = new Uri(alphavantageURL);
            stock_client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            logger.Debug(alphavantageURL + stockName + "&apikey=" + apiKey);
            HttpResponseMessage stock_response = stock_client.GetAsync(alphavantageURL + stockName + "&apikey=" + apiKey).Result;
            //logger.Debug(stock_response);
            if (stock_response.IsSuccessStatusCode)
            {
                logger.Info("Successfully retrieved info from AlphaVantage");
                var result = await stock_response.Content.ReadAsStringAsync();
                //logger.Debug(result);
                dynamic obj = JsonConvert.DeserializeObject(result);
                string stock_ticker = obj["Meta Data"]["2. Symbol"];
                string stock_high = obj["Time Series (Daily)"][todaysDate]["2. high"];
                string stock_low = obj["Time Series (Daily)"][todaysDate]["3. low"];
                string stock_close = obj["Time Series (Daily)"][todaysDate]["4. close"];

                logger.Info(stock_ticker);
                logger.Info(stock_high);
                logger.Info(stock_low);
                logger.Info(stock_close);
                

            }
        }
    }
}
