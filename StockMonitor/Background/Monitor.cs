using System;
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
        public Monitor(IHubContext<PushHub> hubContext, TransactionContext transactionContext)
        {
            _hubContext = hubContext;
            _transactionContext = transactionContext;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var transaction = await _transactionContext.Transactions.SingleOrDefaultAsync(m => m.Id == 1);
                await _hubContext.Clients.All.SendAsync("BroadcastMessage", "Transaction: ", $"{transaction.StockId}");

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
