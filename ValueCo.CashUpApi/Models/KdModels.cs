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
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string BillNo { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public decimal ReceiptAmount { get; set; }
    public decimal PaymentAmount { get; set; }
    public decimal Balance { get; set; }
}

public class KdSales
{
    public string StoreCode { get; set; } = string.Empty;
    public string CashUpDate { get; set; } = string.Empty;
    public string CashierFullName { get; set; } = string.Empty;
    public decimal Cash { get; set; }
    public decimal EFT { get; set; }
    public decimal Card { get; set; }
    public decimal Rounding { get; set; }
    public decimal GrossSales { get; set; }
    public decimal TotalReturns { get; set; }
    public decimal Voucher { get; set; }
    public decimal PickingCard { get; set; }
    public decimal LoyaltyPoints { get; set; }
}