using rxdev.Accounting.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace rxdev.Accounting.App.Adapters;

public class InvoiceAdapter
    : EntityAdapter
{
    private const int PaymentDays = 30;

    private int _customerId;
    private InvoiceState _state;
    private string _number = string.Empty;
    private string? _title;
    private DateTime _issueDate = DateTime.Now;
    private CustomerAdapter? _customer;
    private ObservableCollection<InvoiceItemAdapter> _items = new();
    private int _index;
    private int _executionDays = 0;
    private int? _attachmentId;
    private decimal _total;
    private decimal _totalVAT;
    private ObservableCollection<RevenueEntryAdapter> _revenueEntries = new();
    private ObservableCollection<PurchaseEntryAdapter> _purchaseEntries = new();

    public ObservableCollection<PurchaseEntryAdapter> PurchaseEntries { get => _purchaseEntries; set => SetDirty(ref _purchaseEntries, value); }
    public ObservableCollection<RevenueEntryAdapter> RevenueEntries { get => _revenueEntries; set => SetDirty(ref _revenueEntries, value); }
    public decimal TotalVAT { get => _totalVAT; set => SetDirty(ref _totalVAT, value); }
    public decimal Total { get => _total; set => SetDirty(ref _total, value); }
    public int? AttachmentId { get => _attachmentId; set => Set(ref _attachmentId, value); }
    public int ExecutionDays { get => _executionDays; set => SetDirty(ref _executionDays, value, raise: new string[] { nameof(ExecutionDays) }); }
    public int Index { get => _index; set => Set(ref _index, value); }
    public ObservableCollection<InvoiceItemAdapter> Items { get => _items; set => SetDirty(ref _items, value); }
    public CustomerAdapter? Customer { get => _customer; set => SetDirty(ref _customer, value); }
    public DateTime ExecutionDate => IssueDate + TimeSpan.FromDays(ExecutionDays);
    public DateTime IssueDate { get => _issueDate; set => SetDirty(ref _issueDate, value, raise: new string[] { nameof(ExecutionDays) }); }
    public string? Title { get => _title; set => SetDirty(ref _title, value); }
    public string Number { get => _number; set => SetDirty(ref _number, value); }
    public int CustomerId { get => _customerId; set => SetDirty(ref _customerId, value); }
    public InvoiceState State { get => _state; set => SetDirty(ref _state, value); }
    public bool IsFullyPaid => RevenueEntries.Sum(e => e.Amount) >= Total + TotalVAT;
    public bool IsLate => !IsFullyPaid && ProgressDays > PaymentDays;
    public int LateDays => (DateTime.Now - (ExecutionDate + TimeSpan.FromDays(PaymentDays))).Days;
    public double Progress => (DateTime.Now - ExecutionDate).Days / (double)PaymentDays;
    public int ProgressDays => (DateTime.Now - ExecutionDate).Days;
    public double PaymentProgress => (double)RevenueEntries.Sum(e => e.Amount) / (double)(Total + TotalVAT);
}