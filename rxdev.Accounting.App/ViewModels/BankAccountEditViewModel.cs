using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;
using System;

namespace rxdev.Accounting.App.ViewModels;

public class BankAccountEditViewModel
    : EditViewModel<BankAccount, BankAccountAdapter>
{
    public BankAccountEditViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }
}