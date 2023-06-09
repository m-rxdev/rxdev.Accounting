using System;
using System.Collections.ObjectModel;

namespace rxdev.Accounting.App.Adapters;

public class BankAccountAdapter
    : EntityAdapter
{
    private string? _apiInfo;
    private string? _bank;
    private string? _bic;
    private string? _iban;
    private string? _label;
    private DateTime? _lastSyncDate;
    private DateTime _openDate;
    private string? _rib;
    private ObservableCollection<BankTransactionAdapter> _transactions = new();

    public string? ApiInfo { get => _apiInfo; set => SetDirty(ref _apiInfo, value); }
    public string? Bank { get => _bank; set => SetDirty(ref _bank, value); }
    public string? BIC { get => _bic; set => SetDirty(ref _bic, value); }
    public string? IBAN { get => _iban; set => SetDirty(ref _iban, value); }
    public string? Label { get => _label; set => SetDirty(ref _label, value); }
    public DateTime? LastSyncDate { get => _lastSyncDate; set => SetDirty(ref _lastSyncDate, value); }
    public DateTime OpenDate { get => _openDate; set => SetDirty(ref _openDate, value); }
    public string? RIB { get => _rib; set => SetDirty(ref _rib, value); }
    public ObservableCollection<BankTransactionAdapter> Transactions { get => _transactions; set => SetDirty(ref _transactions, value); }
}