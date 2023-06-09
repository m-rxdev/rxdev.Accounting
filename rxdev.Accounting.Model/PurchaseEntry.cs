namespace rxdev.Accounting.Model;

public class PurchaseEntry
    : Entity
{
    public int BankTransactionId { get; set; }
    public BankTransaction? BankTransaction { get; set; }
    public decimal Amount { get; set; }
    public decimal VAT { get; set; }
    public int? AttachmentId { get; set; }
    public Attachment? Attachment { get; set; }
    public string? Vendor { get; set; }
    public string? Description { get; set; }
}