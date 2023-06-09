﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.App.Resources.MVVM;
using rxdev.Accounting.FileGeneration;
using rxdev.Accounting.Model;
using rxdev.Accounting.Persistence;
using System;
using System.Linq;
using System.Windows.Input;

namespace rxdev.Accounting.App.ViewModels;

public class InvoiceGridViewModel
    : DateFilteredGridViewModel<Invoice, InvoiceAdapter>
{
    private ICommand? _downloadCommand;
    private ICommand? _uploadCommand;

    public InvoiceGridViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider, e => e.IssueDate)
    {
        //Commands.ActionBar.HasUpload = true;
    }

    public ICommand DownloadCommand => _downloadCommand ??= new RelayCommand<InvoiceAdapter>(OnDownload, CanDownload);
    public ICommand UploadCommand => _uploadCommand ??= new RelayCommand(OnUpload, CanUpload);

    protected override IQueryable<Invoice> GetQuery(bool tracking = false)
        => base.GetQuery(tracking)
        .Include(e => e.Customer)
        .Include(e => e.Items)
        .Include(e => e.RevenueEntries);

    protected override bool CanEdit(InvoiceAdapter? item)
        => true; 
    //item is not null
    //    && item.State == InvoiceState.Draft;

    private bool CanDownload(InvoiceAdapter? item)
        => item is not null
        && item.State != InvoiceState.Draft;

    private bool CanUpload()
        => true;

    private void OnDownload(InvoiceAdapter? item)
    {
    }

    private void OnUpload()
    {
        OpenFileDialog ofd = new()
        {
            Filter = "PDF file|*.pdf",
            Multiselect = false,
        };

        if (ofd.ShowDialog() != true)
            return;

        Repository<Invoice> repo = ServiceProvider.GetRequiredService<Repository<Invoice>>();
        /*
        Item.Attachment = new AttachmentAdapter
        {
            FileName = Path.GetFileName(ofd.FileName),
            EntityData = new EntityData()
            {
                Data = File.ReadAllBytes(ofd.FileName),
            }
        };*/
    }

    protected override bool CanRemove(InvoiceAdapter? item)
        => item is not null
        && item.State == InvoiceState.Draft;

    public override void Load(params object[] args)
    {
        //decimal total = ServiceProvider.GetRequiredService<Repository<InvoiceItem>>().AsQueryable().Sum(e => e.Price * e.Quantity);



        base.Load(args);
    }
}