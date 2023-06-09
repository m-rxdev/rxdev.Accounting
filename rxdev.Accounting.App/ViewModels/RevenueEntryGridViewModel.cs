using Microsoft.EntityFrameworkCore;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;
using System;
using System.Linq;

namespace rxdev.Accounting.App.ViewModels;

public class RevenueEntryGridViewModel
    : DateFilteredGridViewModel<RevenueEntry, RevenueEntryAdapter>
{
    private int? _invoiceId;
    private int? _bankTransactionId;

    public RevenueEntryGridViewModel(IServiceProvider serviceProvider) 
        : base(serviceProvider, e => e.BankTransaction!.SettledDate)
    {
        Commands.ActionBar.HasAdd = false;
    }

    public int? BankTransactionId { get => _bankTransactionId; set => Set(ref _bankTransactionId, value); }
    public int? InvoiceId { get => _invoiceId; set => Set(ref _invoiceId, value); }

    public override void Load(params object[] args)
    {
        BankTransactionId = args.GetArg<BankTransactionAdapter>(0)?.Id;
        InvoiceId = args.GetArg<InvoiceAdapter>(0)?.Id;

        base.Load(args);
    }

    protected override IQueryable<RevenueEntry> GetQuery(bool tracking = false)
    {
        IQueryable<RevenueEntry> query = base.GetQuery(tracking)
            .Include(e => e.Invoice)
            .Include(e => e.BankTransaction);

        if (InvoiceId.HasValue)
            query = query.Where(e => e.InvoiceId == InvoiceId.Value);

        if (BankTransactionId.HasValue)
            query = query.Where(e => e.BankTransactionId == BankTransactionId.Value);

        return query;
    }

    protected override void OnAdd()
    {
        RevenueEntry entity = new();

        if(BankTransactionId.HasValue)
            entity.BankTransactionId = BankTransactionId.Value;

        if (InvoiceId.HasValue)
            entity.InvoiceId = InvoiceId.Value;

        NavigationService.NavigateToEdit<RevenueEntry, RevenueEntryAdapter>(entity);
    }
}