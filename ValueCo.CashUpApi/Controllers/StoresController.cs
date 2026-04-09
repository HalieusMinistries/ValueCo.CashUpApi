using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ValueCo.CashUpApi.Data;

namespace ValueCo.CashUpApi.Controllers;

[ApiController]
[Route("api/stores")]
public class StoresController : ControllerBase
{
    private readonly CashUpDbContext _db;
    public StoresController(CashUpDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Stores.Where(s => s.IsActive).OrderBy(s => s.StoreCode).ToListAsync());
    [HttpPost]
    public async Task<IActionResult> AddStore([FromBody] ValueCo.CashUpApi.Models.Store store)
    {
        if (await _db.Stores.AnyAsync(s => s.StoreCode == store.StoreCode))
            return Conflict($"Store {store.StoreCode} already exists");
        store.IsActive = true;
        _db.Stores.Add(store);
        await _db.SaveChangesAsync();
        return Ok(store);
    }

    [HttpPut("{storeCode}")]
    public async Task<IActionResult> UpdateStore(string storeCode, [FromBody] ValueCo.CashUpApi.Models.Store updated)
    {
        var store = await _db.Stores.FirstOrDefaultAsync(s => s.StoreCode == storeCode);
        if (store == null) return NotFound($"Store {storeCode} not found");
        store.StoreCode = updated.StoreCode;
        store.StoreName = updated.StoreName;
        store.BankAccount = updated.BankAccount;
        store.SpeedPointId = updated.SpeedPointId;
        store.IsActive = updated.IsActive;
        await _db.SaveChangesAsync();
        return Ok(store);
    }
}