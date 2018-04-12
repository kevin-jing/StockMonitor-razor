using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockMonitor.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public int StockId { get; set; }
        public bool IsBuying { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        public float CurrentPrice { get; set; }
        public float ExpectedDays { get; set; }
    }
}
