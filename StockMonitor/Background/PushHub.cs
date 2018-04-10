using Microsoft.AspNetCore.SignalR;

namespace StockMonitor.Background
{
    public class PushHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.SendAsync("BroadcastMessage", name, message);
        }
    }
}
