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

                string requestUri = $"list={transaction.StockId}";

                HttpResponseMessage resp = await _httpClient.GetAsync(requestUri);
                var content = await resp.Content.ReadAsStringAsync();

                await _hubContext.Clients.All.SendAsync("BroadcastMessage", "Transaction: ", $"{content}");

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
