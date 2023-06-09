using Microsoft.Extensions.DependencyInjection;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;
using rxdev.Accounting.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace rxdev.Accounting.App.ViewModels;

public class RevenueEntryEditViewModel
    : EditViewModel<RevenueEntry, RevenueEntryAdapter>
{
    private bool _canSelectBankTransaction;
    private bool _canSelectInvoice;
    private decimal _maxAmount;
    private ObservableCollection<InvoiceAdapter> _invoices = new();
    private ObservableCollection<BankTransactionAdapter> _bankTransactions = new();

    public RevenueEntryEditViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
    { }

    public ObservableCollection<BankTransactionAdapter> BankTransactions { get => _bankTransactions; set => Set(ref _bankTransactions, value); }
    public ObservableCollection<InvoiceAdapter> Invoices { get => _invoices; set => Set(ref _invoices, value); }
    public bool CanSelectInvoice { get => _canSelectInvoice; set => Set(ref _canSelectInvoice, value); }
    public bool CanSelectBankTransaction { get => _canSelectBankTransaction; set => Set(ref _canSelectBankTransaction, value); }
    public decimal MaxAmount { get => _maxAmount; set => Set(ref _maxAmount, value); }

    public override void Load(params object[] args)
    {
        base.Load(args);

        CanSelectBankTransaction = Item.BankTransactionId == 0;
        CanSelectInvoice = Item.InvoiceId == 0;

        Repository<Invoice> invoiceRepository = ServiceProvider.GetRequiredService<Repository<Invoice>>();
        Repository<BankTransaction> bankTransactionRepository = ServiceProvider.GetRequiredService<Repository<BankTransaction>>();

        Invoices = new ObservableCollection<InvoiceAdapter>(
            Mapper.Map<IEnumerable<InvoiceAdapter>>(
                invoiceRepository
                .AsQueryable()
                .Where(e => e.Id == Item.InvoiceId || e.RevenueEntries.Sum(r => r.Amount) < e.Total + e.TotalVAT)
                .OrderByDescending(e => e.IssueDate)));

        BankTransactions = new ObservableCollection<BankTransactionAdapter>(
            Mapper.Map<IEnumerable<BankTransactionAdapter>>(
                bankTransactionRepository
                .AsQueryable()
                .Where(e => e.Id == Item.BankTransactionId || (e.Amount > 0 && e.RevenueEntries.Sum(r => r.Amount) < e.Amount))
                .OrderByDescending(e => e.SettledDate)));

        if(Item.BankTransactionId != 0)
            MaxAmount = BankTransactions.First(e => e.Id == Item.BankTransactionId).Amount;
        if (Item.InvoiceId != 0)
        {
            InvoiceAdapter adapter = Invoices.First(e => e.Id == Item.InvoiceId);
            MaxAmount = adapter.Total + adapter.TotalVAT;
        }
    }
}