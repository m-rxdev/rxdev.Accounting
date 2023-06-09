using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;
using System;

namespace rxdev.Accounting.App.ViewModels;

public class InvoiceItemEditViewModel
    : EditViewModel<InvoiceItem, InvoiceItemAdapter>
{
    public InvoiceItemEditViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }
}