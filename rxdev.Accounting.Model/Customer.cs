namespace rxdev.Accounting.Model;

public class Customer
    : Entity
{
    public List<Invoice> Invoices { get; set; } = new();
    public List<Quotation> Quotations { get; set; } = new();
    public List<Contact> Contacts { get; set; } = new();
    public string? VAT { get; set; }
    public string? Address { get; set; }
    public string? SIRET { get; set; }
    public string Name { get; set; } = "Customer Name";
    public string? Website { get; set; }
}