using Microsoft.EntityFrameworkCore;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.App.Resources.MVVM;
using rxdev.Accounting.Model;
using System;
using System.Linq;
using System.Windows.Input;

namespace rxdev.Accounting.App.ViewModels;

public class BankTransactionEditViewModel
    : EditViewModel<BankTransaction, BankTransactionAdapter>
{
    private ICommand? _addCommand;

    public BankTransactionEditViewModel(
        IServiceProvider serviceProvider,
        RevenueEntryGridViewModel revenueEntryGridViewModel,
        PurchaseEntryGridViewModel purchaseEntryGridViewModel) 
        : base(serviceProvider)
    {
        Commands.ActionBar.HasAdd = true;

        RevenueEntryGridViewModel = revenueEntryGridViewModel;
        PurchaseEntryGridViewModel = purchaseEntryGridViewModel;
    }

    public RevenueEntryGridViewModel RevenueEntryGridViewModel { get; init; }
    public PurchaseEntryGridViewModel PurchaseEntryGridViewModel { get; init; }
    public ICommand AddCommand => _addCommand ??= new RelayCommand(OnAdd, CanAdd);

    protected override IQueryable<BankTransaction> GetQuery(bool tracking = false)
        => base.GetQuery(tracking)
        .Include(e => e.BankAccount)
        .Include(e => e.RevenueEntries)
        .ThenInclude(e => e.Invoice)
        .Include(e => e.PurchaseEntries)
        ;

    public override void Load(params object[] args)
    {
        base.Load(args);

        RevenueEntryGridViewModel.Load(Item);
        PurchaseEntryGridViewModel.Load(Item);
    }

    public override void Reload()
    {
        base.Reload();

        RevenueEntryGridViewModel.Reload();
        PurchaseEntryGridViewModel.Reload();
    }

    private bool CanAdd()
        => Item.Amount > 0
        ? Item.RevenueEntries.Sum(e => e.Amount) < Item.Amount && RevenueEntryGridViewModel.AddCommand.CanExecute(null)
        : Item.PurchaseEntries.Sum(e => e.Amount + e.VAT) < Math.Abs(Item.Amount) && PurchaseEntryGridViewModel.AddCommand.CanExecute(null);

    private void OnAdd()
    {
        if (Item.Amount > 0)
            RevenueEntryGridViewModel.AddCommand.Execute(null);
        else
            PurchaseEntryGridViewModel.AddCommand.Execute(null);
    }
}