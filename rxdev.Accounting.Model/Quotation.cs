using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rxdev.Accounting.Model;

public class Quotation
    : Entity
{
    public QuotationState State { get; set; }
    public List<InvoiceItem> Items { get; set; } = new();
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int? FileDataId { get; set; }
    public EntityData? FileData { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.Now;
    public DateTime ValidityDate { get; set; } = DateTime.Now + TimeSpan.FromDays(30);
    public string Number { get; set; } = string.Empty;
    public string? Title { get; set; }
    public int Index { get; set; }
}

public enum QuotationState
{
    Draft,
    Sent, 
    Accepted,
    Denied
}