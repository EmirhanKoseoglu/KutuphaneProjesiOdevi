using Microsoft.AspNetCore.Identity; // Kullanıcıyı tanımak için
using System.ComponentModel.DataAnnotations;

namespace LibraryMS.Models
{
    public class BorrowRecord
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public string UserId { get; set; } 
        public IdentityUser User { get; set; }

        public DateTime LoanDate { get; set; }

        public DateTime DueDate { get; set; } 

        public DateTime? ReturnDate { get; set; }
    }
}