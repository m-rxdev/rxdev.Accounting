using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Windows.Input;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.App.Resources.MVVM;
using rxdev.Accounting.FileGeneration;
using rxdev.Accounting.Model;
using rxdev.Accounting.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace rxdev.Accounting.App.ViewModels;

public class InvoiceEditViewModel
    : EditViewModel<Invoice, InvoiceAdapter>
{
    private ObservableCollection<CustomerAdapter> _customers = new();
    private ICommand? _addCommand;
    private ICommand? _generateCommand;
    private ICommand? _previewCommand;

    public InvoiceEditViewModel(
        IServiceProvider serviceProvider,
        InvoiceItemGridViewModel invoiceItemGridViewModel,
        RevenueEntryGridViewModel revenueEntryGridViewModel)
        : base(serviceProvider)
    {
        Commands.ActionBar.HasAdd = true;
        Commands.ActionBar.HasGenerate = true;
        Commands.ActionBar.HasPreview = true;

        InvoiceItemGridViewModel = invoiceItemGridViewModel;
        RevenueEntryGridViewModel = revenueEntryGridViewModel;
    }

    public ObservableCollection<CustomerAdapter> Customers { get => _customers; set => SetDirty(ref _customers, value); }
    public InvoiceItemGridViewModel InvoiceItemGridViewModel { get; init; }
    public RevenueEntryGridViewModel RevenueEntryGridViewModel { get; init; }
    public ICommand AddCommand => _addCommand ??= new RelayCommand(OnAdd, CanAdd);
    public ICommand GenerateCommand => _generateCommand ??= new RelayCommand(OnGenerate, CanGenerate);
    public ICommand PreviewCommand => _previewCommand ??= new RelayCommand(OnPreview, CanPreview);

    protected override IQueryable<Invoice> GetQuery(bool tracking = false)
        => base.GetQuery(tracking)
        .Include(e => e.RevenueEntries)
        .ThenInclude(e => e.BankTransaction);

    public override void Load(params object[] args)
    {
        base.Load(args);

        Customers = new ObservableCollection<CustomerAdapter>(
            Mapper.Map<IEnumerable<CustomerAdapter>>(ServiceProvider.GetRequiredService<Repository<Customer>>().AsQueryable()));

        if (Customers.Count == 0)
        {
            NavigationService.NavigateToEdit<Customer, CustomerAdapter>();
            return;
        }

        if (Item.Id == 0)
            Item.CustomerId = Customers.First().Id;

        Item.IsDirty = false;
        InvoiceItemGridViewModel.Load(Item);
        RevenueEntryGridViewModel.Load(Item);
    }

    public override void Reload()
    {
        base.Reload();

        InvoiceItemGridViewModel.Reload();
        RevenueEntryGridViewModel.Reload();
    }

    private bool CanGenerate()
        => Item.State == InvoiceState.Draft;

    private void OnGenerate()
    {
        // Sure ?
        // Unsaved changes ?
        using FileStream fs = File.OpenWrite(@"D:\test.pdf");
        Generate(fs);
    }

    private bool CanAdd()
        => Item.State == InvoiceState.Draft
        && InvoiceItemGridViewModel.AddCommand.CanExecute(null);

    private void OnAdd()
    {
        if(Item.Id == 0)
        {
            Save();
            InvoiceItemGridViewModel.Load(Item);
        }

        InvoiceItemGridViewModel.AddCommand.Execute(null);
    }

    private bool CanPreview()
        => true;

    private void OnPreview()
    {
        object? source = null;

        switch (Item.State)
        {
            case InvoiceState.Draft:
                {
                    if(Item.IsDirty)
                        Save();
                    using MemoryStream memoryStream = new();
                    Generate(memoryStream);
                    source = memoryStream.ToArray();
                }
                break;
            case InvoiceState.Locked:
            case InvoiceState.Imported:
                {
                    source = ServiceProvider.GetRequiredService<Repository<Invoice>>()
                        .AsQueryable()
                        .Include(e => e.Attachment)
                        .ThenInclude(e => e!.EntityData)
                        .First(e => e.Id == Item.Id)
                        .Attachment!.EntityData;
                }
                break;
        }

        if(source is not null)
            NavigationService.NavigateTo<PDFPreviewViewModel>(source);
    }

    private void Generate(Stream stream)
        => PdfGenerator.Generate(stream,
            ServiceProvider.GetRequiredService<Repository<CompanyInfo>>().AsQueryable().First(),
            ServiceProvider.GetRequiredService<Repository<Invoice>>().AsQueryable()
                .Include(e => e.Items)
                .Include(e => e.Customer)
                .First(e => e.Id == Item.Id),
            ServiceProvider.GetRequiredService<Repository<BankAccount>>().AsQueryable().First());

    protected override Invoice CreateEntity()
    {
        Invoice result = base.CreateEntity();
        CompanyInfo companyInfo = ServiceProvider.GetRequiredService<Repository<CompanyInfo>>().AsQueryable().First();

        result.Index = ServiceProvider.GetRequiredService<Repository<Invoice>>().AsQueryable().OrderByDescending(e => e.Index).Select(e => e.Index).FirstOrDefault();

        if (result.Index == 0)
            result.Index = ServiceProvider.GetRequiredService<Repository<CompanyInfo>>().AsQueryable().First().InvoiceIndex;
        else
            result.Index++;

        result.Number = string.Format(companyInfo.InvoiceNumberingFormat, DateTime.Now, result.Index);

        return result;
    }
}