using rxdev.Accounting.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace rxdev.Accounting.App.Adapters;

public class QuotationAdapter
    : EntityAdapter
{
    private int _customerId;
    private QuotationState _state;
    private string _number = string.Empty;
    private string? _title;
    private DateTime _issueDate = DateTime.Now;
    private CustomerAdapter? _customer;
    private ObservableCollection<InvoiceItemAdapter> _items = new();
    private int _index;
    private int _validityDays = 30;
    private int? _attachmentId;
    private decimal _total;
    private decimal _totalVAT;

    public decimal TotalVAT { get => _totalVAT; set => SetDirty(ref _totalVAT, value); }
    public decimal Total { get => _total; set => SetDirty(ref _total, value); }
    public int? AttachmentId { get => _attachmentId; set => Set(ref _attachmentId, value); }
    public int ValidityDays { get => _validityDays; set => SetDirty(ref _validityDays, value, raise: new string[] { nameof(ValidityDate) }); }
    public int Index { get => _index; set => Set(ref _index, value); }
    public ObservableCollection<InvoiceItemAdapter> Items { get => _items; set => SetDirty(ref _items, value); }
    public CustomerAdapter? Customer { get => _customer; set => SetDirty(ref _customer, value); }
    public DateTime ValidityDate => IssueDate + TimeSpan.FromDays(ValidityDays);
    public DateTime IssueDate { get => _issueDate; set => SetDirty(ref _issueDate, value, raise: new string[] { nameof(ValidityDate) }); }
    public string? Title { get => _title; set => SetDirty(ref _title, value); }
    public string Number { get => _number; set => SetDirty(ref _number, value); }
    public int CustomerId { get => _customerId; set => SetDirty(ref _customerId, value); }
    public QuotationState State { get => _state; set => SetDirty(ref _state, value); }
}