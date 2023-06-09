namespace rxdev.Accounting.Model;

public class Tax
    : Entity
{
    public string Name { get; set; } = "Tax";
    public double Rate { get; set; }
    public List<TaxReporting> Reportings { get; set; } = new();
}