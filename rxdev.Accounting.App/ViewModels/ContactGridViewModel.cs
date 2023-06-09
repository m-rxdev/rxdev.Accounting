using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;
using System;
using System.Linq;

namespace rxdev.Accounting.App.ViewModels;

public class ContactGridViewModel
    : GridViewModel<Contact, ContactAdapter>
{
    private int? _customerId;

    public ContactGridViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }

    public override void Load(params object[] args)
    {
        _customerId = args.GetArg<CustomerAdapter>(0)?.Id;
        base.Load(args);
    }

    protected override IQueryable<Contact> GetQuery(bool tracking = false)
        => _customerId.HasValue
        ? base.GetQuery(tracking).Where(e => e.CustomerId == _customerId.Value)
        : base.GetQuery(tracking);

    protected override void OnAdd()
    {
        Contact entity = new();
        if (_customerId.HasValue)
            entity.CustomerId = _customerId.Value;
        NavigationService.NavigateToEdit<Contact, ContactAdapter>(entity);
    }
}