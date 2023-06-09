using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rxdev.Accounting.Model;

public class BankTransaction
    : Entity
{
    public int BankAccountId { get; set; }
    public BankAccount? BankAccount { get; set; }
    public DateTime SettledDate { get; set; }
    public string? Label { get; set; }
    public decimal Amount { get; set; }
    public List<Attachment> Attachments { get; set; } = new();
    public string TransactionId { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public string? Note { get; set; }
    public List<PurchaseEntry> PurchaseEntries { get; set; } = new();
    public List<RevenueEntry> RevenueEntries { get; set; } = new();

    public static void CreateModel(EntityTypeBuilder<BankTransaction> modelBuilder)
    {
        modelBuilder.HasIndex(e => e.TransactionId);
    }
}