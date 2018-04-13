using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using StockMonitor.Models;

namespace StockMonitor.Background
{
    public class Monitor : BackgroundService
    {
        private readonly IHubContext<PushHub> _hubContext;
        private readonly TransactionContext _transactionContext;
        private HttpClient _httpClient;
        public Monitor(IHubContext<PushHub> hubContext, TransactionContext transactionContext)
        {
            _hubContext = hubContext;
            _transactionContext = transactionContext;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://hq.sinajs.cn/");
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                var transactions = await _transactionContext.Transactions.ToListAsync<Transaction>();
                float deviationMinimum = 100000;
                string stockName = "";
                float dayss = -1;
                float orderPrice = -1;
                float c = -1;
                
                foreach (var transaction in transactions)
                {
                    //string requestUri = $"list={transaction.StockId}";
                    string requestUri = $"list=s_{transaction.StockId}";

                    HttpResponseMessage resp = await _httpClient.GetAsync(requestUri);
                    var content = await resp.Content.ReadAsStringAsync();
                    // var hq_str_s_sh603577="汇金通,13.650,-0.130,-0.94,47190,6420";
                    // 指数名称，当前点数，当前价格，涨跌率，成交量（手），成交额（万元）

                    var infos = content.Substring(23).Split(','); // 汇金通,13.650,-0.130,-0.94,47190,6420";
                    var name = infos[0];
                    var currentPrice = float.Parse(infos[1]);

                    float deviation;
                    var initialPrice = transaction.InitialPrice;
                    var rate = transaction.Rate;
                    var expectedDays = transaction.ExpectedDays;
                    var days = (float)((now - transaction.StartDate).TotalDays);
                    float desiredPrice = -1;
                    if (transaction.IsBuying)
                    {
                        desiredPrice = initialPrice * (1 + rate * (days / expectedDays - 1) / 100);
                        deviation = (currentPrice - desiredPrice) / initialPrice * 100;
                    }
                    else
                    {
                        desiredPrice = initialPrice * (1 + rate * (1 - days / expectedDays) / 100);
                        deviation = (desiredPrice - currentPrice) / initialPrice * 100;
                    }

                    if (deviation < deviationMinimum)
                    {
                        deviationMinimum =  deviation;
                        stockName = name;
                        dayss = days;
                        orderPrice = desiredPrice;
                        c = currentPrice;
                    }
                    
                }


                await _hubContext.Clients.All.SendAsync("BroadcastMessage", "Transaction: ", $"{stockName},{dayss},{orderPrice},{c},{deviationMinimum}");
                
                //await _hubContext.Clients.All.SendAsync("BroadcastMessage", "Transaction: ", "123");
                await Task.Delay(TimeSpan.FromSeconds(4), cancellationToken);
            }
        }
    }
}
