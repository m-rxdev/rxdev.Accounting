using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;
using System;

namespace rxdev.Accounting.App.ViewModels;

public class BankAccountGridViewModel
    : GridViewModel<BankAccount, BankAccountAdapter>
{
    public BankAccountGridViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }
}