using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;
using System;

namespace rxdev.Accounting.App.ViewModels;

public class ContactEditViewModel
    : EditViewModel<Contact, ContactAdapter>
{
    public ContactEditViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }
}