using AutoMapper;
using Library.BusinessLogic.DTOs;
using Library.BusinessLogic.Exceptions;
using Library.BusinessLogic.Interfaces;
using Library.DataAccess.Interfaces;
using Library.Models.Entities;
using Microsoft.Extensions.Logging;

namespace Library.BusinessLogic.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BookService> _logger;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BookService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _unitOfWork.Books.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<BookDto> GetBookByIdAsync(int id)
        {
            var book = await _unitOfWork.Books.GetByIdWithDetailsAsync(id);
            if (book == null)
            {
                _logger.LogWarning("ספר עם מזהה {BookId} לא נמצא", id);
                throw new NotFoundException(nameof(Book), id);
            }

            return _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto dto)
        {
            // חוק עסקי: ISBN חייב להיות ייחודי
            if (await _unitOfWork.Books.IsbnExistsAsync(dto.ISBN))
            {
                throw new BusinessRuleException($"ספר עם ISBN '{dto.ISBN}' כבר קיים במערכת.");
            }

            // חוק עסקי: הקטגוריה חייבת להתקיים
            var categoryExists = await _unitOfWork.Categories.ExistsAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
            {
                throw new BusinessRuleException($"קטגוריה עם מזהה '{dto.CategoryId}' אינה קיימת.");
            }

            var book = _mapper.Map<Book>(dto);

            // קישור מחברים (Many-to-Many) אם סופקו
            if (dto.AuthorIds.Any())
            {
                foreach (var authorId in dto.AuthorIds.Distinct())
                {
                    var authorExists = await _unitOfWork.Authors.ExistsAsync(a => a.AuthorId == authorId);
                    if (!authorExists)
                    {
                        throw new BusinessRuleException($"מחבר עם מזהה '{authorId}' אינו קיים.");
                    }
                    book.BookAuthors.Add(new BookAuthor { AuthorId = authorId });
                }
            }

            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("נוצר ספר חדש: {Title} (מזהה {BookId})", book.Title, book.BookId);

            var createdBook = await _unitOfWork.Books.GetByIdWithDetailsAsync(book.BookId);
            return _mapper.Map<BookDto>(createdBook);
        }

        public async Task UpdateBookAsync(int id, UpdateBookDto dto)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null)
            {
                throw new NotFoundException(nameof(Book), id);
            }

            if (await _unitOfWork.Books.IsbnExistsAsync(dto.ISBN, excludeBookId: id))
            {
                throw new BusinessRuleException($"ספר אחר עם ISBN '{dto.ISBN}' כבר קיים במערכת.");
            }

            var categoryExists = await _unitOfWork.Categories.ExistsAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
            {
                throw new BusinessRuleException($"קטגוריה עם מזהה '{dto.CategoryId}' אינה קיימת.");
            }

            var loanedCopies = book.CopiesTotal - book.CopiesAvailable;
            if (dto.CopiesTotal < loanedCopies)
            {
                throw new BusinessRuleException(
                    $"לא ניתן להקטין את מספר העותקים מתחת ל-{loanedCopies} (מספר העותקים המושאלים כרגע).");
            }

            book.Title = dto.Title;
            book.ISBN = dto.ISBN;
            book.CopiesTotal = dto.CopiesTotal;
            book.CopiesAvailable = dto.CopiesTotal - loanedCopies;
            book.PublicationYear = dto.PublicationYear;
            book.Genre = dto.Genre;
            book.CategoryId = dto.CategoryId;

            _unitOfWork.Books.Update(book);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("עודכן ספר {BookId}", id);
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null)
            {
                throw new NotFoundException(nameof(Book), id);
            }

            // חוק עסקי: אי אפשר למחוק ספר שיש לו השאלות פתוחות
            var hasActiveLoans = await _unitOfWork.Loans.ExistsAsync(l => l.BookId == id && l.ReturnDate == null);
            if (hasActiveLoans)
            {
                throw new BusinessRuleException("לא ניתן למחוק ספר עם השאלות פעילות.");
            }

            _unitOfWork.Books.Remove(book);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("נמחק ספר {BookId}", id);
        }

        public async Task<IEnumerable<BookDto>> SearchByGenreAsync(string? genre)
        {
            var books = await GetAllBooksAsync();
            if (string.IsNullOrWhiteSpace(genre))
            {
                return books;
            }

            return books.Where(b => b.Genre != null &&
                b.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase));
        }
    }
}
