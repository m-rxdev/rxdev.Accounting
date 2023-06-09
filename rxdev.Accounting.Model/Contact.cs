namespace rxdev.Accounting.Model;

public class Contact
    : Entity
{
    public Customer? Customer { get; set; }
    public int CustomerId { get; set; }
    public string Name { get; set; } = "Contact Name";
    public string? Position { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool SendInvoice { get; set; }
    public bool SendQuotation { get; set; }
}