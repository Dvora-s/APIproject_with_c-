using Library.DataAccess.Data;
using Library.DataAccess.Interfaces;
using Library.Models.Entities;

namespace Library.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryDbContext _context;

        private IBookRepository? _books;
        private IMemberRepository? _members;
        private IGenericRepository<Author>? _authors;
        private IGenericRepository<Category>? _categories;
        private IGenericRepository<Loan>? _loans;
        private IGenericRepository<BookAuthor>? _bookAuthors;

        public UnitOfWork(LibraryDbContext context)
        {
            _context = context;
        }

        public IBookRepository Books => _books ??= new BookRepository(_context);
        public IMemberRepository Members => _members ??= new MemberRepository(_context);
        public IGenericRepository<Author> Authors => _authors ??= new GenericRepository<Author>(_context);
        public IGenericRepository<Category> Categories => _categories ??= new GenericRepository<Category>(_context);
        public IGenericRepository<Loan> Loans => _loans ??= new GenericRepository<Loan>(_context);
        public IGenericRepository<BookAuthor> BookAuthors => _bookAuthors ??= new GenericRepository<BookAuthor>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
