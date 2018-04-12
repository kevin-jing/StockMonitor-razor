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
                var transaction = await _transactionContext.Transactions.SingleOrDefaultAsync(m => m.Id == 1);

                //string requestUri = $"list={transaction.StockId}";
                string requestUri = $"list=s_{transaction.StockId}";

                HttpResponseMessage resp = await _httpClient.GetAsync(requestUri);
                var content = await resp.Content.ReadAsStringAsync();
                // var hq_str_s_sh603577="汇金通,13.650,-0.130,-0.94,47190,6420";
                // 指数名称，当前点数，当前价格，涨跌率，成交量（手），成交额（万元）

                var infos = content.Substring(23).Split(','); // 汇金通,13.650,-0.130,-0.94,47190,6420";
                var name = infos[0];
                var currentPrice = float.Parse(infos[1]);

                await _hubContext.Clients.All.SendAsync("BroadcastMessage", "Transaction: ", $"{name},{currentPrice}");


                //await _hubContext.Clients.All.SendAsync("BroadcastMessage", "Transaction: ", "123");
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
