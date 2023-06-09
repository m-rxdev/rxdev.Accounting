namespace rxdev.Accounting.Model;

public class TaxReporting
    : Entity
{
    public int TaxId { get; set; }
    public Tax? Tax { get; set; }
}