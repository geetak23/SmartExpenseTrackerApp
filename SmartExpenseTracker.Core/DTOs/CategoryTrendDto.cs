namespace SmartExpenseTracker.Core.DTOs;

public class CategoryTrendDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string Category { get; set; } = "";
    public decimal Total { get; set; }
}
