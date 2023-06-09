using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;
using System;

namespace rxdev.Accounting.App.ViewModels;

public class CustomerGridViewModel
    : GridViewModel<Customer, CustomerAdapter>
{
    public CustomerGridViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }
}