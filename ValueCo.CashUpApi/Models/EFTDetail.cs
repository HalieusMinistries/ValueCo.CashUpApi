namespace ValueCo.CashUpApi.Models;

public class EFTDetail
{
    public int EFTId { get; set; }
    public int DayId { get; set; }
    public CashUpDay CashUpDay { get; set; } = null!;

    public DateOnly EFTDate { get; set; }
    public string? SONumber { get; set; }
    public decimal Amount { get; set; }
}