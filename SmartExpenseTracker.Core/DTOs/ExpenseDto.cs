namespace SmartExpenseTracker.Core.DTOs;

public class ExpenseDto
{
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
}