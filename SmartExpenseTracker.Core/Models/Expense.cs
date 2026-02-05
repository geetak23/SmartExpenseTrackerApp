using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartExpenseTracker.Core.Enums;

namespace SmartExpenseTracker.Core.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public required string MerchantName { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public ExpenseCategory Category { get; set; }
        public required string DocumentName { get; set; }
    }
}
