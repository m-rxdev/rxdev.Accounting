namespace rxdev.Accounting.App.Adapters;

public class CustomerAdapter
    : EntityAdapter
{
    private string? _address;
    private string _name = "Company Name";
    private string? _siret;
    private string? _vat;
    private string? _website;

    public string? Address { get => _address; set => SetDirty(ref _address, value); }
    public string Name { get => _name; set => SetDirty(ref _name, value); }
    public string? SIRET { get => _siret; set => SetDirty(ref _siret, value); }
    public string? VAT { get => _vat; set => SetDirty(ref _vat, value); }
    public string? Website { get => _website; set => SetDirty(ref _website, value); }
}