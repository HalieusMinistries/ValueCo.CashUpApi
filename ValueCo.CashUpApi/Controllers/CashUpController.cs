using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ValueCo.CashUpApi.Data;
using ValueCo.CashUpApi.DTOs;
using ValueCo.CashUpApi.Models;

namespace ValueCo.CashUpApi.Controllers;

[ApiController]
[Route("api/cashup")]
public class CashUpController : ControllerBase
{
    private readonly CashUpDbContext _db;
    public CashUpController(CashUpDbContext db) => _db = db;

    [HttpGet("{storeCode}/{year:int}/{month:int}")]
    public async Task<IActionResult> GetMonth(string storeCode, int year, int month)
    {
        var store = await _db.Stores.FirstOrDefaultAsync(s => s.StoreCode == storeCode);
        if (store == null) return NotFound($"Store {storeCode} not found");

        var days = await _db.CashUpDays
            .Include(d => d.CashierRows)
            .Include(d => d.EFTDetails)
            .Where(d => d.StoreId == store.StoreId
                     && d.CashUpDate.Year == year
                     && d.CashUpDate.Month == month)
            .OrderBy(d => d.CashUpDate)
            .ToListAsync();

        return Ok(days);
    }

    [HttpGet("{storeCode}/{year:int}/{month:int}/{day:int}")]
    public async Task<IActionResult> GetDay(string storeCode, int year, int month, int day)
    {
        var store = await _db.Stores.FirstOrDefaultAsync(s => s.StoreCode == storeCode);
        if (store == null) return NotFound($"Store {storeCode} not found");

        var date = new DateOnly(year, month, day);
        var record = await _db.CashUpDays
            .Include(d => d.CashierRows)
            .Include(d => d.EFTDetails)
            .FirstOrDefaultAsync(d => d.StoreId == store.StoreId && d.CashUpDate == date);

        if (record == null) return NotFound();
        return Ok(record);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] CashUpSubmitDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var store = await _db.Stores.FirstOrDefaultAsync(s => s.StoreCode == dto.StoreCode);
        if (store == null) return NotFound($"Store {dto.StoreCode} not found");

        var date = new DateOnly(dto.Year, dto.Month, dto.Day);

        var existing = await _db.CashUpDays
            .Include(d => d.CashierRows)
            .Include(d => d.EFTDetails)
            .FirstOrDefaultAsync(d => d.StoreId == store.StoreId && d.CashUpDate == date);

        if (existing == null)
        {
            var newDay = MapToEntity(dto, store.StoreId, date);
            _db.CashUpDays.Add(newDay);
        }
        else
        {
            if (existing.Submitted && !dto.Submitted)
                return Conflict("Day is already submitted. Contact Head Office to unlock.");
            UpdateEntity(existing, dto);
        }

        try
        {
            await _db.SaveChangesAsync();
            return Ok(new { message = "Saved", date = date.ToString("yyyy-MM-dd") });
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
        {
            var retry = await _db.CashUpDays
                .Include(d => d.CashierRows)
                .Include(d => d.EFTDetails)
                .FirstOrDefaultAsync(d => d.StoreId == store.StoreId && d.CashUpDate == date);
            if (retry == null) return StatusCode(500, "Unexpected conflict");
            UpdateEntity(retry, dto);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Updated (conflict resolved)", date = date.ToString("yyyy-MM-dd") });
        }
    }

    private static CashUpDay MapToEntity(CashUpSubmitDto dto, int storeId, DateOnly date) => new()
    {
        StoreId = storeId,
        CashUpDate = date,
        FNB = dto.FNB,
        Surrender = dto.Surrender,
        Floats = dto.Floats,
        ChangeBoxes = dto.ChangeBoxes,
        LooseChange = dto.LooseChange,
        PettyCash = dto.PettyCash,
        PettyNotes = dto.PettyNotes,
        CashUpNotes = dto.CashUpNotes,
        CountingBanking = dto.CountingBanking,
        Submitted = dto.Submitted,
        SubmittedAt = dto.SubmittedAt,
        CashierRows = dto.Cashiers.Select(c => new CashierRow
        {
            CashierName = c.Name,
            Cash = c.Cash,
            Card = c.Card,
            EFT = c.EFT,
            Erase = c.Erase,
            Returns = c.Returns,
            Gift = c.Gift,
            Coupon = c.Coupon,
            Loyalty = c.Loyalty
        }).ToList(),
        EFTDetails = dto.EFTDetails.Select(e => new EFTDetail
        {
            EFTDate = DateOnly.TryParse(e.Date, out var d) ? d : DateOnly.FromDateTime(DateTime.Today),
            SONumber = e.SONumber,
            Amount = e.Amount
        }).ToList()
    };

    private static void UpdateEntity(CashUpDay existing, CashUpSubmitDto dto)
    {
        existing.FNB = dto.FNB;
        existing.Surrender = dto.Surrender;
        existing.Floats = dto.Floats;
        existing.ChangeBoxes = dto.ChangeBoxes;
        existing.LooseChange = dto.LooseChange;
        existing.PettyCash = dto.PettyCash;
        existing.PettyNotes = dto.PettyNotes;
        existing.CashUpNotes = dto.CashUpNotes;
        existing.CountingBanking = dto.CountingBanking;
        existing.Submitted = dto.Submitted;
        existing.SubmittedAt = dto.SubmittedAt;
        existing.CashierRows.Clear();
        foreach (var c in dto.Cashiers)
            existing.CashierRows.Add(new CashierRow
            {
                CashierName = c.Name,
                Cash = c.Cash,
                Card = c.Card,
                EFT = c.EFT,
                Erase = c.Erase,
                Returns = c.Returns,
                Gift = c.Gift,
                Coupon = c.Coupon,
                Loyalty = c.Loyalty
            });
        existing.EFTDetails.Clear();
        foreach (var e in dto.EFTDetails)
            existing.EFTDetails.Add(new EFTDetail
            {
                EFTDate = DateOnly.TryParse(e.Date, out var d) ? d : DateOnly.FromDateTime(DateTime.Today),
                SONumber = e.SONumber,
                Amount = e.Amount
            });
    }

    [HttpDelete("{storeCode}/{year:int}/{month:int}")]
    public async Task<IActionResult> DeleteMonth(string storeCode, int year, int month)
    {
        var store = await _db.Stores.FirstOrDefaultAsync(s => s.StoreCode == storeCode);
        if (store == null) return NotFound($"Store {storeCode} not found");

        var days = await _db.CashUpDays
            .Include(d => d.CashierRows)
            .Include(d => d.EFTDetails)
            .Where(d => d.StoreId == store.StoreId &&
                        d.CashUpDate.Year == year &&
                        d.CashUpDate.Month == month)
            .ToListAsync();

        if (!days.Any())
            return NotFound($"No data found for {storeCode} {month}/{year}");

        _db.CashUpDays.RemoveRange(days);
        await _db.SaveChangesAsync();

        return Ok($"Deleted {days.Count} day(s) for {storeCode} {month}/{year}");
    }
}