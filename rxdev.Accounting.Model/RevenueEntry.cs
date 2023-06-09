namespace rxdev.Accounting.Model;

public class RevenueEntry
    : Entity
{
    public decimal Amount { get; set; }
    public int BankTransactionId { get; set; }
    public BankTransaction? BankTransaction { get; set; }
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
}