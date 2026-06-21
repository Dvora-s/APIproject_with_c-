using Library.Models.Entities;

namespace Library.DataAccess.Interfaces
{
    /// <summary>
    /// Repository ייעודי ל-Book, מוסיף פעולות שדורשות Include של ישויות קשורות
    /// (Category, Authors) שה-Generic Repository לא מטפל בהן.
    /// </summary>
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<Book?> GetByIdWithDetailsAsync(int bookId);
        Task<IEnumerable<Book>> GetAllWithDetailsAsync();
        Task<bool> IsbnExistsAsync(string isbn, int? excludeBookId = null);
    }
}
