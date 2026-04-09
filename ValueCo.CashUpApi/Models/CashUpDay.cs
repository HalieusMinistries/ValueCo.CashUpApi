namespace ValueCo.CashUpApi.Models;

public class CashUpDay : ValueCo.CashUpApi.Data.IAuditable
{
    public int DayId { get; set; }
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public DateOnly CashUpDate { get; set; }

    public decimal FNB { get; set; }
    public decimal Surrender { get; set; }
    public decimal Floats { get; set; }
    public decimal ChangeBoxes { get; set; }
    public decimal LooseChange { get; set; }
    public decimal PettyCash { get; set; }

    public string? PettyNotes { get; set; }
    public string? CashUpNotes { get; set; }
    public string? CountingBanking { get; set; }

    public bool Submitted { get; set; }
    public DateTime? SubmittedAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<CashierRow> CashierRows { get; set; } = new List<CashierRow>();
    public ICollection<EFTDetail> EFTDetails { get; set; } = new List<EFTDetail>();
}