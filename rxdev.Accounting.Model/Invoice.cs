namespace rxdev.Accounting.Model;

public class Invoice
    : Entity
{
    public List<InvoiceItem> Items { get; set; } = new();
    public List<RevenueEntry> RevenueEntries { get; set; } = new();

    public InvoiceState State { get; set; } = InvoiceState.Draft;
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int? AttachmentId { get; set; }
    public Attachment? Attachment { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.Now;
    public DateTime ExecutionDate { get; set; } = DateTime.Now;
    public string Number { get; set; } = string.Empty;
    public string? Title { get; set; }
    public int Index { get; set; }
    public decimal Total { get; set; }
    public decimal TotalVAT { get; set; }
}

public enum InvoiceState
{
    Draft,
    Locked,
    Imported,
}