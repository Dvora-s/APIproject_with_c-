using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models.Entities
{
    /// <summary>
    /// מייצג ספר במערכת הספרייה.
    /// </summary>
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ISBN { get; set; } = string.Empty;

        [Range(1, 9999)]
        public int CopiesTotal { get; set; }

        [Range(0, 9999)]
        public int CopiesAvailable { get; set; }

        public int PublicationYear { get; set; }

        [MaxLength(100)]
        public string? Genre { get; set; }

        // Many-to-One: ספר רבים שייכים לקטגוריה אחת (לדוגמה)
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        // Many-to-Many דרך טבלת קשר BookAuthor
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

        // One-to-Many: ספר אחד יכול להיות מושאל פעמים רבות לאורך זמן
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
