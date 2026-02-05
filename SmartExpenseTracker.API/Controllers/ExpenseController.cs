using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartExpenseTracker.Core.DTOs;
using SmartExpenseTracker.Infrastructure.Data.Entities;
using SmartExpenseTracker.Infrastructure.Repositories;
using SmartExpenseTracker.Infrastructure.Services;
namespace SmartExpenseTracker.API.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpenseController : ControllerBase
{
    private readonly BlobStorageService _blob;
    private readonly ReceiptAnalysisService _analyzer;
    private readonly ReceiptRepository _repo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly MyDbContext _dbContext;

    public ExpenseController(
       MyDbContext dbContext,
       BlobStorageService blob,
       ReceiptAnalysisService analyzer,
       ReceiptRepository repo,
       UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _blob = blob;
        _analyzer = analyzer;
        _repo = repo;
        _userManager = userManager;
    }

    /* [HttpPost("upload-receipt")]
     public async Task<IActionResult> UploadReceipt([FromForm] ReceiptUploadDto dto)
     {
         var file = dto.File;

         // Get logged-in user OR fallback to static user   
         var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "static@demo.com");

         if (user == null)
         {
             user = new ApplicationUser
             {
                 UserName = "staticuser",
                 Email = "static@demo.com"
             };
             _dbContext.Users.Add(user);
             await _dbContext.SaveChangesAsync();
         }

         // Upload file to Blob Storage
         string blobUrl;
         using (var uploadStream = file.OpenReadStream())
         {
             blobUrl = await _blob.UploadAsync(uploadStream, file.FileName);
         }

         // Analyze receipt (use a NEW stream)
         Receipt receipt;
         IEnumerable<ReceiptItem> items;

         using (var analyzeStream = file.OpenReadStream())
         {
             (receipt, items) = await _analyzer.AnalyzeAsync(analyzeStream);
         }

         // Populate Receipt entity
         receipt.Id = Guid.NewGuid();
         receipt.UserId = user.Id;
         //receipt.User = null;

         receipt.FileName = file.FileName;
         receipt.BlobUrl = blobUrl;

         receipt.TransactionDate = receipt.TransactionDate?.ToUniversalTime();

         receipt.CreatedAt = DateTime.UtcNow;

         // Add receipt items
         foreach (var item in items)
         {
             receipt.Items.Add(item);
         }

         // Save to DB
         await _repo.AddAsync(receipt);

         return Ok(receipt);
     }*/

    [HttpPost("upload-receipts")]
    public async Task<IActionResult> UploadReceipts([FromForm] List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
            return BadRequest("No files uploaded");

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == "static@demo.com");

        if (user == null)
            return Ok(new List<Receipt>()); // no user → no receipts saved

        var results = new List<Receipt>();

        foreach (var file in files)
        {
            // Upload to blob
            string blobUrl;
            using (var uploadStream = file.OpenReadStream())
            {
                blobUrl = await _blob.UploadAsync(uploadStream, file.FileName);
            }

            // Analyze receipt
            Receipt receipt;
            IEnumerable<ReceiptItem> items;
            using (var analyzeStream = file.OpenReadStream())
            {
                (receipt, items) = await _analyzer.AnalyzeAsync(analyzeStream);
            }

            // Populate receipt entity
            receipt.Id = Guid.NewGuid();
            receipt.UserId = user.Id;
            receipt.FileName = file.FileName;
            receipt.BlobUrl = blobUrl;
            receipt.TransactionDate = receipt.TransactionDate?.ToUniversalTime();
            receipt.CreatedAt = DateTime.UtcNow;

            // Add items
            foreach (var item in items)
                receipt.Items.Add(item);

            // Save to DB
            await _repo.AddAsync(receipt);
            results.Add(receipt);
        }

        return Ok(results);
    }



    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthly(DateTime start, DateTime end)
    {
        // Normalize incoming dates to UTC
        start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        end = DateTime.SpecifyKind(end, DateTimeKind.Utc);

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == "static@demo.com");

        if (user == null)
            return Ok(new List<MonthlyExpenseDto>());

        var data = await _dbContext.Receipts
            .Where(r =>
                r.UserId == user.Id &&
                r.TransactionDate >= start &&
                r.TransactionDate <= end)
            .GroupBy(r => new
            {
                r.TransactionDate!.Value.Year,
                r.TransactionDate!.Value.Month
            })
            .Select(g => new MonthlyExpenseDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Total = g.Sum(r => r.Items.Sum(i => i.UnitPrice ?? 0))
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        return Ok(data);
    }
    [HttpGet("by-category")]
    public async Task<IActionResult> GetByCategory(
    DateTime start,
    DateTime end)
    {
        // Normalize incoming dates to UTC
        start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        end = DateTime.SpecifyKind(end, DateTimeKind.Utc);

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == "static@demo.com");

        if (user == null)
            return Ok(new List<ExpenseCategorySummaryDto>());

        // Step 1: Aggregate by normalized item name
        var grouped = await _dbContext.Receipts
     .Where(r =>
         r.UserId == user.Id &&
         r.TransactionDate.HasValue &&
         r.TransactionDate.Value >= start &&
         r.TransactionDate.Value <= end)
     .SelectMany(r => r.Items)
     .Where(i => !string.IsNullOrWhiteSpace(i.ItemName))
     .GroupBy(i => i.ItemName!.Trim().ToLower())   // exact normalized key
     .Select(g => new ExpenseCategorySummaryDto
     {
         Category = g.Key,                          // already lowercase
         Total = g.Sum(x => x.UnitPrice ?? 0m)
     })
     .OrderByDescending(x => x.Total)
     .ToListAsync();



        // Step 2: Top 10 + Others
        var top20 = grouped.Take(20).ToList();

        var othersTotal = grouped
            .Skip(20)
            .Sum(x => x.Total);

        if (othersTotal > 0)
        {
            top20.Add(new ExpenseCategorySummaryDto
            {
                Category = "others",
                Total = othersTotal
            });
        }

        return Ok(top20);
    }
    
    [HttpGet("by-store")]
    public async Task<ActionResult<List<ExpenseByStoreDto>>> GetExpenseByStore(DateTime start,DateTime end)
    {
        // Normalize incoming dates to UTC
        start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        end = DateTime.SpecifyKind(end, DateTimeKind.Utc);

        if (start > end)
            return BadRequest("Start date must be before end date.");

        var result = await _dbContext.Receipts
            .Where(r =>
                r.TransactionDate.HasValue &&
                r.TransactionDate.Value.Date >= start.Date &&
                r.TransactionDate.Value.Date <= end.Date)
            .GroupBy(r => r.MerchantName ?? "Unknown")
            .Select(g => new ExpenseByStoreDto
            {
                Store = g.Key,
                Total = g.Sum(r => r.Items.Sum(i => i.UnitPrice ?? 0)),
                ReceiptCount = g.Count()
            })
            .OrderByDescending(x => x.Total)
            .ToListAsync();

        return Ok(result);
    }
}