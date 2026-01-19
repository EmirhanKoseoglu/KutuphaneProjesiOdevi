using System.ComponentModel.DataAnnotations;

namespace LibraryMS.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Yazar adı boş bırakılamaz.")]
        public string Name { get; set; } 
    }
}