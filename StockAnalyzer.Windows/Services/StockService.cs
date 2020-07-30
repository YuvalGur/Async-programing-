using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StockAnalyzer.Core.Domain;

namespace StockAnalyzer.Windows.Services
{
    public interface IStockService
    {
        Task<IEnumerable<StockPrice>> GetStockPricesFor(string ticker, CancellationToken cancellationToken);
    }
    public class StockService:IStockService
    {
        public async Task<IEnumerable<StockPrice>> GetStockPricesFor(string ticker,CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                ticker = ticker.ToUpper();
                var result = await client.GetAsync($"http://localhost:61363/api/stocks/{ticker}", cancellationToken);

                result.EnsureSuccessStatusCode();

                var content = await result.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<IEnumerable<StockPrice>>(content);
            }
        }
    }

    public class MockStockService : IStockService
    {
        public Task<IEnumerable<StockPrice>> GetStockPricesFor(string ticker, CancellationToken cancellationToken)
        {
            var stocks = new List<StockPrice>
            {
                new StockPrice{Ticker= "MSFT",Change = 0.5m,ChangePercent = 0.75m},
                new StockPrice{Ticker= "MSFT",Change = 0.2m,ChangePercent = 0.14m},
                new StockPrice{Ticker= "GOOGL",Change = 0.4m,ChangePercent = 0.61m},
                new StockPrice{Ticker= "GOOGL",Change = 0.8m,ChangePercent = 0.35m}
            };
            return Task.FromResult(stocks.Where(stock => stock.Ticker == ticker));
        }
    }
}
