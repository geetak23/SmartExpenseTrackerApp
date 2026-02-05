using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartExpenseTracker.Core.Models;
using Microsoft.EntityFrameworkCore; 

namespace SmartExpenseTracker.Infrastructure.Data
{
    public class ExpenseDbContext : DbContext
    {
        public DbSet<Expense> Expenses => Set<Expense>();

        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options)
            : base(options) { }
    }
}
