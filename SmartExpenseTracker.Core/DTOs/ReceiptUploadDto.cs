using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

namespace SmartExpenseTracker.Core.DTOs
{
    public class ReceiptUploadDto
    {
        [Required]
        public IFormFile File { get; set; }
    }

}
