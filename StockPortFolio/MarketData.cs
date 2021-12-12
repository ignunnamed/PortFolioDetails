using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StockPortFolio
{
    public class MarketData
    {
        public static double GetMarketData(string stockSymbol)
        {

            //    WebRequest request = WebRequest.Create(string.Format("http://www.nseindia.com/api/quote-equity?symbol={0}", stockSymbol));
            //  request.Headers.Add("user-agent", @"Mozilla/5.0 (iPad; U; CPU OS 3_2 like Mac OS X; en-us) AppleWebKit/531.21.10 (KHTML, like Gecko) Version/4.0.4 Mobile/7B334b Safari/531.21.10");
            //WebResponse response = request.GetResponse();
            using (WebClient client = new WebClient())
            {
                // oClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:30.0) Gecko/20100101 Firefox/30.0");

                // var response = new WebClient().DownloadString("https://www.nseindia.com/api/quote-equity?symbol=ICICIBANK&section=trade_info");
                string url = string.Format("https://query1.finance.yahoo.com/v8/finance/chart/{0}.NS?symbol={0}.NS&range=1d&useYfid=true&interval=60m&includePrePost=true&events=div%7Csplit%7Cearn&lang=en-US&region=US&crumb=H.D9hubFl7k&corsDomain=finance.yahoo.com", stockSymbol);
                // Add a user agent header in case the
                // requested URI contains a query.
                // client.BaseAddress = "https://www.nseindia.com/api/quote-equity?symbol=ICICIBANK&section=trade_info";
                client.Headers.Add("user-agent", @"Mozilla/5.0 (iPad; U; CPU OS 3_2 like Mac OS X; en-us) AppleWebKit/531.21.10 (KHTML, like Gecko) Version/4.0.4 Mobile/7B334b Safari/531.21.10");
                try
                {
                    client.UseDefaultCredentials = true;
                    //Stream data = client.OpenRead(new Uri(string.Format("http://www.nseindia.com/api/quote-equity?symbol={0}", stockSymbol)));
                    Stream data = client.OpenRead(new Uri(url));

                    StreamReader reader = new StreamReader(data);
                    string s = reader.ReadToEnd();
                    var json = JsonConvert.DeserializeObject(s);
                    var sss = JObject.Parse(json.ToString());
                    var value = sss.SelectToken("chart.result[0].meta.regularMarketPrice");
                    reader.Close();
                    data.Close();

                    return double.Parse(value.ToString());
                }
                catch (Exception ex)
                {
                    if (stockSymbol.Contains('-'))
                    {
                        var ss = stockSymbol.Substring(0, stockSymbol.IndexOf('-'));
                        return GetMarketData(ss);
                    }

                    //  return GetMarketData(stockSymbol);
                    return 0;
                }
            }
        }
    }
}
