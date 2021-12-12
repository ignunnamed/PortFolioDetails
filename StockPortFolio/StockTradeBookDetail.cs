using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPortFolio
{
    public class StockTradeBookDetail
    {
        public string StockSymbol { get; set; }
        public DateTime StockTradeDate { get; set; }
        public string StockTradeType { get; set; }
        public double StockQuantity { get; set; }
        public double StockPrice { get; set; }
    }
}
