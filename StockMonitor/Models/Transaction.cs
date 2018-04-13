using System;
using System.ComponentModel.DataAnnotations;

namespace StockMonitor.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(8)]
        public string StockId { get; set; }
        public bool IsBuying { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        public float InitialPrice { get; set; }
        public float Rate { get; set; }
        public float ExpectedDays { get; set; }
    }
}
