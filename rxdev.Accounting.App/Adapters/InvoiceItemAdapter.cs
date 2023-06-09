using rxdev.Accounting.Model;

namespace rxdev.Accounting.App.Adapters;

public class InvoiceItemAdapter
    : EntityAdapter
{
    private int? _invoiceId;
    private int? _quotationId;
    private InvoiceItemUnit _unit;
    private double _quantity;
    private decimal _price;
    private double _vatRate = 0.2;
    private string? _title;
    private string? _description;

    public string? Description { get => _description; set => SetDirty(ref _description, value); }
    public string? Title { get => _title; set => SetDirty(ref _title, value); }
    public double VATRate { get => _vatRate; set => SetDirty(ref _vatRate, value); }
    public decimal Price { get => _price; set => SetDirty(ref _price, value); }
    public double Quantity { get => _quantity; set => SetDirty(ref _quantity, value); }
    public InvoiceItemUnit Unit { get => _unit; set => SetDirty(ref _unit, value); }
    public int? QuotationId { get => _quotationId; set => SetDirty(ref _quotationId, value); }
    public int? InvoiceId { get => _invoiceId; set => SetDirty(ref _invoiceId, value); }
}