
using StockPortFolio;
using System;
using System.Collections.Generic;
using System.IO;

namespace CsvReader
{
    public class Reader : IDisposable
    {
        string filePath;
        public Reader(string fileLocation)
        {
            filePath = fileLocation;
        }

        public List<StockTradeBookDetail> GetData()
        {
            List<StockTradeBookDetail> data = new List<StockTradeBookDetail>();
            using (var reader = new StreamReader(@filePath))
            {
                int rowCount = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    if (rowCount > 0) // To Skip first row
                    {
                        data.Add(new StockTradeBookDetail()
                        {
                            StockPrice = double.Parse(values[int.Parse(Resources.Price)]),
                            StockQuantity = double.Parse(values[int.Parse(Resources.Quantity)]),
                            StockSymbol = GetStockSymbolName(values[int.Parse(Resources.Symbol)]),
                            StockTradeDate = DateTime.Parse(values[int.Parse(Resources.TradeDate)]),
                            StockTradeType = values[int.Parse(Resources.TradeType)],
                        });
                    }
                    rowCount++;
                }
            }
            return data;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        private string GetStockSymbolName(string stockName)
        {
            if (stockName.Contains('-'))
            {
                return stockName.Substring(0, stockName.IndexOf('-'));
            }
            else return stockName;
        }

    }
}
