using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ValueCo.CashUpApi.Models;

namespace ValueCo.CashUpApi.Controllers;

[ApiController]
[Route("api/kingdee")]
public class KingDeeController : ControllerBase
{
    private readonly IConfiguration _config;

    public KingDeeController(IConfiguration config) => _config = config;

    private SqlConnection GetKdConnection() =>
        new SqlConnection(_config.GetConnectionString("KingDeeConnection"));

    [HttpGet("sales")]
    public async Task<IActionResult> GetSales([FromQuery] string store, [FromQuery] string dateFrom, [FromQuery] string dateTo)
    {
        if (string.IsNullOrEmpty(store) || string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(dateTo))
            return BadRequest("store, dateFrom and dateTo are required");

        const string sql = @"
            SELECT
                tblOrg.fdescription                        AS StoreCode,
                CONVERT(varchar(10), tblCMKRet.fdate, 23)  AS CashUpDate,
                caslng.FRealName                           AS CashierFullName,
                SUM(CASE WHEN tblCMKRetFine.FSETTLEID = 1     THEN ISNULL(tblCMKRetFine.FSETTLEAMOUNT,0) ELSE 0 END) AS Cash,
                SUM(CASE WHEN tblCMKRetFine.FSETTLEID = 10005 THEN ISNULL(tblCMKRetFine.FSETTLEAMOUNT,0) ELSE 0 END) AS EFT,
                SUM(CASE WHEN tblCMKRetFine.FSETTLEID = 2     THEN ISNULL(tblCMKRetFine.FSETTLEAMOUNT,0) ELSE 0 END) AS Card,
                SUM(CASE WHEN tblCMKRetFine.FSETTLEID = 1000  THEN ISNULL(tblCMKRetFine.FSETTLEAMOUNT,0) ELSE 0 END) AS Rounding,
                SUM(CASE WHEN tblCMKRet.FRETURNSALENO IS NULL OR tblCMKRet.FRETURNSALENO = ''
                         THEN ISNULL(tblCMKRetFine.FSETTLEAMOUNT,0) ELSE 0 END) AS GrossSales,
                SUM(CASE WHEN tblCMKRet.FRETURNSALENO IS NOT NULL AND tblCMKRet.FRETURNSALENO <> ''
                         THEN ISNULL(tblCMKRetFine.FSETTLEAMOUNT,0) ELSE 0 END) AS TotalReturns,
                SUM(CASE WHEN tblCMKRetFine.FSETTLEID = 4     THEN ISNULL(tblCMKRetFine.FSETTLEAMOUNT,0) ELSE 0 END) AS Voucher,
                SUM(CASE WHEN tblCMKRetFine.FSETTLEID = 5     THEN ISNULL(tblCMKRetFine.FSETTLEAMOUNT,0) ELSE 0 END) AS PickingCard,
                SUM(CASE WHEN tblCMKRetFine.FSETTLEID = 13    THEN ISNULL(tblCMKRetFine.FSETTLEAMOUNT,0) ELSE 0 END) AS LoyaltyPoints
            FROM CMK_LS_Retail tblCMKRet
            LEFT JOIN T_ORG_ORGANIZATIONS_L tblOrg
                ON tblCMKRet.FSALORGID = tblOrg.forgid AND tblOrg.flocaleid = '1033'
            LEFT JOIN CMK_LS_RetailFinE tblCMKRetFine
                ON tblCMKRet.FID = tblCMKRetFine.FID
            LEFT JOIN CMK_RSM_Cashier cas
                ON tblCMKRet.FCASHIER = cas.FID
            LEFT JOIN CMK_RSM_Cashier_L caslng
                ON cas.FID = caslng.FID AND caslng.FLocaleID = '1033'
            WHERE CAST(tblCMKRet.fdate AS DATETIME) >= @DateFrom
              AND CAST(tblCMKRet.fdate AS DATETIME) <= @DateTo
              AND tblOrg.fdescription = @Store
            GROUP BY
                tblOrg.fdescription,
                tblOrg.FNAME,
                CONVERT(varchar(10), tblCMKRet.fdate, 23),
                cas.FNUMBER,
                caslng.FRealName
            ORDER BY
                StoreCode,
                CashierFullName";

        using var conn = GetKdConnection();
        var rows = await conn.QueryAsync<KdSales>(sql, new { Store = store, DateFrom = dateFrom, DateTo = dateTo });
        return Ok(rows);
    }

    [HttpGet("contributions")]
    public async Task<IActionResult> GetContributions([FromQuery] string store, [FromQuery] string dateFrom, [FromQuery] string dateTo)
    {
        if (string.IsNullOrEmpty(store) || string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(dateTo))
            return BadRequest("store, dateFrom and dateTo are required");

        // TODO: Replace with real query once Robbie provides contributions table names
        var result = new List<KdContribution>();
        return Ok(result);
    }

    [HttpGet("journal")]
    public async Task<IActionResult> GetJournal([FromQuery] string store, [FromQuery] string dateFrom, [FromQuery] string dateTo)
    {
        if (string.IsNullOrEmpty(store) || string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(store))
            return BadRequest("store, dateFrom and dateTo are required");

        // TODO: Replace with real query once Robbie provides journal table names
        var result = new List<KdJournal>();
        return Ok(result);
    }
}