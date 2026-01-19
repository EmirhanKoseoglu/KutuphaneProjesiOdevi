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
    }
}