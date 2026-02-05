using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartExpenseTracker.Infrastructure.Data;
using SmartExpenseTracker.Infrastructure.Data.Entities;

namespace SmartExpenseTracker.Infrastructure.Repositories
{
    public class ReceiptRepository
    {
        private readonly AppDbContext _db;

        public ReceiptRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Receipt receipt)
        {
            _db.Receipts.Add(receipt);
            await _db.SaveChangesAsync();
        }
    }
}
