using System;

namespace rxdev.Accounting.App.Adapters;

public class CompanyInfoAdapter
    : EntityAdapter
{
    private string? _activity;
    private string? _address;
    private string? _apeCode;
    private string? _invoiceCustomFooter;
    private string? _invoiceCustomHeader;
    private int _invoiceIndex;
    private string _invoiceNumberingFormat = "F-{0:yyyyMM}-{1:D3}";
    private string? _legalStatus;
    private string? _mail;
    private string _name = string.Empty;
    private string? _phoneNumber;
    private string? _quotationCustomFooter;
    private string? _quotationCustomHeader;
    private int _quotationIndex;
    private string _quotationNumberingFormat = "D-{0:yyyyMM}-{1:D3}";
    private string? _siren;
    private string? _siret;
    private string? _vat;
    private string? _website;
    private DateTime _creationDate;

    public DateTime CreationDate { get => _creationDate; set => SetDirty(ref _creationDate, value); }
    public string? Activity { get => _activity; set => SetDirty(ref _activity, value); }
    public string? Address { get => _address; set => SetDirty(ref _address, value); }
    public string? APECode { get => _apeCode; set => SetDirty(ref _apeCode, value); }
    public string? InvoiceCustomFooter { get => _invoiceCustomFooter; set => SetDirty(ref _invoiceCustomFooter, value); }
    public string? InvoiceCustomHeader { get => _invoiceCustomHeader; set => SetDirty(ref _invoiceCustomHeader, value); }
    public int InvoiceIndex { get => _invoiceIndex; set => SetDirty(ref _invoiceIndex, value); }
    public string InvoiceNumberingFormat { get => _invoiceNumberingFormat; set => SetDirty(ref _invoiceNumberingFormat, value); }
    public string? LegalStatus { get => _legalStatus; set => SetDirty(ref _legalStatus, value); }
    public string? Mail { get => _mail; set => SetDirty(ref _mail, value); }
    public string Name { get => _name; set => SetDirty(ref _name, value); }
    public string? PhoneNumber { get => _phoneNumber; set => SetDirty(ref _phoneNumber, value); }
    public string? QuotationCustomFooter { get => _quotationCustomFooter; set => SetDirty(ref _quotationCustomFooter, value); }
    public string? QuotationCustomHeader { get => _quotationCustomHeader; set => SetDirty(ref _quotationCustomHeader, value); }
    public int QuotationIndex { get => _quotationIndex; set => SetDirty(ref _quotationIndex, value); }
    public string QuotationNumberingFormat { get => _quotationNumberingFormat; set => SetDirty(ref _quotationNumberingFormat, value); }
    public string? SIREN { get => _siren; set => SetDirty(ref _siren, value); }
    public string? SIRET { get => _siret; set => SetDirty(ref _siret, value); }
    public string? VAT { get => _vat; set => SetDirty(ref _vat, value); }
    public string? Website { get => _website; set => SetDirty(ref _website, value); }
}