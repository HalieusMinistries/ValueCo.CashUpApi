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

    const string sql = @"
        WITH CTE_SystemRetail AS (
            SELECT 
                CAST(t1.FDate AS DATE) AS ReconDate,
                t1.FSALORGID AS OrgID,
                t1.FCashier AS CashierID,
                t2.FSettleID AS SettleModeID,
                MAX(t2.FRate) AS ExchRate,
                SUM(ISNULL(t2.FSettleAmount, 0)) AS CollectOrig, 
                SUM(ISNULL(t2.FLocSettleAmt, 0)) AS CollectFunc  
            FROM CMK_LS_Retail t1 WITH (NOLOCK)
            INNER JOIN CMK_LS_RetailFinE t2 WITH (NOLOCK) 
                ON t1.FID = t2.FID
            WHERE t1.FDOCUMENTSTATUS = 'C'
              AND CAST(t1.FDate AS DATE) >= @DateFrom
              AND CAST(t1.FDate AS DATE) <= @DateTo
            GROUP BY 
                CAST(t1.FDate AS DATE), 
                t1.FSALORGID, 
                t1.FCashier, 
                t2.FSettleID
        ),
        CTE_ManualCashUp AS (
            SELECT 
                CAST(p.FPAYDATE AS DATE) AS ReconDate,
                p.FSALORGID AS OrgID,
                p.FCASHIER AS CashierID,
                pe.FPAYMODE AS SettleModeID,
                SUM(ISNULL(pe.FPAYAMOUNT, 0)) AS ContribOrig, 
                SUM(ISNULL(pe.FPAYAMOUNT, 0)) AS ContribFunc,
                MAX(CAST(p.FMEMO AS VARCHAR(500))) AS OverallRemark, 
                MAX(CAST(pe.FREMARKS AS VARCHAR(500))) AS Remarks 
            FROM CMK_LS_Payment p WITH (NOLOCK)
            INNER JOIN CMK_LS_paymententry pe WITH (NOLOCK) 
                ON p.FID = pe.FID
            WHERE CAST(p.FPAYDATE AS DATE) >= @DateFrom
              AND CAST(p.FPAYDATE AS DATE) <= @DateTo
            GROUP BY 
                CAST(p.FPAYDATE AS DATE), 
                p.FSALORGID, 
                p.FCASHIER, 
                pe.FPAYMODE
        ),
        CTE_CombinedRecon AS (
            SELECT 
                ISNULL(r.ReconDate, p.ReconDate) AS ReconDate,
                ISNULL(r.OrgID, p.OrgID) AS OrgID,
                ISNULL(r.CashierID, p.CashierID) AS CashierID,
                ISNULL(r.SettleModeID, p.SettleModeID) AS SettleModeID,
                ISNULL(r.ExchRate, 1) AS ExchRate,
                ISNULL(r.CollectOrig, 0) AS CollectOrig,
                ISNULL(r.CollectFunc, 0) AS CollectFunc,
                ISNULL(p.ContribOrig, 0) AS ContribOrig,
                ISNULL(p.ContribFunc, 0) AS ContribFunc,
                p.OverallRemark,
                p.Remarks
            FROM CTE_SystemRetail r
            FULL OUTER JOIN CTE_ManualCashUp p
                ON r.ReconDate = p.ReconDate
                AND r.OrgID = p.OrgID
                AND r.CashierID = p.CashierID
                AND r.SettleModeID = p.SettleModeID
        )
        SELECT 
            CONVERT(VARCHAR(10), c.ReconDate, 23) AS Date,
            tblOrg.fdescription AS StoreCode,
            caslng.FRealName AS Cashier,
            CASE c.SettleModeID
                WHEN 1 THEN 'Cash'
                WHEN 2 THEN 'Bank Card'
                WHEN 4 THEN 'Voucher'
                WHEN 5 THEN 'Picking Card'
                WHEN 13 THEN 'Payment by points'
                WHEN 1000 THEN 'Erase'
                WHEN 10005 THEN 'EFT'
                ELSE CAST(c.SettleModeID AS VARCHAR(50)) 
            END AS SettlementMode,
            c.ContribOrig AS ContributionAmount,
            0 AS PettyCash,
            (c.CollectOrig - c.ContribOrig) AS Difference,
            ISNULL(c.Remarks, '') AS Remark
        FROM CTE_CombinedRecon c
        LEFT JOIN T_ORG_ORGANIZATIONS_L tblOrg WITH (NOLOCK)
            ON c.OrgID = tblOrg.forgid AND tblOrg.flocaleid = '1033'
        LEFT JOIN CMK_RSM_Cashier_L caslng WITH (NOLOCK)
            ON c.CashierID = caslng.FID AND caslng.FLocaleID = '1033'
        WHERE tblOrg.fdescription = @Store
        ORDER BY 
            c.ReconDate,
            tblOrg.fdescription,
            caslng.FRealName,
            SettlementMode";

    using var conn = GetKdConnection();
    var rows = await conn.QueryAsync<KdContribution>(sql, new { Store = store, DateFrom = dateFrom, DateTo = dateTo });
    return Ok(rows);
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