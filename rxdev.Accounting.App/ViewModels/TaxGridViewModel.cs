using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;
using System;

namespace rxdev.Accounting.App.ViewModels;

public class TaxGridViewModel
    : GridViewModel<Tax, TaxAdapter>
{
    public TaxGridViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }
}