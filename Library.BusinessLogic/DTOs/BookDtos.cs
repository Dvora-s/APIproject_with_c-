using System.ComponentModel.DataAnnotations;

namespace Library.BusinessLogic.DTOs
{
    /// <summary>DTO להצגת ספר ללקוח (כולל פרטי קטגוריה ומחברים).</summary>
    public class BookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int CopiesTotal { get; set; }
        public int CopiesAvailable { get; set; }
        public int PublicationYear { get; set; }
        public string? Genre { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<string> AuthorNames { get; set; } = new();
    }

    /// <summary>DTO ליצירת ספר חדש - כולל Validation.</summary>
    public class CreateBookDto
    {
        [Required(ErrorMessage = "כותרת הספר היא שדה חובה")]
        [MaxLength(200, ErrorMessage = "כותרת הספר לא יכולה לעלות על 200 תווים")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN הוא שדה חובה")]
        [MaxLength(20)]
        public string ISBN { get; set; } = string.Empty;

        [Range(1, 9999, ErrorMessage = "מספר עותקים חייב להיות בין 1 ל-9999")]
        public int CopiesTotal { get; set; }

        [Range(1000, 2100, ErrorMessage = "שנת הוצאה לא תקינה")]
        public int PublicationYear { get; set; }

        [MaxLength(100)]
        public string? Genre { get; set; }

        [Required(ErrorMessage = "יש לבחור קטגוריה")]
        public int CategoryId { get; set; }

        public List<int> AuthorIds { get; set; } = new();
    }

    /// <summary>DTO לעדכון ספר קיים.</summary>
    public class UpdateBookDto
    {
        [Required(ErrorMessage = "כותרת הספר היא שדה חובה")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN הוא שדה חובה")]
        [MaxLength(20)]
        public string ISBN { get; set; } = string.Empty;

        [Range(0, 9999)]
        public int CopiesTotal { get; set; }

        [Range(1000, 2100)]
        public int PublicationYear { get; set; }

        [MaxLength(100)]
        public string? Genre { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public List<int> AuthorIds { get; set; } = new();
    }
}
