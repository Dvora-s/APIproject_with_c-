using Library.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Data
{
    /// <summary>
    /// ה-DbContext הראשי של המערכת. כל הגישה למסד הנתונים עוברת דרכו,
    /// ורק שכבת ה-DataAccess מכירה אותו (ה-API וה-BusinessLogic לא מכירים EF ישירות).
    /// </summary>
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Member> Members { get; set; } = null!;
        public DbSet<Loan> Loans { get; set; } = null!;
        public DbSet<BookAuthor> BookAuthors { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ----- Many-to-Many: Book <-> Author (דרך BookAuthor) -----
            modelBuilder.Entity<BookAuthor>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId });

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Book)
                .WithMany(b => b.BookAuthors)
                .HasForeignKey(ba => ba.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Author)
                .WithMany(a => a.BookAuthors)
                .HasForeignKey(ba => ba.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // ----- Many-to-One: Book -> Category -----
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- Many-to-One: Loan -> Book -----
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- Many-to-One: Loan -> Member -----
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Member)
                .WithMany(m => m.Loans)
                .HasForeignKey(l => l.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            // IsReturned הוא Property מחושב (get-only) ולא עמודה במסד הנתונים
            modelBuilder.Entity<Loan>().Ignore(l => l.IsReturned);

            // אינדקס ייחודי - לא ניתן להזין אותו ISBN פעמיים
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();

            // אינדקס ייחודי - לא ניתן להזין אותו אימייל פעמיים
            modelBuilder.Entity<Member>()
                .HasIndex(m => m.Email)
                .IsUnique();

            // ----- Seed Data ראשוני לבדיקות -----
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "מדע בדיוני", Description = "ספרי מדע בדיוני ופנטזיה" },
                new Category { CategoryId = 2, Name = "היסטוריה", Description = "ספרי היסטוריה ותיעוד" },
                new Category { CategoryId = 3, Name = "רומן", Description = "רומנים וספרות יפה" }
            );

            modelBuilder.Entity<Author>().HasData(
                new Author { AuthorId = 1, FirstName = "עמוס", LastName = "עוז", Nationality = "ישראלי" },
                new Author { AuthorId = 2, FirstName = "אתגר", LastName = "קרת", Nationality = "ישראלי" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    BookId = 1,
                    Title = "סיפור על אהבה וחושך",
                    ISBN = "978-965-07-1234-5",
                    CopiesTotal = 3,
                    CopiesAvailable = 3,
                    PublicationYear = 2002,
                    Genre = "אוטוביוגרפיה",
                    CategoryId = 3
                }
            );

            modelBuilder.Entity<BookAuthor>().HasData(
                new BookAuthor { BookId = 1, AuthorId = 1 }
            );
        }
    }
}
