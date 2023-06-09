namespace rxdev.Accounting.App.Adapters;

public class VATStatisticsAdapter
    : Adapter
{
    private decimal vatIn;
    private decimal vatOut;

    public decimal VATIn { get => vatIn; set => Set(ref vatIn, value, raise: new string[] { nameof(Result) }); }
    public decimal VATOut { get => vatOut; set => Set(ref vatOut, value, raise: new string[] { nameof(Result) }); }
    public decimal Result => VATIn - VATOut;
}