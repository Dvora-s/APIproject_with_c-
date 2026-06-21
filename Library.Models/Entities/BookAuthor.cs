namespace Library.Models.Entities
{
    /// <summary>
    /// טבלת קשר (Join Table) למימוש Many-to-Many בין Book לבין Author.
    /// ספר יכול להיות עם כמה מחברים, ומחבר יכול לכתוב כמה ספרים.
    /// </summary>
    public class BookAuthor
    {
        public int BookId { get; set; }
        public Book? Book { get; set; }

        public int AuthorId { get; set; }
        public Author? Author { get; set; }
    }
}
