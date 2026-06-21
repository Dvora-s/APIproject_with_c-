using System.ComponentModel.DataAnnotations;

namespace Library.Models.Entities
{
    /// <summary>
    /// מייצג חבר/ה רשום/ה בספרייה.
    /// </summary>
    public class Member
    {
        [Key]
        public int MemberId { get; set; }

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

        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // One-to-Many: חבר אחד יכול להשאיל כמה ספרים לאורך זמן
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
