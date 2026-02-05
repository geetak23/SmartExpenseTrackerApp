namespace SmartExpenseTracker.Core.DTOs;

public class ExpenseByStoreDto
{
    public string Store { get; set; } = default!;
    public decimal Total { get; set; }
    public int ReceiptCount { get; set; }

}

