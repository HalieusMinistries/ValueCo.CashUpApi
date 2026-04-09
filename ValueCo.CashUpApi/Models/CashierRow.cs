namespace ValueCo.CashUpApi.Models;

public class CashierRow
{
    public int RowId { get; set; }
    public int DayId { get; set; }
    public CashUpDay CashUpDay { get; set; } = null!;

    public string CashierName { get; set; } = string.Empty;
    public decimal Cash { get; set; }
    public decimal Card { get; set; }
    public decimal EFT { get; set; }
    public decimal Erase { get; set; }
    public decimal Returns { get; set; }
    public decimal Gift { get; set; }
    public decimal Coupon { get; set; }
    public decimal Loyalty { get; set; }
}