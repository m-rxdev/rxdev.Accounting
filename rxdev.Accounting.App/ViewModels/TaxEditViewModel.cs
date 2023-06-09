using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;
using System;

namespace rxdev.Accounting.App.ViewModels;

public class TaxEditViewModel
    : EditViewModel<Tax, TaxAdapter>
{
    public TaxEditViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }
}