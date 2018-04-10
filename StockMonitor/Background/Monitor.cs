using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace StockMonitor.Background
{
    public class Monitor : BackgroundService
    {
        private readonly IHubContext<PushHub> _hubContext;
        public Monitor(IHubContext<PushHub> hubContext)
        {
            _hubContext = hubContext;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _hubContext.Clients.All.SendAsync("BroadcastMessage", "server", "123");

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
