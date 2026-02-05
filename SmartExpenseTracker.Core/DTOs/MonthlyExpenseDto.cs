namespace SmartExpenseTracker.Core.DTOs
{
    public class MonthlyExpenseDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Total { get; set; }
    }
}
