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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace rxdev.Accounting.App.ViewModels;

public class QuotationEditViewModel
    : EditViewModel<Quotation, QuotationAdapter>
{
    private ObservableCollection<CustomerAdapter> _customers = new();
    private ICommand? _addCommand;
    private ICommand? _generateCommand;
    private ICommand? _previewCommand;

    public QuotationEditViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        Commands.ActionBar.HasAdd = true;
        Commands.ActionBar.HasGenerate = true;
        Commands.ActionBar.HasPreview = true;
        InvoiceItemGridViewModel = serviceProvider.GetRequiredService<InvoiceItemGridViewModel>();
    }

    public ObservableCollection<CustomerAdapter> Customers { get => _customers; set => SetDirty(ref _customers, value); }
    public InvoiceItemGridViewModel InvoiceItemGridViewModel { get; private set; }
    public ICommand AddCommand => _addCommand ??= new RelayCommand(OnAdd, CanAdd);
    public ICommand GenerateCommand => _generateCommand ??= new RelayCommand(OnGenerate, CanGenerate);
    public ICommand PreviewCommand => _previewCommand ??= new RelayCommand(OnPreview, CanPreview);

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
    }

    public override void Reload()
    {
        base.Reload();

        InvoiceItemGridViewModel.Reload();
    }

    private bool CanGenerate()
        => true;

    private void OnGenerate()
    {
        // Sure ?
        // Unsaved changes ?
        using FileStream fs = File.OpenWrite(@"D:\test.pdf");
        Generate(fs);
    }

    private bool CanAdd()
        => InvoiceItemGridViewModel.AddCommand.CanExecute(null);

    private void OnAdd()
    {
        if (Item.Id == 0)
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
        // Unsaved changes ?
        Save();
        string fileName = "quotationpreview.pdf";
        using FileStream fs = File.OpenWrite(fileName);
        Generate(fs);
        fs.Close();
        Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
    }

    private void Generate(Stream stream)
        => PdfGenerator.Generate(stream,
            ServiceProvider.GetRequiredService<Repository<CompanyInfo>>().AsQueryable().First(),
            ServiceProvider.GetRequiredService<Repository<Quotation>>().AsQueryable()
                .Include(e => e.Items)
                .Include(e => e.Customer)
                .First(e => e.Id == Item.Id),
            ServiceProvider.GetRequiredService<Repository<BankAccount>>().AsQueryable().First());

    protected override Quotation CreateEntity()
    {
        Quotation result = base.CreateEntity();
        CompanyInfo companyInfo = ServiceProvider.GetRequiredService<Repository<CompanyInfo>>().AsQueryable().First();

        result.Index = ServiceProvider.GetRequiredService<Repository<Quotation>>().AsQueryable().OrderByDescending(e => e.Index).Select(e => e.Index).FirstOrDefault();

        if (result.Index == 0)
            result.Index = ServiceProvider.GetRequiredService<Repository<CompanyInfo>>().AsQueryable().First().QuotationIndex;
        else
            result.Index++;

        result.Number = string.Format(companyInfo.QuotationNumberingFormat, DateTime.Now, result.Index);

        return result;
    }
}