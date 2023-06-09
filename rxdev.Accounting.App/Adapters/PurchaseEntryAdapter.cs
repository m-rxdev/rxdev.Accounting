namespace rxdev.Accounting.App.Adapters;

public class PurchaseEntryAdapter
    : EntityAdapter
{
    private decimal _amount;
    private decimal _vat;
    private int _bankTransactionId;
    private int? _attachmentId;
    private AttachmentAdapter? _attachment;
    private BankTransactionAdapter? _bankTransaction;
    private string? _vendor;
    private string? _description;

    public string? Description { get => _description; set => SetDirty(ref _description, value); }
    public string? Vendor { get => _vendor; set => SetDirty(ref _vendor, value); }
    public BankTransactionAdapter? BankTransaction { get => _bankTransaction; set => SetDirty(ref _bankTransaction, value); }
    public AttachmentAdapter? Attachment { get => _attachment; set => SetDirty(ref _attachment, value); }
    public int? AttachmentId { get => _attachmentId; set => SetDirty(ref _attachmentId, value); }
    public int BankTransactionId { get => _bankTransactionId; set => SetDirty(ref _bankTransactionId, value); }
    public decimal VAT { get => _vat; set => SetDirty(ref _vat, value); }
    public decimal Amount { get => _amount; set => SetDirty(ref _amount, value); }
}