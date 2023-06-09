using Microsoft.EntityFrameworkCore;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;
using System;
using System.Linq;

namespace rxdev.Accounting.App.ViewModels;

public class PurchaseEntryGridViewModel
    : DateFilteredGridViewModel<PurchaseEntry, PurchaseEntryAdapter>
{
    private int? _bankTransactionId;

    public PurchaseEntryGridViewModel(IServiceProvider serviceProvider) 
        : base(serviceProvider, e => e.BankTransaction!.SettledDate)
    {
        Commands.ActionBar.HasAdd = false;
    }

    public int? BankTransactionId { get => _bankTransactionId; set => Set(ref _bankTransactionId, value); }

    public override void Load(params object[] args)
    {
        BankTransactionId = args.GetArg<BankTransactionAdapter>(0)?.Id;

        base.Load(args);
    }

    protected override IQueryable<PurchaseEntry> GetQuery(bool tracking = false)
    {
        IQueryable<PurchaseEntry> query = base.GetQuery(tracking)
        .Include(e => e.BankTransaction)
        .Include(e => e.Attachment);

        if (BankTransactionId.HasValue)
            query = query.Where(e => e.BankTransactionId == BankTransactionId.Value);

        return query;
    }

    protected override void OnAdd()
    {
        PurchaseEntry entity = new();

        if (BankTransactionId.HasValue)
            entity.BankTransactionId = BankTransactionId.Value;

        NavigationService.NavigateToEdit<PurchaseEntry, PurchaseEntryAdapter>(entity);
    }
}