using Library.DataAccess.Data;
using Library.DataAccess.Interfaces;
using Library.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories
{
    public class MemberRepository : GenericRepository<Member>, IMemberRepository
    {
        public MemberRepository(LibraryDbContext context) : base(context)
        {
        }

        public async Task<Member?> GetByIdWithLoansAsync(int memberId)
        {
            return await _dbSet
                .Include(m => m.Loans)
                    .ThenInclude(l => l.Book)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MemberId == memberId);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeMemberId = null)
        {
            return await _dbSet.AnyAsync(m => m.Email == email &&
                (!excludeMemberId.HasValue || m.MemberId != excludeMemberId.Value));
        }
    }
}
