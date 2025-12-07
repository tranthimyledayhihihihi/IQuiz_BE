using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.InputModels;
using System.Security.Claims;

[Route("api/trandau")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
public class TranDauController : ControllerBase
{
    private readonly IOnlineMatchService _service;

    public TranDauController(IOnlineMatchService service)
    {
        _service = service;
    }

    private int GetUserId()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(id);
    }

    [HttpGet("{matchCode}")]
    public async Task<IActionResult> GetMatch(string matchCode)
    {
        var match = await _service.GetMatchByCodeAsync(matchCode);
        if (match == null) return NotFound();

        return Ok(match);
    }

    [HttpPost("gui-dap-an/{matchCode}")]
    public async Task<IActionResult> SubmitAnswer(string matchCode, [FromBody] MatchAnswerModel model)
    {
        int id = GetUserId();
        bool ok = await _service.SubmitAnswerByMatchCodeAsync(matchCode, id, model);
        if (!ok) return BadRequest();

        return NoContent();
    }

    [HttpPost("ket-thuc/{matchCode}")]
    public async Task<IActionResult> EndMatch(string matchCode)
    {
        var result = await _service.EndMatchByCodeAsync(matchCode);
        return Ok(result);
    }
}
