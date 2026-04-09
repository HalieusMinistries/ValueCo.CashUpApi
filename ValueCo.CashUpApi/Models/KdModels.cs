namespace ValueCo.CashUpApi.Models;

public class KdContribution
{
    public string Date { get; set; } = string.Empty;
    public string StoreCode { get; set; } = string.Empty;
    public string Cashier { get; set; } = string.Empty;
    public string SettlementMode { get; set; } = string.Empty;
    public decimal ContributionAmount { get; set; }
    public decimal PettyCash { get; set; }
    public decimal Difference { get; set; }
    public string Remark { get; set; } = string.Empty;
}

public class KdJournal
{
    public string Date { get; set; } = string.Empty;
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal PrevBalance { get; set; }
    public decimal RevenueToday { get; set; }
    public decimal ExpenseToday { get; set; }
    public decimal BalanceToday { get; set; }
}

public class KdSales
{
    public string Date { get; set; } = string.Empty;
    public string Cashier { get; set; } = string.Empty;
    public decimal Cash { get; set; }
    public decimal Card { get; set; }
    public decimal EFT { get; set; }
    public decimal Erase { get; set; }
    public decimal GrossTotal { get; set; }
    public decimal Returns { get; set; }
    public decimal GiftVoucher { get; set; }
    public decimal Coupon { get; set; }
    public decimal Loyalty { get; set; }
    public decimal FNBDeposit { get; set; }
    public decimal Surrender { get; set; }
    public decimal Floats { get; set; }
    public decimal ChangeGiven { get; set; }
    public decimal PettyCash { get; set; }
}