using System.ComponentModel.DataAnnotations;

namespace LibraryMS.Models
{
    public class Book
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }

        public int PageCount { get; set; } 

        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string? ImageUrl { get; set; }

        public int Stock { get; set; }

        [Required]
        [Range(0, 1000000, ErrorMessage = "Fiyat 0 ile 1,000,000 arasında olmalıdır.")]
        [Display(Name = "Fiyat")]
        public decimal Price { get; set; }

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public Microsoft.AspNetCore.Http.IFormFile? ImageUpload { get; set; }
    }
}