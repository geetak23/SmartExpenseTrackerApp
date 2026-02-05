namespace SmartExpenseTracker.Core.DTOs
{
    public class ReceiptDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public List<ReceiptItemDto> Items { get; set; }
    }
}
