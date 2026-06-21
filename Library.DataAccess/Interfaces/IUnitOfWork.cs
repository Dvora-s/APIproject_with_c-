using Library.Models.Entities;

namespace Library.DataAccess.Interfaces
{
    /// <summary>
    /// Unit of Work - מרכז גישה לכל ה-Repositories ושומר על SaveChanges אחד מרוכז לכל פעולה עסקית.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IBookRepository Books { get; }
        IMemberRepository Members { get; }
        IGenericRepository<Author> Authors { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Loan> Loans { get; }
        IGenericRepository<BookAuthor> BookAuthors { get; }

        Task<int> SaveChangesAsync();
    }
}
