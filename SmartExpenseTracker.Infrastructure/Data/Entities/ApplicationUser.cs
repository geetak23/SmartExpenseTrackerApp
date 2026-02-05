using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SmartExpenseTracker.Infrastructure.Data.Entities
{
    [Table("AspNetUsers")]
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserName { get; set; }
        public string Email { get; set; }

        // Navigation property
        public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
    }
}
