namespace rxdev.Accounting.App.Adapters;

public class RevenueEntryAdapter
    : EntityAdapter
{
    private decimal _amount;
    private BankTransactionAdapter? _bankTransaction;
    private int _bankTransactionId;
    private InvoiceAdapter? _invoice;
    private int _invoiceId;

    public decimal Amount { get => _amount; set => SetDirty(ref _amount, value); }
    public BankTransactionAdapter? BankTransaction { get => _bankTransaction; set => SetDirty(ref _bankTransaction, value); }
    public int BankTransactionId { get => _bankTransactionId; set => SetDirty(ref _bankTransactionId, value); }
    public InvoiceAdapter? Invoice { get => _invoice; set => SetDirty(ref _invoice, value); }
    public int InvoiceId { get => _invoiceId; set => SetDirty(ref _invoiceId, value); }
}