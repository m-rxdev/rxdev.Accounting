using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;
using System;
using System.Linq;

namespace rxdev.Accounting.App.ViewModels;

public class InvoiceItemGridViewModel
    : GridViewModel<InvoiceItem, InvoiceItemAdapter>
{
    private int? _invoiceId;
    private int? _quotationId;

    public InvoiceItemGridViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }

    public override void Load(params object[] args)
    {
        _invoiceId = args.GetArg<InvoiceAdapter>(0)?.Id;
        _quotationId = args.GetArg<QuotationAdapter>(0)?.Id;
        base.Load(args);
    }

    protected override IQueryable<InvoiceItem> GetQuery(bool tracking = false)
    {
        IQueryable<InvoiceItem> query = base.GetQuery(tracking);

        if (_invoiceId.HasValue)
            query = query.Where(e => e.InvoiceId == _invoiceId.Value);

        if (_quotationId.HasValue)
            query = query.Where(e => e.QuotationId == _quotationId.Value);

        return query;
    }

    protected override void OnAdd()
    {
        InvoiceItem entity = new();

        if (_invoiceId.HasValue)
            entity.InvoiceId = _invoiceId.Value;

        if (_quotationId.HasValue)
            entity.QuotationId = _quotationId.Value;

        NavigationService.NavigateToEdit<InvoiceItem, InvoiceItemAdapter>(entity);
    }
}