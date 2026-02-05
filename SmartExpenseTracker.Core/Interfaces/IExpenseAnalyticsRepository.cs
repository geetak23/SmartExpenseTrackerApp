using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartExpenseTracker.Core.DTOs;

namespace SmartExpenseTracker.Infrastructure.Repositories
{
    public interface IExpenseAnalyticsRepository
    {
        Task<List<MonthlyExpenseDto>> GetMonthlyExpenses(DateTime start, DateTime end);
        Task<List<ExpenseByStoreDto>> GetExpenseByStore(DateTime start, DateTime end);
        Task<List<ExpenseCategorySummaryDto>> GetExpenseByCategory(int year, int month);
        Task<List<CategoryTrendDto>> GetCategoryTrend(DateTime start, DateTime end);
        Task<List<ReceiptBucketDto>> GetReceiptSizeDistribution();
    }
}
