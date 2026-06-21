using AutoMapper;
using Library.BusinessLogic.DTOs;
using Library.BusinessLogic.Exceptions;
using Library.BusinessLogic.Interfaces;
using Library.DataAccess.Interfaces;
using Library.Models.Entities;
using Microsoft.Extensions.Logging;

namespace Library.BusinessLogic.Services
{
    public class LoanService : ILoanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanService> _logger;

        public LoanService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<LoanService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<LoanDto>> GetAllLoansAsync()
        {
            var loans = await _unitOfWork.Loans.GetAllAsync(nameof(Loan.Book), nameof(Loan.Member));
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        public async Task<LoanDto> GetLoanByIdAsync(int id)
        {
            var loans = await _unitOfWork.Loans.GetAllAsync(nameof(Loan.Book), nameof(Loan.Member));
            var loan = loans.FirstOrDefault(l => l.LoanId == id);
            if (loan == null)
            {
                throw new NotFoundException(nameof(Loan), id);
            }

            return _mapper.Map<LoanDto>(loan);
        }

        public async Task<LoanDto> CreateLoanAsync(CreateLoanDto dto)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(dto.BookId);
            if (book == null)
            {
                throw new NotFoundException(nameof(Book), dto.BookId);
            }

            var member = await _unitOfWork.Members.GetByIdAsync(dto.MemberId);
            if (member == null)
            {
                throw new NotFoundException(nameof(Member), dto.MemberId);
            }

            // חוק עסקי: חבר לא פעיל לא יכול להשאיל
            if (!member.IsActive)
            {
                throw new BusinessRuleException("לא ניתן להשאיל ספר לחבר שאינו פעיל.");
            }

            // חוק עסקי: חייב להיות עותק זמין
            if (book.CopiesAvailable <= 0)
            {
                throw new BusinessRuleException($"אין עותקים זמינים להשאלה עבור הספר '{book.Title}'.");
            }

            var loan = new Loan
            {
                BookId = dto.BookId,
                MemberId = dto.MemberId,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(dto.LoanPeriodDays)
            };

            book.CopiesAvailable -= 1;
            _unitOfWork.Books.Update(book);

            await _unitOfWork.Loans.AddAsync(loan);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("נוצרה השאלה חדשה: ספר {BookId} לחבר {MemberId}", dto.BookId, dto.MemberId);

            // שליפה מחדש עם Include כדי שה-DTO יקבל את BookTitle ו-MemberFullName
            var loansWithDetails = await _unitOfWork.Loans.GetAllAsync(nameof(Loan.Book), nameof(Loan.Member));
            var createdLoan = loansWithDetails.First(l => l.LoanId == loan.LoanId);

            return _mapper.Map<LoanDto>(createdLoan);
        }

        public async Task<LoanDto> ReturnBookAsync(int loanId)
        {
            var loan = await _unitOfWork.Loans.GetByIdAsync(loanId);
            if (loan == null)
            {
                throw new NotFoundException(nameof(Loan), loanId);
            }

            if (loan.ReturnDate.HasValue)
            {
                throw new BusinessRuleException("הספר כבר הוחזר.");
            }

            loan.ReturnDate = DateTime.UtcNow;
            _unitOfWork.Loans.Update(loan);

            var book = await _unitOfWork.Books.GetByIdAsync(loan.BookId);
            if (book != null)
            {
                book.CopiesAvailable += 1;
                _unitOfWork.Books.Update(book);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("הוחזר ספר עבור השאלה {LoanId}", loanId);

            // שליפה מחדש עם Include כדי שה-DTO יקבל את BookTitle ו-MemberFullName
            var loansWithDetails = await _unitOfWork.Loans.GetAllAsync(nameof(Loan.Book), nameof(Loan.Member));
            var updatedLoan = loansWithDetails.First(l => l.LoanId == loanId);

            return _mapper.Map<LoanDto>(updatedLoan);
        }

        public async Task DeleteLoanAsync(int id)
        {
            var loan = await _unitOfWork.Loans.GetByIdAsync(id);
            if (loan == null)
            {
                throw new NotFoundException(nameof(Loan), id);
            }

            if (!loan.ReturnDate.HasValue)
            {
                var book = await _unitOfWork.Books.GetByIdAsync(loan.BookId);
                if (book != null)
                {
                    book.CopiesAvailable += 1;
                    _unitOfWork.Books.Update(book);
                }
            }

            _unitOfWork.Loans.Remove(loan);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("נמחקה השאלה {LoanId}", id);
        }
    }
}
