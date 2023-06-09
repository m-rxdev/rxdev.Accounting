using Microsoft.Extensions.DependencyInjection;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;
using rxdev.Accounting.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace rxdev.Accounting.App.ViewModels;

public class DashboardViewModel
    : ViewModel
{
    private int _monthSpan = 3;
    private PeriodStatisticsAdapter? _yearStatistics;
    private ObservableCollection<PeriodStatisticsAdapter> _periodStatistics = new();

    public DashboardViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }

    public int MonthSpan { get => _monthSpan; set => Set(ref _monthSpan, value, action: Reload); }
    public ObservableCollection<PeriodStatisticsAdapter> PeriodStatistics { get => _periodStatistics; set => Set(ref _periodStatistics, value); }
    public PeriodStatisticsAdapter? YearStatistics { get => _yearStatistics; set => Set(ref _yearStatistics, value); }

    public override void Reload()
    {
        Tax[] taxes = ServiceProvider.GetRequiredService<Repository<Tax>>().AsQueryable().ToArray();
        int year = NavigationService.SelectedYear;

        YearStatistics = new PeriodStatisticsAdapter()
        {
            Title = year.ToString(),
            Start = new DateTime(year, 1, 1),
            End = new DateTime(year + 1, 1, 1),
        };

        PeriodStatistics.Clear();
        PeriodStatistics.Add(YearStatistics);
        int start = 0;
        while (start < 12)
        {
            int end = Math.Min(12, start + MonthSpan);
            PeriodStatistics.Add(new()
            {
                Title = string.Join("/", Enumerable.Range(start + 1, end - start).Select(j => new DateTime(year, j, 1).ToString("MMM", CultureInfo.InvariantCulture))),
                Start = new DateTime(year, start + 1, 1),
                End = new DateTime(year + (end / 12), 1 + end % 12, 1),
            });
            start = end;
        }

        Repository<RevenueEntry> revenueRepository = ServiceProvider.GetRequiredService<Repository<RevenueEntry>>();
        Repository<PurchaseEntry> purchaseRepository = ServiceProvider.GetRequiredService<Repository<PurchaseEntry>>();

        foreach (PeriodStatisticsAdapter adapter in new List<PeriodStatisticsAdapter>{ YearStatistics }.Concat(PeriodStatistics))
        {
            adapter.IsLocked = DateTime.Now > adapter.End;
            adapter.VATStatistics.VATIn = revenueRepository
                .AsQueryable()
                .Where(e => e.BankTransaction!.SettledDate >= adapter.Start && e.BankTransaction.SettledDate < adapter.End)
                .Sum(e => e.Invoice!.TotalVAT * e.Amount / (e.Invoice!.Total + e.Invoice!.TotalVAT));
            adapter.VATStatistics.VATOut = purchaseRepository
                .AsQueryable()
                .Where(e => e.BankTransaction!.SettledDate >= adapter.Start && e.BankTransaction.SettledDate < adapter.End)
                .Sum(e => e.VAT);
            adapter.Revenue = revenueRepository
                .AsQueryable()
                .Where(e => e.BankTransaction!.SettledDate >= adapter.Start && e.BankTransaction.SettledDate < adapter.End)
                .Sum(e => e.Invoice!.Total * e.Amount / (e.Invoice!.Total + e.Invoice!.TotalVAT));
            adapter.Purchase = purchaseRepository
                .AsQueryable()
                .Where(e => e.BankTransaction!.SettledDate >= adapter.Start && e.BankTransaction.SettledDate < adapter.End)
                .Sum(e => e.Amount);

            adapter.TaxStatistics.Clear();
            foreach (Tax tax in taxes)
            {
                adapter.TaxStatistics.Add(new TaxStatisticsAdapter
                {
                    Tax = tax,
                    Result = (decimal)tax.Rate * adapter.Revenue,
                });
            }
        }
    }
}