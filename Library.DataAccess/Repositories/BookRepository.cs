using Library.DataAccess.Data;
using Library.DataAccess.Interfaces;
using Library.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(LibraryDbContext context) : base(context)
        {
        }

        public async Task<Book?> GetByIdWithDetailsAsync(int bookId)
        {
            return await _dbSet
                .Include(b => b.Category)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookId == bookId);
        }

        public async Task<IEnumerable<Book>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(b => b.Category)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> IsbnExistsAsync(string isbn, int? excludeBookId = null)
        {
            return await _dbSet.AnyAsync(b => b.ISBN == isbn &&
                (!excludeBookId.HasValue || b.BookId != excludeBookId.Value));
        }
    }
}
