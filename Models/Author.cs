using System.ComponentModel.DataAnnotations;

namespace LibraryMS.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Yazar adı boş bırakılamaz.")]
        public string Name { get; set; } 

        public string? Bio { get; set; }

        public string? ImageUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public Microsoft.AspNetCore.Http.IFormFile? ImageUpload { get; set; }
    }
}