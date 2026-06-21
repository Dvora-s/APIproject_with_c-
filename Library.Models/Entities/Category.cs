using System.ComponentModel.DataAnnotations;

namespace Library.Models.Entities
{
    /// <summary>
    /// קטגוריית ספרים (לדוגמה: מדע בדיוני, היסטוריה, רומן).
    /// קשר Many-to-One מול Book (הרבה ספרים -> קטגוריה אחת).
    /// </summary>
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
