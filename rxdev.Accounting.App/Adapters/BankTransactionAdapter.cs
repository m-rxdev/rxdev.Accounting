using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace rxdev.Accounting.App.Adapters;

public class BankTransactionAdapter
    : EntityAdapter
{
    private int _bankAccountId;
    private BankAccountAdapter? _bankAccound;
    private string? _label;
    private string? _reference;
    private string? _note;
    private decimal _amount;
    private DateTime _settledDate;
    private ObservableCollection<RevenueEntryAdapter> _revenueEntries = new();
    private ObservableCollection<PurchaseEntryAdapter> _purchaseEntries = new();

    public ObservableCollection<PurchaseEntryAdapter> PurchaseEntries { get => _purchaseEntries; set => Set(ref _purchaseEntries, value); }
    public ObservableCollection<RevenueEntryAdapter> RevenueEntries { get => _revenueEntries; set => Set(ref _revenueEntries, value); }
    public DateTime SettledDate { get => _settledDate; set => SetDirty(ref _settledDate, value); }
    public decimal Amount { get => _amount; set => SetDirty(ref _amount, value); }
    public string? Note { get => _note; set => SetDirty(ref _note, value); }
    public string? Reference { get => _reference; set => SetDirty(ref _reference, value); }
    public string? Label { get => _label; set => SetDirty(ref _label, value); }
    public BankAccountAdapter? BankAccount { get => _bankAccound; set => SetDirty(ref _bankAccound, value); }
    public int BankAccountId { get => _bankAccountId; set => SetDirty(ref _bankAccountId, value); }
    public bool IsCredit => Amount > 0;
    public bool IsDebit => Amount < 0;
    public bool IsFullyAssociated => Amount == (IsCredit
        ? RevenueEntries.Sum(e => e.Amount)
        : - PurchaseEntries.Sum(e => e.Amount + e.VAT));
    public string Associations => IsCredit
        ? string.Join(", ", RevenueEntries.Select(e => e.Invoice?.Number))
        : string.Join(", ", PurchaseEntries.Select(e => e.Attachment!.FileName));
    public decimal VAT => IsCredit
        ? RevenueEntries.Sum(e => e.Invoice!.TotalVAT)
        : - PurchaseEntries.Sum(e => e.VAT);
}