# Library Management System – ASP.NET Core Web API

פרויקט סיום לקורס ASP.NET Core Web API – מערכת לניהול ספרייה: ספרים, מחברים, חברים והשאלות.

## 1. תיאור קצר של המערכת

המערכת מאפשרת ניהול מלא של ספרייה:
- **ספרים** (Books) – כולל קטגוריה, מחברים, מספר עותקים כולל וזמינים.
- **מחברים** (Authors) – קשורים לספרים בקשר Many-to-Many.
- **קטגוריות** (Categories) – קשורות לספרים בקשר Many-to-One.
- **חברי ספרייה** (Members) – הרשומים במערכת ויכולים להשאיל ספרים.
- **השאלות** (Loans) – מקשרות בין ספר לחבר, כולל תאריך השאלה, תאריך יעד והחזרה. בעת השאלה נבדק שיש עותק זמין ושהחבר פעיל; בעת החזרה מתעדכן מלאי העותקים אוטומטית.

הפרויקט בנוי בארכיטקטורת שכבות מלאה (API → Business Logic → Data Access), עם Entity Framework Core (Code-First), DTOs + AutoMapper, Dependency Injection, Validation, טיפול שגיאות גלובלי, ולוגינג.

## 2. הוראות התקנה והרצה

### דרישות מקדימות
- Visual Studio 2022 (או 2026) עם עומס עבודה ASP.NET ו-Web Development
- .NET 8 SDK
- SQL Server LocalDB (מותקן כברירת מחדל עם Visual Studio)

### שלבי הרצה

1. **שכפול הריפו**
   ```bash
   git clone https://github.com/Dvora-s/APIproject_with_c-.git
   cd APIproject_with_c-
   ```

2. **פתיחת הפתרון** – פתחו את `LibrarySystem.sln` ב-Visual Studio.

3. **בדיקת מחרוזת החיבור** – בקובץ `Library.API/appsettings.json` מוגדרת מחרוזת חיבור ל-LocalDB:
   ```json
   "DefaultConnection": "Server=(localdb)\\mssqllocaldb;AttachDbFilename=|DataDirectory|\\LibraryDb.mdf;Database=LibraryDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   ```
   אם ברצונכם לעבוד מול SQL Server רגיל או MySQL, יש לעדכן כאן בלבד (לא בקוד) – ראו `בחירת_סוג_DB_לפרויקט.pdf`.

4. **יצירת מסד הנתונים** – שתי אפשרויות:

   **אפשרות א' – EF Core Migrations (מומלץ):**
   ב-Package Manager Console, בחרו את `Library.DataAccess` כ-Default Project והריצו:
   ```powershell
   Add-Migration InitialCreate -Project Library.DataAccess -StartupProject Library.API
   Update-Database -Project Library.DataAccess -StartupProject Library.API
   ```
   לחלופין, מה-Terminal (אם מותקן כלי `dotnet-ef`):
   ```bash
   dotnet tool install --global dotnet-ef
   cd Library.API
   dotnet ef migrations add InitialCreate -p ../Library.DataAccess -s .
   dotnet ef database update -p ../Library.DataAccess -s .
   ```
   המסד גם נוצר/מתעדכן אוטומטית בהרצה הראשונה הודות ל-`dbContext.Database.Migrate()` ב-`Program.cs`.

   **אפשרות ב' – סקריפט SQL ידני:**
   הריצו את `DB/CreateDatabase.sql` מול ה-LocalDB/SQL Server שלכם (ב-SSMS או SQL Server Object Explorer).

5. **הרצת הפרויקט** – הגדירו את `Library.API` כ-Startup Project והריצו (F5). הדפדפן ייפתח אוטומטית עם Swagger UI בכתובת השורש.

### בדיקת ה-API
- **Swagger UI**: נפתח אוטומטית בסביבת Development בכתובת הבסיס (`https://localhost:7258/`).
- **Postman**: ייבאו את הקובץ `Postman/LibraryManagementAPI.postman_collection.json` ועדכנו את המשתנה `baseUrl` בהתאם לפורט שבו רץ הפרויקט אצלכם.

## 3. תיאור הישויות המרכזיות

| ישות | תיאור | קשרים |
|---|---|---|
| **Book** | ספר: כותרת, ISBN, עותקים, שנת הוצאה, ז'אנר | Many-to-One ל-Category, Many-to-Many ל-Author (דרך BookAuthor), One-to-Many ל-Loan |
| **Author** | מחבר/ת ספר | Many-to-Many ל-Book |
| **Category** | קטגוריית ספרים | One-to-Many ל-Book |
| **Member** | חבר ספרייה | One-to-Many ל-Loan |
| **Loan** | השאלת ספר על ידי חבר, כולל תאריכי השאלה/יעד/החזרה | Many-to-One ל-Book ול-Member |
| **BookAuthor** | טבלת קשר (Join Table) למימוש Many-to-Many בין Book ל-Author | — |

## 4. מבנה השכבות

```
LibrarySystem/
├── Library.API/              # שכבת ה-API: Controllers, Middleware, Program.cs, appsettings
├── Library.BusinessLogic/    # שכבת הלוגיקה העסקית: Services, DTOs, AutoMapper Profiles, Exceptions
├── Library.DataAccess/       # שכבת הגישה לנתונים: DbContext, Repositories, Unit of Work
└── Library.Models/           # ישויות (Entities) משותפות - ה-DB Schema
```

- **Library.API** תלוי ב-BusinessLogic ו-DataAccess (ל-DI registration בלבד), אך הקונטרולרים עצמם משוחחים אך ורק עם שכבת ה-BusinessLogic (Services), ולא נוגעים ב-Entities או ב-DbContext ישירות.
- **Library.BusinessLogic** מכיל את כל הלוגיקה העסקית (חוקי תקינות, בדיקות זמינות עותקים וכו'), עובד מול DTOs בלבד כלפי חוץ, וממפה Entities↔DTOs באמצעות AutoMapper.
- **Library.DataAccess** הוא השכבה היחידה שמכירה את Entity Framework Core וה-DbContext. חושפת Repository Pattern + Unit of Work, עם פעולות אסינכרוניות בלבד.
- **Library.Models** מכיל רק את ה-Entities (POCO classes) המשותפות לכל השכבות.

הזרימה: `Controller → IService → IUnitOfWork/IRepository → DbContext → Database`, עם הזרקת תלויות (Dependency Injection) בכל המעברים בין השכבות.

## 5. הערות נוספות

- מחרוזת החיבור **אינה** מופיעה בקוד בשום מקום – נטענת אך ורק דרך `appsettings.json` ומנגנון ה-Configuration של ASP.NET Core.
- כל הגישה למסד הנתונים אסינכרונית (`async`/`await`).
- טיפול שגיאות גלובלי באמצעות `ExceptionHandlingMiddleware` – ממיר חריגות עסקיות ל-400, "לא נמצא" ל-404, ושגיאות בלתי צפויות ל-500 עם הודעה ידידותית ללקוח (מבלי לחשוף Stack Trace).
- Logging מובנה (Console + Debug) לפעולות מרכזיות ולשגיאות.
