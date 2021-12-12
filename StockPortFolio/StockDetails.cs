using Excel.FinancialFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPortFolio
{
    public class StockDetails
    {
        private double xirr;
        public string StockSymbol { get; set; }
        public double StockQuantity { get; set; }

        public List<TradeDetails> StockTradeBreakDown = new List<TradeDetails>();
        public double AvgPrice { get; set; }
        public double RealizeProfitLoss { get; set; }
        public double UnRealizeProfitLoss { get { return currentMarketValue -( StockQuantity * AvgPrice); } }
        public double currentMarketValue { get; internal set; }

        internal Dictionary<DateTime, double> ValuePair = new Dictionary<DateTime, double>();
        public double XIRR
        {
            get
            {
                if (xirr != 0)
                    return xirr;
                else
                {
                    xirr = CalculateXirr();
                    return xirr;
                }
            }
        }
        private double CalculateXirr()
        {
            List<double> ValueList = new List<double>();
            List<DateTime> dateList = new List<DateTime>();
            foreach (var item in ValuePair)
            {
                ValueList.Add(item.Value);
                dateList.Add(item.Key);
            }
            try
            {
                return Financial.XIrr(ValueList, dateList) * 100;
            }
            catch (Exception)
            {

                return 0;
            }

        }
    }

    public class TradeDetails
    {
        public DateTime PurchaseDate;
        public double stockQuantity;
        public double StockPrice;
        public bool isSold;
    }

    public class TradeDetailsComparer : IComparer<TradeDetails>
    {
        public TradeDetailsComparer()
        {

        }
        public int Compare(TradeDetails? x, TradeDetails? y)
        {
            return DateTime.Compare(x.PurchaseDate, y.PurchaseDate);
        }
    }
}
