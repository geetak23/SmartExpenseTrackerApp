using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartExpenseTracker.Core.Models;

namespace SmartExpenseTracker.Core.Interfaces
{
    public interface IDocumentAnalyzer
    {
        Task<Expense> AnalyzeReceiptAsync(Stream fileStream, string fileName);
    }
}
