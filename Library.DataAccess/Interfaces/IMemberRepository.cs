using Library.Models.Entities;

namespace Library.DataAccess.Interfaces
{
    public interface IMemberRepository : IGenericRepository<Member>
    {
        Task<Member?> GetByIdWithLoansAsync(int memberId);
        Task<bool> EmailExistsAsync(string email, int? excludeMemberId = null);
    }
}
