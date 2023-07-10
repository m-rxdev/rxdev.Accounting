using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.App.Resources.MVVM;
using rxdev.Accounting.FileGeneration;
using rxdev.Accounting.Model;
using rxdev.Accounting.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        Item.IsDirty = false;

        if (Item.Id == 0)
            Item.CustomerId = Customers.First().Id;

        if (Item.State == InvoiceState.Draft)
            UpdateTotal();

        InvoiceItemGridViewModel.Load(Item);
        RevenueEntryGridViewModel.Load(Item);
    }

    public override void Reload()
    {
        base.Reload();

        if (Item.State == InvoiceState.Draft)
            UpdateTotal();

        InvoiceItemGridViewModel.Reload();
        RevenueEntryGridViewModel.Reload();
    }

    private bool CanGenerate()
        => Item.State == InvoiceState.Draft;

    private void OnGenerate()
    {
        // Sure ?
        // Unsaved changes ?
        using MemoryStream ms = new();
        Generate(ms);

        using IServiceScope scope = ServiceProvider.CreateScope();
        UnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
        Repository<Attachment> repo = scope.ServiceProvider.GetRequiredService<Repository<Attachment>>();
        if (Item.AttachmentId.HasValue)
            repo.Remove(Item.AttachmentId.Value);
        Attachment attachment = new()
        {
            FileName = $"{Item.Number}.pdf",
            EntityData = new EntityData { Data = ms.ToArray() }
        };
        repo.Add(attachment);
        unitOfWork.Save();

        Item.AttachmentId = attachment.Id;
        Item.State = InvoiceState.Locked;
        UpdateTotal();
        Save();
    }

    private void UpdateTotal()
    {
        using IServiceScope scope = ServiceProvider.CreateScope();
        Repository<InvoiceItem> repo = scope.ServiceProvider.GetRequiredService<Repository<InvoiceItem>>();
        InvoiceItem[] items = repo.AsQueryable().Where(e => e.InvoiceId == Item.Id).ToArray();

        Item.Total = items.Sum(e => (decimal)e.Quantity * e.Price);
        Item.TotalVAT = items.Sum(e => (decimal)e.Quantity * e.Price * (decimal)e.VATRate);
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