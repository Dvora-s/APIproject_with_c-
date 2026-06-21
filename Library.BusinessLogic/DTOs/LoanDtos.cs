using System.ComponentModel.DataAnnotations;

namespace Library.BusinessLogic.DTOs
{
    /// <summary>DTO להצגת השאלה.</summary>
    public class LoanDto
    {
        public int LoanId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public int MemberId { get; set; }
        public string MemberFullName { get; set; } = string.Empty;
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }
    }

    /// <summary>DTO ליצירת השאלה חדשה.</summary>
    public class CreateLoanDto
    {
        [Required(ErrorMessage = "יש לבחור ספר")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "יש לבחור חבר")]
        public int MemberId { get; set; }

        [Range(1, 60, ErrorMessage = "תקופת ההשאלה חייבת להיות בין 1 ל-60 יום")]
        public int LoanPeriodDays { get; set; } = 14;
    }
}
