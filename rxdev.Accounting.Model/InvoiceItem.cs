using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rxdev.Accounting.Model;

public class InvoiceItem
    : Entity
{
    public string? Description { get; set; }
    public Invoice? Invoice { get; set; }
    public int? InvoiceId { get; set; }
    public decimal Price { get; set; }
    public double Quantity { get; set; } = 1;
    public Quotation? Quotation { get; set; }
    public int? QuotationId { get; set; }
    public string? Title { get; set; }
    public InvoiceItemUnit Unit { get; set; }
    public double VATRate { get; set; } = 0.2;

    public static void CreateModel(EntityTypeBuilder<InvoiceItem> modelBuilder)
    {
        //modelBuilder.Property(e => e.Price).HasConversion(v => (int)(v * 10), v => ((decimal)v) / 10);
    }
}

public enum InvoiceItemUnit
{
    Day,
    Hour,
    Unit,
    Click,
    Page,
    Line,
    Word,
    Character,
}