using Microsoft.EntityFrameworkCore;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.App.Resources.MVVM;
using rxdev.Accounting.Model;
using System;
using System.Linq;
using System.Windows.Input;

namespace rxdev.Accounting.App.ViewModels;

public class QuotationGridViewModel
    : DateFilteredGridViewModel<Quotation, QuotationAdapter>
{
    private ICommand? _downloadCommand;

    public QuotationGridViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider, e => e.IssueDate)
    { }

    public ICommand DownloadCommand => _downloadCommand ??= new RelayCommand<QuotationAdapter>(OnDownload, CanDownload);

    protected override IQueryable<Quotation> GetQuery(bool tracking = false)
        => base.GetQuery(tracking)
        .Include(e => e.Customer)
        .Include(e => e.Items);

    protected override bool CanEdit(QuotationAdapter? item)
        => item is not null
        && item.State == QuotationState.Draft;

    private bool CanDownload(QuotationAdapter? item)
        => item is not null
        && item.State != QuotationState.Draft;

    private void OnDownload(QuotationAdapter? item)
    {
    }
}