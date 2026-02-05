using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartExpenseTracker.Infrastructure.Data.Entities
{
    public class ReceiptItem
    {
        public Guid Id { get; set; }

        public Guid ReceiptId { get; set; }
        
        [JsonIgnore] // 🔥 Prevents the cycle
        public Receipt Receipt { get; set; }

        public string ItemName { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
