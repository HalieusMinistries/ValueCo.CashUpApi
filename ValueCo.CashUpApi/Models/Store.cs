namespace ValueCo.CashUpApi.Models;

public class Store
{
    public int StoreId { get; set; }
    public string StoreCode { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public string BankAccount { get; set; } = string.Empty;
    public string SpeedPointId { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<CashUpDay> CashUpDays { get; set; } = new List<CashUpDay>();
}