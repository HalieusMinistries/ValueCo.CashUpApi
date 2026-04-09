namespace ValueCo.CashUpApi.DTOs;

public class CashierRowDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Cash { get; set; }
    public decimal Card { get; set; }
    public decimal EFT { get; set; }
    public decimal Erase { get; set; }
    public decimal Returns { get; set; }
    public decimal Gift { get; set; }
    public decimal Coupon { get; set; }
    public decimal Loyalty { get; set; }
}

public class EFTDetailDto
{
    public string Date { get; set; } = string.Empty;
    public string? SONumber { get; set; }
    public decimal Amount { get; set; }
}

public class CashUpSubmitDto
{
    public string StoreCode { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
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
    public List<CashierRowDto> Cashiers { get; set; } = new();
    public List<EFTDetailDto> EFTDetails { get; set; } = new();
}