using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.App.Resources.MVVM;
using rxdev.Accounting.Banking.Qonto;
using rxdev.Accounting.Model;
using rxdev.Accounting.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace rxdev.Accounting.App.ViewModels;

public class BankTransactionGridViewModel
    : DateFilteredGridViewModel<BankTransaction, BankTransactionAdapter>
{
    private ICommand? _syncCommand;

    public BankTransactionGridViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider, e => e.SettledDate)
    {
        Commands.ActionBar.HasAdd = false;
        Commands.ActionBar.HasSync = true;
    }
    public ICommand SyncCommand => _syncCommand ??= new RelayCommand(OnSync, CanSync);

    protected override IQueryable<BankTransaction> GetQuery(bool tracking = false)
        => base
        .GetQuery(tracking)
        .Include(e => e.BankAccount)
        .Include(e => e.RevenueEntries)
        .ThenInclude(e => e.Invoice)
        .Include(e => e.PurchaseEntries)
        .ThenInclude(e => e.Attachment);

    private void OnSync()
    {
        QontoClient client = ServiceProvider.GetRequiredService<QontoClient>();

        foreach (int id in ServiceProvider.GetRequiredService<Repository<BankAccount>>().AsQueryable().Select(e => e.Id).ToArray())
        {
            try
            {
                DateTime date = DateTime.Now;
                using IServiceScope scope = ServiceProvider.CreateScope();
                UnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
                Repository<BankAccount> bankAccountRespository = scope.ServiceProvider.GetRequiredService<Repository<BankAccount>>();
                Repository<BankTransaction> transactionRepository = scope.ServiceProvider.GetRequiredService<Repository<BankTransaction>>();

                BankAccount bankAccount = bankAccountRespository.Get(id, true);

                if (bankAccount.IBAN is null
                    || bankAccount.ApiInfo is null)
                    continue;

                List<BankTransaction> transactions = client.GetTransactions(bankAccount.IBAN, bankAccount.ApiInfo, bankAccount.LastSyncDate, date);
                string[] ids = transactions.Select(t => t.TransactionId).ToArray();
                string[] excludeids = (from ent in transactionRepository.AsQueryable()
                                       where ids.Contains(ent.TransactionId)
                                       select ent.TransactionId).ToArray();
                transactions = transactions.ExceptBy(excludeids, e => e.TransactionId).ToList();

                foreach (BankTransaction transaction in transactions)
                    transaction.BankAccountId = bankAccount.Id;

                transactionRepository.AddRange(transactions);

                bankAccount.LastSyncDate = date;
                unitOfWork.Save();
            }
            catch { }
        }

        Reload();
    }

    private bool CanSync()
        => true;
}