using AutoMapper;
using Library.BusinessLogic.DTOs;
using Library.Models.Entities;

namespace Library.BusinessLogic.Mapping
{
    /// <summary>
    /// פרופיל AutoMapper מרכזי - ממפה בין Entities (DB) לבין DTOs (חשיפה ללקוח).
    /// כך הלקוח אף פעם לא חשוף ישירות לישויות מה-Database.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ---------- Book ----------
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.AuthorNames,
                    opt => opt.MapFrom(src => src.BookAuthors
                        .Where(ba => ba.Author != null)
                        .Select(ba => $"{ba.Author!.FirstName} {ba.Author.LastName}")));

            CreateMap<CreateBookDto, Book>()
                .ForMember(dest => dest.CopiesAvailable, opt => opt.MapFrom(src => src.CopiesTotal))
                .ForMember(dest => dest.BookAuthors, opt => opt.Ignore());

            CreateMap<UpdateBookDto, Book>()
                .ForMember(dest => dest.BookAuthors, opt => opt.Ignore());

            // ---------- Author ----------
            CreateMap<Author, AuthorDto>();
            CreateMap<CreateAuthorDto, Author>();

            // ---------- Category ----------
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();

            // ---------- Member ----------
            CreateMap<Member, MemberDto>()
                .ForMember(dest => dest.ActiveLoansCount,
                    opt => opt.MapFrom(src => src.Loans.Count(l => l.ReturnDate == null)));

            CreateMap<CreateMemberDto, Member>();
            CreateMap<UpdateMemberDto, Member>();

            // ---------- Loan ----------
            CreateMap<Loan, LoanDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : string.Empty))
                .ForMember(dest => dest.MemberFullName,
                    opt => opt.MapFrom(src => src.Member != null ? $"{src.Member.FirstName} {src.Member.LastName}" : string.Empty))
                .ForMember(dest => dest.IsReturned, opt => opt.MapFrom(src => src.ReturnDate.HasValue));
        }
    }
}
