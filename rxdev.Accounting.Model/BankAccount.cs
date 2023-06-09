namespace rxdev.Accounting.Model;

public class BankAccount
    : Entity
{
    public DateTime OpenDate { get; set; } = DateTime.Now;
    public DateTime? LastSyncDate { get; set; }
    public string? Bank { get; set; }
    public string? IBAN { get; set; }
    public string? BIC { get; set; }
    public string? RIB { get; set; }
    public string? Label { get; set; }
    public string? ApiInfo { get; set; }
    public List<BankTransaction> Transactions { get; set; } = new();
    public List<RevenueEntry> RevenueEntries { get; set; } = new();
    public List<PurchaseEntry> PurchaseEntries { get; set; } = new();
}