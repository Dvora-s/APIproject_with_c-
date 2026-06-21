using System.ComponentModel.DataAnnotations;

namespace Library.Models.Entities
{
    /// <summary>
    /// מייצג סופר/ת. קשר Many-to-Many מול ספרים דרך BookAuthor.
    /// </summary>
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        [MaxLength(100)]
        public string? Nationality { get; set; }

        // Navigation property - Many-to-Many דרך טבלת קשר
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}
