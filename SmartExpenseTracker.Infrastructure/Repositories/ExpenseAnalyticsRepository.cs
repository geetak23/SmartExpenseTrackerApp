using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SmartExpenseTracker.Core.DTOs;
using SmartExpenseTracker.Core.Interfaces;
using SmartExpenseTracker.Infrastructure.Data;

namespace SmartExpenseTracker.Infrastructure.Repositories
{

    public class ExpenseAnalyticsRepository : IExpenseAnalyticsRepository
    {
        private readonly AppDbContext _context;

        public ExpenseAnalyticsRepository(AppDbContext context)
        {
            _context = context;
        }

        // 1️⃣ Monthly Expense Trend
        public async Task<List<MonthlyExpenseDto>> GetMonthlyExpenses(
            DateTime start, DateTime end)
        {
            var sql = """
            SELECT
                EXTRACT(YEAR FROM transaction_date) AS "Year",
                EXTRACT(MONTH FROM transaction_date) AS "Month",
                SUM(total_amount) AS "Total"
            FROM "Receipts"
            WHERE transaction_date BETWEEN @start AND @end
            GROUP BY "Year", "Month"
            ORDER BY "Year", "Month";
        """;

            return await _context.Set<MonthlyExpenseDto>()
                .FromSqlRaw(sql,
                    new NpgsqlParameter("start", start),
                    new NpgsqlParameter("end", end))
                .AsNoTracking()
                .ToListAsync();
        }

        // 2️⃣ Expense by Store
        public async Task<List<ExpenseByStoreDto>> GetExpenseByStore(
            DateTime start, DateTime end)
        {
            var sql = """
            SELECT
                store_name AS "StoreName",
                SUM(total_amount) AS "Total"
            FROM "Receipts"
            WHERE transaction_date BETWEEN @start AND @end
            GROUP BY store_name
            ORDER BY "Total" DESC;
        """;

            return await _context.Set<ExpenseByStoreDto>()
                .FromSqlRaw(sql,
                    new NpgsqlParameter("start", start),
                    new NpgsqlParameter("end", end))
                .AsNoTracking()
                .ToListAsync();
        }

        // 3️⃣ Expense by Category (Monthly)
        public async Task<List<ExpenseCategorySummaryDto>> GetExpenseByCategory(
            int year, int month)
        {
            var sql = """
            SELECT
                i.category AS "Category",
                SUM(i.total_price) AS "Total"
            FROM "ReceiptItems" i
            JOIN "Receipts" r ON r.id = i.receipt_id
            WHERE EXTRACT(YEAR FROM r.transaction_date) = @year
              AND EXTRACT(MONTH FROM r.transaction_date) = @month
            GROUP BY i.category;
        """;

            return await _context.Set<ExpenseCategorySummaryDto>()
                .FromSqlRaw(sql,
                    new NpgsqlParameter("year", year),
                    new NpgsqlParameter("month", month))
                .AsNoTracking()
                .ToListAsync();
        }

        // 4️⃣ Category Trend Over Time
        public async Task<List<CategoryTrendDto>> GetCategoryTrend(
            DateTime start, DateTime end)
        {
            var sql = """
            SELECT
                EXTRACT(YEAR FROM r.transaction_date) AS "Year",
                EXTRACT(MONTH FROM r.transaction_date) AS "Month",
                i.category AS "Category",
                SUM(i.total_price) AS "Total"
            FROM "Receipts" r
            JOIN "ReceiptItems" i ON r.id = i.receipt_id
            WHERE r.transaction_date BETWEEN @start AND @end
            GROUP BY "Year", "Month", i.category
            ORDER BY "Year", "Month";
        """;

            return await _context.Set<CategoryTrendDto>()
                .FromSqlRaw(sql,
                    new NpgsqlParameter("start", start),
                    new NpgsqlParameter("end", end))
                .AsNoTracking()
                .ToListAsync();
        }

        // 6️⃣ Receipt Size Distribution
        public async Task<List<ReceiptBucketDto>> GetReceiptSizeDistribution()
        {
            var sql = """
            SELECT
                CASE
                    WHEN total_amount < 20 THEN '0-20'
                    WHEN total_amount < 50 THEN '20-50'
                    WHEN total_amount < 100 THEN '50-100'
                    ELSE '100+'
                END AS "Range",
                COUNT(*) AS "Count"
            FROM "Receipts"
            GROUP BY "Range";
        """;

            return await _context.Set<ReceiptBucketDto>()
                .FromSqlRaw(sql)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
