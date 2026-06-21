using Library.BusinessLogic.DTOs;

namespace Library.BusinessLogic.Interfaces
{
    /// <summary>
    /// שירות לניהול השאלות - מדגים לוגיקה עסקית שמשלבת בין Book ל-Member.
    /// </summary>
    public interface ILoanService
    {
        Task<IEnumerable<LoanDto>> GetAllLoansAsync();
        Task<LoanDto> GetLoanByIdAsync(int id);
        Task<LoanDto> CreateLoanAsync(CreateLoanDto dto);
        Task<LoanDto> ReturnBookAsync(int loanId);
        Task DeleteLoanAsync(int id);
    }
}
