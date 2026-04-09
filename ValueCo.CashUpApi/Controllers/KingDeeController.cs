using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ValueCo.CashUpApi.Data;
using ValueCo.CashUpApi.Models;

namespace ValueCo.CashUpApi.Controllers;

[ApiController]
[Route("api/kingdee")]
public class KingDeeController : ControllerBase
{
    private readonly KingDeeDbContext _kd;

    public KingDeeController(KingDeeDbContext kd) => _kd = kd;

    [HttpGet("contributions")]
    public async Task<IActionResult> GetContributions([FromQuery] string store, [FromQuery] string date)
    {
        if (string.IsNullOrEmpty(store) || string.IsNullOrEmpty(date))
            return BadRequest("store and date are required");

        // TODO: Replace with actual KingDee mirror query once Robbie provides table names
        var result = new List<KdContribution>();
        return Ok(result);
    }

    [HttpGet("journal")]
    public async Task<IActionResult> GetJournal([FromQuery] string store, [FromQuery] string date)
    {
        if (string.IsNullOrEmpty(store) || string.IsNullOrEmpty(date))
            return BadRequest("store and date are required");

        // TODO: Replace with actual KingDee mirror query once Robbie provides table names
        var result = new List<KdJournal>();
        return Ok(result);
    }

    [HttpGet("sales")]
    public async Task<IActionResult> GetSales([FromQuery] string store, [FromQuery] string date)
    {
        if (string.IsNullOrEmpty(store) || string.IsNullOrEmpty(date))
            return BadRequest("store and date are required");

        // TODO: Replace with actual KingDee mirror query once Robbie provides table names
        var result = new List<KdSales>();
        return Ok(result);
    }
}