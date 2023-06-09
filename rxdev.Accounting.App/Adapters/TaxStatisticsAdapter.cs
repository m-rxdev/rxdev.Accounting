using rxdev.Accounting.Model;

namespace rxdev.Accounting.App.Adapters;

public class TaxStatisticsAdapter
    : Adapter
{
    private Tax? _tax;
    private decimal _result;

    public decimal Result { get => _result; set => Set(ref _result, value); }
    public Tax? Tax { get => _tax; set => Set(ref _tax, value); }
}