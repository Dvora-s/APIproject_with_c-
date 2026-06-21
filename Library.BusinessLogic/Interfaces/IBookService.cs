using Library.BusinessLogic.DTOs;

namespace Library.BusinessLogic.Interfaces
{
    /// <summary>
    /// שירות הלוגיקה העסקית לניהול ספרים - CRUD מלא.
    /// </summary>
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto> GetBookByIdAsync(int id);
        Task<BookDto> CreateBookAsync(CreateBookDto dto);
        Task UpdateBookAsync(int id, UpdateBookDto dto);
        Task DeleteBookAsync(int id);
        Task<IEnumerable<BookDto>> SearchByGenreAsync(string? genre);
    }
}
