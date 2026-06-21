using Library.BusinessLogic.DTOs;
using Library.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    /// <summary>
    /// ניהול חברי ספרייה - CRUD מלא.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        /// <response code="200">רשימת החברים הוחזרה בהצלחה</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MemberDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetAll()
        {
            var members = await _memberService.GetAllMembersAsync();
            return Ok(members);
        }

        /// <response code="200">החבר נמצא</response>
        /// <response code="404">החבר לא נמצא</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MemberDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MemberDto>> GetById([FromRoute] int id)
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            return Ok(member);
        }

        /// <response code="201">החבר נוצר בהצלחה</response>
        /// <response code="400">קלט לא תקין או אימייל כפול</response>
        [HttpPost]
        [ProducesResponseType(typeof(MemberDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MemberDto>> Create([FromBody] CreateMemberDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _memberService.CreateMemberAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.MemberId }, created);
        }

        /// <response code="204">העדכון בוצע בהצלחה</response>
        /// <response code="400">קלט לא תקין</response>
        /// <response code="404">החבר לא נמצא</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateMemberDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _memberService.UpdateMemberAsync(id, dto);
            return NoContent();
        }

        /// <response code="204">החבר נמחק בהצלחה</response>
        /// <response code="400">לא ניתן למחוק (לדוגמה השאלות פעילות)</response>
        /// <response code="404">החבר לא נמצא</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _memberService.DeleteMemberAsync(id);
            return NoContent();
        }
    }
}
