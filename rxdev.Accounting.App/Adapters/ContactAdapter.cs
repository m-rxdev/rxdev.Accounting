namespace rxdev.Accounting.App.Adapters;

public class ContactAdapter
    : EntityAdapter
{
    private string? _email;
    private string _name = "Contact Name";
    private string? _phoneNumber;
    private string? _position;
    private int _customerId;
    private bool _sendInvoice;
    private bool _sendQuotation;

    public string? Email { get => _email; set => SetDirty(ref _email, value); }
    public string Name { get => _name; set => SetDirty(ref _name, value); }
    public string? PhoneNumber { get => _phoneNumber; set => SetDirty(ref _phoneNumber, value); }
    public string? Position { get => _position; set => SetDirty(ref _position, value); }
    public int CustomerId { get => _customerId; set => SetDirty(ref _customerId, value); }
    public bool SendInvoice { get => _sendInvoice; set => SetDirty(ref _sendInvoice, value); }
    public bool SendQuotation { get => _sendQuotation; set => SetDirty(ref _sendQuotation, value); }
}