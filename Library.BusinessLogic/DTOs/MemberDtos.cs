using System.ComponentModel.DataAnnotations;

namespace Library.BusinessLogic.DTOs
{
    /// <summary>DTO להצגת חבר ספרייה.</summary>
    public class MemberDto
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
        public int ActiveLoansCount { get; set; }
    }

    /// <summary>DTO ליצירת חבר חדש - כולל Validation.</summary>
    public class CreateMemberDto
    {
        [Required(ErrorMessage = "שם פרטי הוא שדה חובה")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "שם משפחה הוא שדה חובה")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "כתובת אימייל היא שדה חובה")]
        [EmailAddress(ErrorMessage = "כתובת אימייל אינה תקינה")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "מספר טלפון אינו תקין")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
    }

    /// <summary>DTO לעדכון חבר קיים.</summary>
    public class UpdateMemberDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }
    }
}
