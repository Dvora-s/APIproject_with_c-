-- =====================================================================
-- Library Management System - סקריפט יצירת מסד הנתונים
-- =====================================================================
-- סקריפט זה יוצר את כל הטבלאות, הקשרים והאינדקסים במידה ולא רוצים
-- להריץ EF Core Migrations (Add-Migration / Update-Database).
-- מתאים להרצה מול LocalDB / SQL Server.
-- =====================================================================

IF DB_ID('LibraryDb') IS NULL
BEGIN
    CREATE DATABASE LibraryDb;
END
GO

USE LibraryDb;
GO

-- ===================== Categories =====================
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
GO
CREATE TABLE dbo.Categories
(
    CategoryId   INT IDENTITY(1,1) PRIMARY KEY,
    Name         NVARCHAR(100) NOT NULL,
    Description  NVARCHAR(300) NULL
);
GO

-- ===================== Authors =====================
IF OBJECT_ID('dbo.Authors', 'U') IS NOT NULL DROP TABLE dbo.Authors;
GO
CREATE TABLE dbo.Authors
(
    AuthorId    INT IDENTITY(1,1) PRIMARY KEY,
    FirstName   NVARCHAR(100) NOT NULL,
    LastName    NVARCHAR(100) NOT NULL,
    BirthDate   DATETIME2 NULL,
    Nationality NVARCHAR(100) NULL
);
GO

-- ===================== Members =====================
IF OBJECT_ID('dbo.Members', 'U') IS NOT NULL DROP TABLE dbo.Members;
GO
CREATE TABLE dbo.Members
(
    MemberId    INT IDENTITY(1,1) PRIMARY KEY,
    FirstName   NVARCHAR(100) NOT NULL,
    LastName    NVARCHAR(100) NOT NULL,
    Email       NVARCHAR(150) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    JoinDate    DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    IsActive    BIT NOT NULL DEFAULT (1),
    CONSTRAINT UQ_Members_Email UNIQUE (Email)
);
GO

-- ===================== Books =====================
IF OBJECT_ID('dbo.Books', 'U') IS NOT NULL DROP TABLE dbo.Books;
GO
CREATE TABLE dbo.Books
(
    BookId           INT IDENTITY(1,1) PRIMARY KEY,
    Title            NVARCHAR(200) NOT NULL,
    ISBN             NVARCHAR(20) NOT NULL,
    CopiesTotal      INT NOT NULL,
    CopiesAvailable  INT NOT NULL,
    PublicationYear  INT NOT NULL,
    Genre            NVARCHAR(100) NULL,
    CategoryId       INT NOT NULL,
    CONSTRAINT UQ_Books_ISBN UNIQUE (ISBN),
    CONSTRAINT FK_Books_Categories FOREIGN KEY (CategoryId)
        REFERENCES dbo.Categories(CategoryId)
);
GO

-- ===================== BookAuthors (Many-to-Many join table) =====================
IF OBJECT_ID('dbo.BookAuthors', 'U') IS NOT NULL DROP TABLE dbo.BookAuthors;
GO
CREATE TABLE dbo.BookAuthors
(
    BookId   INT NOT NULL,
    AuthorId INT NOT NULL,
    CONSTRAINT PK_BookAuthors PRIMARY KEY (BookId, AuthorId),
    CONSTRAINT FK_BookAuthors_Books FOREIGN KEY (BookId)
        REFERENCES dbo.Books(BookId) ON DELETE CASCADE,
    CONSTRAINT FK_BookAuthors_Authors FOREIGN KEY (AuthorId)
        REFERENCES dbo.Authors(AuthorId) ON DELETE CASCADE
);
GO

-- ===================== Loans =====================
IF OBJECT_ID('dbo.Loans', 'U') IS NOT NULL DROP TABLE dbo.Loans;
GO
CREATE TABLE dbo.Loans
(
    LoanId     INT IDENTITY(1,1) PRIMARY KEY,
    BookId     INT NOT NULL,
    MemberId   INT NOT NULL,
    LoanDate   DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    DueDate    DATETIME2 NOT NULL,
    ReturnDate DATETIME2 NULL,
    CONSTRAINT FK_Loans_Books FOREIGN KEY (BookId)
        REFERENCES dbo.Books(BookId),
    CONSTRAINT FK_Loans_Members FOREIGN KEY (MemberId)
        REFERENCES dbo.Members(MemberId)
);
GO

-- ===================== Seed Data =====================
INSERT INTO dbo.Categories (Name, Description) VALUES
    (N'מדע בדיוני', N'ספרי מדע בדיוני ופנטזיה'),
    (N'היסטוריה', N'ספרי היסטוריה ותיעוד'),
    (N'רומן', N'רומנים וספרות יפה');
GO

INSERT INTO dbo.Authors (FirstName, LastName, Nationality) VALUES
    (N'עמוס', N'עוז', N'ישראלי'),
    (N'אתגר', N'קרת', N'ישראלי');
GO

INSERT INTO dbo.Books (Title, ISBN, CopiesTotal, CopiesAvailable, PublicationYear, Genre, CategoryId) VALUES
    (N'סיפור על אהבה וחושך', N'978-965-07-1234-5', 3, 3, 2002, N'אוטוביוגרפיה', 3);
GO

INSERT INTO dbo.BookAuthors (BookId, AuthorId) VALUES (1, 1);
GO

PRINT 'מסד הנתונים LibraryDb נוצר בהצלחה.';
