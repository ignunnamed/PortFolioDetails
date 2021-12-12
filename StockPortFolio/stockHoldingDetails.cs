
using Excel.FinancialFunctions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockPortFolio
{

    public class stockHoldingDetails
    {
        private double xirr;
        private bool isPortFolioBuild;
        List<StockTradeBookDetail> TradeBook;
        public string UserName { get; private set; }
        public double RealizeProfitOrLossAmount { get; private set; }
        public double Xirr
        {
            get
            {
                if (isPortFolioBuild == false)
                    BuildPortFolio();
                if (xirr == 0)
                {
                    xirr = CalculatePortFolioXirr();
                    return xirr;
                }
                else
                    return xirr;
            }
        }

        public stockHoldingDetails(string Name, List<StockTradeBookDetail> tradeBook)
        {
            UserName = Name;
            TradeBook = tradeBook;
        }

        public void BuildPortFolio()
        {
            var groupStockOnDate = TradeBook.GroupBy(x => x.StockTradeDate);
            foreach (var item in groupStockOnDate)
            {
                var groupStockOnSymbol = item.GroupBy(x => x.StockSymbol);

                foreach (var item1 in groupStockOnSymbol)
                {
                    double totalStockSold = 0;
                    double totalStockBought = 0;
                    double totalbuyPrice = 0;
                    double totalsellPrice = 0;
                    foreach (var item2 in item1)
                    {
                        if (item2.StockTradeType.ToLower() == "buy")
                        {
                            totalStockBought += item2.StockQuantity;
                            totalbuyPrice += item2.StockQuantity * item2.StockPrice;
                        }
                        else
                        {
                            totalStockSold += item2.StockQuantity;
                            totalsellPrice += item2.StockQuantity * item2.StockPrice;
                        }
                    }
                    if (totalStockSold - totalStockBought > 0)
                    {
                        var avg = totalsellPrice / totalStockSold;
                        StockDetails portfolioModel = this.StockProtfolio.Find(x => x.StockSymbol == item1.Key);
                        if (portfolioModel != null)
                        {
                            portfolioModel.StockQuantity -= totalStockSold - totalStockBought;
                            var profitLoss = GetProfitLoss(portfolioModel.StockTradeBreakDown, totalStockSold - totalStockBought, totalsellPrice / totalStockSold);
                            this.RealizeProfitOrLossAmount += profitLoss;
                            portfolioModel.RealizeProfitLoss += profitLoss;
                            portfolioModel.AvgPrice = GetAveragePrice(portfolioModel.StockTradeBreakDown);
                            portfolioModel.ValuePair.Add(item.Key, (totalStockSold - totalStockBought) * totalsellPrice / totalStockSold);
                        }

                    }
                    else if (totalStockSold - totalStockBought < 0)
                    {
                        var avg = totalbuyPrice / totalStockBought;

                        TradeDetails holdingModel = new TradeDetails();
                        holdingModel.PurchaseDate = item.Key;
                        holdingModel.StockPrice = avg;
                        holdingModel.stockQuantity = totalStockBought - totalStockSold;

                        if (this.StockProtfolio.FindAll(x => x.StockSymbol == item1.Key).Count == 0)
                        {
                            StockDetails portfolioModel = new StockDetails();
                            portfolioModel.StockQuantity = totalStockBought - totalStockSold;
                            portfolioModel.StockSymbol = item1.Key;
                            portfolioModel.StockTradeBreakDown.Add(holdingModel);
                            portfolioModel.AvgPrice = GetAveragePrice(portfolioModel.StockTradeBreakDown);
                            this.StockProtfolio.Add(portfolioModel);
                            portfolioModel.ValuePair.Add(item.Key, (totalStockSold - totalStockBought) * totalbuyPrice / totalStockBought);
                        }
                        else
                        {
                            StockDetails portfolioModel = this.StockProtfolio.Find(x => x.StockSymbol == item1.Key);
                            portfolioModel.StockTradeBreakDown.Add(holdingModel);
                            portfolioModel.StockQuantity += totalStockBought - totalStockSold;
                            portfolioModel.AvgPrice = GetAveragePrice(portfolioModel.StockTradeBreakDown);
                            portfolioModel.ValuePair.Add(item.Key, (totalStockSold - totalStockBought) * totalbuyPrice / totalStockBought);

                        }

                    }
                }
            }
            MarketValueOfHolding();
            isPortFolioBuild = true;
        }
        public void MarketValueOfHolding()
        {
            foreach (var item in this.StockProtfolio)
            {
                item.currentMarketValue = MarketData.GetMarketData(item.StockSymbol) * item.StockQuantity;
                item.ValuePair.Add(DateTime.Now.Date, item.currentMarketValue);
            }
        }
        private double CalculatePortFolioXirr()
        {
            List<double> ValueList = new List<double>();
            List<DateTime> dateList = new List<DateTime>();
            foreach (var item in this.StockProtfolio)
            {
                foreach (var item1 in item.ValuePair)
                {
                    ValueList.Add(item1.Value);
                    dateList.Add(item1.Key);
                }
            }
            //     The internal rate of return for a schedule of cash flows that is not necessarily
            //     periodic ([learn more](http://office.microsoft.com/en-us/excel/HP052093411033.aspx))
            return Financial.XIrr(ValueList, dateList) * 100;
        }

        public List<StockDetails> StockProtfolio = new List<StockDetails>();


        private double GetAveragePrice(List<TradeDetails> holdingModels)
        {
            var currentValue = holdingModels.Sum(x => x.StockPrice * x.stockQuantity);
            var totalStock = holdingModels.Sum(x => x.stockQuantity);
            if (currentValue == 0)
                return 0;
            var avgPrice = currentValue / totalStock;
            return avgPrice;
        }

        private double GetProfitLoss(List<TradeDetails> holdingModels, double stockSold, double avgPrice)
        {
            holdingModels.Sort(new TradeDetailsComparer());
            double stockToSell = stockSold;
            double buyPrice = 0;
            foreach (var item in holdingModels)
            {
                if (stockToSell != 0)
                {
                    if (stockToSell >= item.stockQuantity)
                    {
                        stockToSell -= item.stockQuantity;
                        buyPrice += item.stockQuantity * item.StockPrice;
                        item.isSold = true;
                    }
                    else
                    {
                        buyPrice += stockToSell * item.StockPrice;
                        item.stockQuantity -= stockToSell;
                        stockToSell = 0;
                    }
                }
            }
            holdingModels.RemoveAll(x => x.isSold == true);
            return stockSold * avgPrice - buyPrice;
        }

    }
}
