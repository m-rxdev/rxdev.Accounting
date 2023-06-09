using System;
using System.Collections.ObjectModel;

namespace rxdev.Accounting.App.Adapters;

public class PeriodStatisticsAdapter
    : Adapter
{
    private string? _title;
    private ObservableCollection<TaxStatisticsAdapter> _taxStatistics = new();
    private VATStatisticsAdapter _vatStatistics = new();
    private DateTime _start;
    private DateTime _end;
    private decimal _revenue;
    private decimal _purchase;
    private bool _isLocked;

    public bool IsLocked { get => _isLocked; set => Set(ref _isLocked, value); }
    public decimal Purchase { get => _purchase; set => Set(ref _purchase, value); }
    public decimal Revenue { get => _revenue; set => Set(ref _revenue, value); }
    public DateTime End { get => _end; set => Set(ref _end, value); }
    public DateTime Start { get => _start; set => Set(ref _start, value); }
    public VATStatisticsAdapter VATStatistics { get => _vatStatistics; set => Set(ref _vatStatistics, value); }
    public ObservableCollection<TaxStatisticsAdapter> TaxStatistics { get => _taxStatistics; set => Set(ref _taxStatistics, value); }
    public string? Title { get => _title; set => Set(ref _title, value); }
}