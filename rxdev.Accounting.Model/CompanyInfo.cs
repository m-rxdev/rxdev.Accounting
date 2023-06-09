namespace rxdev.Accounting.Model;

public class CompanyInfo
    : Entity
{
    public DateTime CreationDate { get; set; }
    public string? Address { get; set; }
    public string? Mail { get; set; }
    public string Name { get; set; } = "Company Name";
    public string? SIREN { get; set; }
    public string? SIRET { get; set; }
    public string? VAT { get; set; }
    public string? Website { get; set; }
    public string? APECode { get; set; }
    public string? Activity { get; set; }
    public string? PhoneNumber { get; set; }
    public string? LegalStatus { get; set; }
    public string? QuotationCustomHeader { get; set; }
    public string? InvoiceCustomHeader { get; set; }
    public string? QuotationCustomFooter { get; set; }
    public string? InvoiceCustomFooter { get; set; }
    public int QuotationIndex { get; set; }
    public int InvoiceIndex { get; set; }
    public string QuotationNumberingFormat { get; set; } = "D-{0:yyyyMM}-{1:D3}";
    public string InvoiceNumberingFormat { get; set; } = "F-{0:yyyyMM}-{1:D3}";
}