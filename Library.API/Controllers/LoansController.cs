using Library.BusinessLogic.DTOs;
using Library.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    /// <summary>
    /// ניהול השאלות - מדגים את הקשר בין Book ל-Member, כולל החזרת ספר.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        /// <response code="200">רשימת ההשאלות הוחזרה בהצלחה</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LoanDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetAll()
        {
            var loans = await _loanService.GetAllLoansAsync();
            return Ok(loans);
        }

        /// <response code="200">ההשאלה נמצאה</response>
        /// <response code="404">ההשאלה לא נמצאה</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(LoanDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LoanDto>> GetById([FromRoute] int id)
        {
            var loan = await _loanService.GetLoanByIdAsync(id);
            return Ok(loan);
        }

        /// <summary>יצירת השאלה חדשה - בודק זמינות עותקים וסטטוס חבר.</summary>
        /// <response code="201">ההשאלה נוצרה בהצלחה</response>
        /// <response code="400">קלט לא תקין או הפרת חוק עסקי (אין עותקים זמינים / חבר לא פעיל)</response>
        /// <response code="404">הספר או החבר לא נמצאו</response>
        [HttpPost]
        [ProducesResponseType(typeof(LoanDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LoanDto>> Create([FromBody] CreateLoanDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _loanService.CreateLoanAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.LoanId }, created);
        }

        /// <summary>החזרת ספר עבור השאלה קיימת.</summary>
        /// <response code="200">הספר הוחזר בהצלחה</response>
        /// <response code="400">הספר כבר הוחזר בעבר</response>
        /// <response code="404">ההשאלה לא נמצאה</response>
        [HttpPut("{id:int}/return")]
        [ProducesResponseType(typeof(LoanDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LoanDto>> ReturnBook([FromRoute] int id)
        {
            var updated = await _loanService.ReturnBookAsync(id);
            return Ok(updated);
        }

        /// <response code="204">ההשאלה נמחקה בהצלחה</response>
        /// <response code="404">ההשאלה לא נמצאה</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _loanService.DeleteLoanAsync(id);
            return NoContent();
        }
    }
}
