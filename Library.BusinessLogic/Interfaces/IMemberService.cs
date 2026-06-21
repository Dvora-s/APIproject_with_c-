using Library.BusinessLogic.DTOs;

namespace Library.BusinessLogic.Interfaces
{
    /// <summary>
    /// שירות הלוגיקה העסקית לניהול חברי ספרייה - CRUD מלא.
    /// </summary>
    public interface IMemberService
    {
        Task<IEnumerable<MemberDto>> GetAllMembersAsync();
        Task<MemberDto> GetMemberByIdAsync(int id);
        Task<MemberDto> CreateMemberAsync(CreateMemberDto dto);
        Task UpdateMemberAsync(int id, UpdateMemberDto dto);
        Task DeleteMemberAsync(int id);
    }
}
