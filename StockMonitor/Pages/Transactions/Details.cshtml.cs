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
    public class DetailsModel : PageModel
    {
        private readonly StockMonitor.Models.TransactionContext _context;

        public DetailsModel(StockMonitor.Models.TransactionContext context)
        {
            _context = context;
        }

        public Transaction Transaction { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Transaction = await _context.Transactions.SingleOrDefaultAsync(m => m.Id == id);

            if (Transaction == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
