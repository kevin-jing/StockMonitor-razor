using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StockMonitor.Models;

namespace StockMonitor.Pages.Transactions
{
    public class IndexModel : PageModel
    {
        private readonly StockMonitor.Models.TransactionContext _context;

        public IndexModel(StockMonitor.Models.TransactionContext context)
        {
            _context = context;
        }

        public IList<Transaction> Transaction { get;set; }

        public async Task OnGetAsync()
        {
            Transaction = await _context.Transactions.ToListAsync();
        }
    }
}
