using Library.API.Middleware;
using Library.BusinessLogic.Interfaces;
using Library.BusinessLogic.Mapping;
using Library.BusinessLogic.Services;
using Library.DataAccess.Data;
using Library.DataAccess.Interfaces;
using Library.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===================== Services Registration =====================

// --- Controllers ---
builder.Services.AddControllers();

// --- Swagger / OpenAPI ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Library Management API",
        Version = "v1",
        Description = "API לניהול ספרייה - ספרים, מחברים, חברים והשאלות. " +
                      "פרויקט סיום קורס ASP.NET Core Web API."
    });
});

// --- Database (Entity Framework Core) ---
// מחרוזת החיבור נטענת אך ורק מ-appsettings.json / Configuration - לא קיימת בקוד (סעיף 3 בדרישות)
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- AutoMapper ---
builder.Services.AddAutoMapper(typeof(MappingProfile));

// --- Dependency Injection: Data Access ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// --- Dependency Injection: Business Logic ---
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// --- Logging ---
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// ===================== Middleware Pipeline =====================

// טיפול שגיאות גלובלי - חייב להיות ראשון בצינור כדי לתפוס כל חריגה (סעיף 8 בדרישות)
app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Management API v1");
        options.RoutePrefix = string.Empty; // Swagger UI בכתובת השורש
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ===================== DB Migration on Startup =====================
// מבטיח שמסד הנתונים קיים ומעודכן בכל הרצה - נוח לסמינר/הגשה.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
