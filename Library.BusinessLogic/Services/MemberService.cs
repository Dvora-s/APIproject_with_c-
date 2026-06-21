using AutoMapper;
using Library.BusinessLogic.DTOs;
using Library.BusinessLogic.Exceptions;
using Library.BusinessLogic.Interfaces;
using Library.DataAccess.Interfaces;
using Library.Models.Entities;
using Microsoft.Extensions.Logging;

namespace Library.BusinessLogic.Services
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MemberService> _logger;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MemberService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<MemberDto>> GetAllMembersAsync()
        {
            var members = await _unitOfWork.Members.GetAllAsync(nameof(Member.Loans));
            return _mapper.Map<IEnumerable<MemberDto>>(members);
        }

        public async Task<MemberDto> GetMemberByIdAsync(int id)
        {
            var member = await _unitOfWork.Members.GetByIdWithLoansAsync(id);
            if (member == null)
            {
                _logger.LogWarning("חבר עם מזהה {MemberId} לא נמצא", id);
                throw new NotFoundException(nameof(Member), id);
            }

            return _mapper.Map<MemberDto>(member);
        }

        public async Task<MemberDto> CreateMemberAsync(CreateMemberDto dto)
        {
            if (await _unitOfWork.Members.EmailExistsAsync(dto.Email))
            {
                throw new BusinessRuleException($"חבר עם כתובת אימייל '{dto.Email}' כבר רשום במערכת.");
            }

            var member = _mapper.Map<Member>(dto);
            member.JoinDate = DateTime.UtcNow;
            member.IsActive = true;

            await _unitOfWork.Members.AddAsync(member);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("נוצר חבר חדש: {FirstName} {LastName} (מזהה {MemberId})",
                member.FirstName, member.LastName, member.MemberId);

            return _mapper.Map<MemberDto>(member);
        }

        public async Task UpdateMemberAsync(int id, UpdateMemberDto dto)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(id);
            if (member == null)
            {
                throw new NotFoundException(nameof(Member), id);
            }

            if (await _unitOfWork.Members.EmailExistsAsync(dto.Email, excludeMemberId: id))
            {
                throw new BusinessRuleException($"חבר אחר עם כתובת אימייל '{dto.Email}' כבר קיים.");
            }

            member.FirstName = dto.FirstName;
            member.LastName = dto.LastName;
            member.Email = dto.Email;
            member.PhoneNumber = dto.PhoneNumber;
            member.IsActive = dto.IsActive;

            _unitOfWork.Members.Update(member);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("עודכן חבר {MemberId}", id);
        }

        public async Task DeleteMemberAsync(int id)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(id);
            if (member == null)
            {
                throw new NotFoundException(nameof(Member), id);
            }

            var hasActiveLoans = await _unitOfWork.Loans.ExistsAsync(l => l.MemberId == id && l.ReturnDate == null);
            if (hasActiveLoans)
            {
                throw new BusinessRuleException("לא ניתן למחוק חבר עם השאלות פעילות.");
            }

            _unitOfWork.Members.Remove(member);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("נמחק חבר {MemberId}", id);
        }
    }
}
