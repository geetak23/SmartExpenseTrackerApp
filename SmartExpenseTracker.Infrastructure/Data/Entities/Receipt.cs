using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartExpenseTracker.Infrastructure.Data.Entities
{
    public class Receipt
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string FileName { get; set; }
        public string BlobUrl { get; set; }
        public string MerchantName { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? DiscountedAmount { get; set; }
        public decimal? UndiscountedTotalAmount { get; set; }
        

        public decimal? TotalAmount { get; set; }
        public decimal? Tax { get; set; }

        public ICollection<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
