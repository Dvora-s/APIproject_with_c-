using Library.BusinessLogic.DTOs;
using Library.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    /// <summary>
    /// ניהול ספרים - CRUD מלא. כל הפעולות אסינכרוניות ומחזירות קודי סטטוס מתאימים.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        /// <summary>שליפת כל הספרים.</summary>
        /// <response code="200">רשימת הספרים הוחזרה בהצלחה</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        /// <summary>שליפת ספר לפי מזהה (Route parameter).</summary>
        /// <response code="200">הספר נמצא</response>
        /// <response code="404">הספר לא נמצא</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> GetById([FromRoute] int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            return Ok(book);
        }

        /// <summary>יצירת ספר חדש (Body parameter).</summary>
        /// <response code="201">הספר נוצר בהצלחה</response>
        /// <response code="400">קלט לא תקין או הפרת חוק עסקי (לדוגמה ISBN כפול)</response>
        [HttpPost]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _bookService.CreateBookAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.BookId }, created);
        }

        /// <summary>עדכון ספר קיים.</summary>
        /// <response code="204">העדכון בוצע בהצלחה</response>
        /// <response code="400">קלט לא תקין</response>
        /// <response code="404">הספר לא נמצא</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateBookDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _bookService.UpdateBookAsync(id, dto);
            return NoContent();
        }

        /// <summary>מחיקת ספר.</summary>
        /// <response code="204">הספר נמחק בהצלחה</response>
        /// <response code="400">לא ניתן למחוק (לדוגמה השאלות פעילות)</response>
        /// <response code="404">הספר לא נמצא</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }

        /// <summary>חיפוש ספרים לפי ז'אנר (Query parameter) - דוגמה לקבלת פרמטר מה-Query String.</summary>
        /// <response code="200">תוצאות החיפוש</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<BookDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookDto>>> SearchByGenre([FromQuery] string? genre)
        {
            var books = await _bookService.SearchByGenreAsync(genre);
            return Ok(books);
        }
    }
}
