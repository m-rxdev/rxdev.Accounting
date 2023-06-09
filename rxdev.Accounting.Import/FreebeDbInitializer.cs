using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.DependencyInjection;
using rxdev.Accounting.Banking.Qonto;
using rxdev.Accounting.Model;
using rxdev.Accounting.Persistence;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace rxdev.Accounting.Import;

public class FreebeDbInitializer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DbContext _dbContext;
    private string _directory = string.Empty;
    private static readonly CsvConfiguration Configuration = new(new CultureInfo("fr-FR"))
    {
        Delimiter = ";",
        HasHeaderRecord = true,
    };

    public FreebeDbInitializer(AccountingDbContext dbContext, IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
    }

    public void Init(string directory)
    {
        _directory = directory;
        PopulateCompanyInfo();
        PopulateBankAccounts();
        PopulateCustomers();
        PopulateContacts();
        PopulateInvoices();
        PopulateTaxes();
        PopulateRevenues();
        PopulatePurchases();
    }

    private void PopulateCompanyInfo()
    {
        string filePath = Path.Combine(_directory, "companyinfo.json");
        DbSet<CompanyInfo> set = _dbContext.Set<CompanyInfo>();

        if (set.Any()
            || !File.Exists(filePath))
            return;

        CompanyInfo? companyInfo = JsonSerializer.Deserialize<CompanyInfo>(File.ReadAllText(filePath));

        if (companyInfo is null)
            return;

        set.Add(companyInfo);

        _dbContext.SaveChanges();
    }

    private void PopulateBankAccounts()
    {
        string filePath = Path.Combine(_directory, "bankAccounts.json");
        DbSet<BankAccount> set = _dbContext.Set<BankAccount>();

        if (set.Any()
            || !File.Exists(filePath))
            return;

        BankAccount[]? bankAccounts = JsonSerializer.Deserialize<BankAccount[]>(File.ReadAllText(filePath));

        if (bankAccounts is null
            || !bankAccounts.Any())
            return;

        DateTime now = DateTime.Now;

        foreach (BankAccount bankAccount in bankAccounts)
        {
            if (bankAccount.IBAN is null
                || bankAccount.ApiInfo is null)
                continue;

            QontoClient client = _serviceProvider.GetRequiredService<QontoClient>();
            try
            {
                bankAccount.Transactions = client.GetTransactions(bankAccount.IBAN, bankAccount.ApiInfo, null);
                bankAccount.LastSyncDate = now;
            }
            catch { }
        }

        set.AddRange(bankAccounts);

        _dbContext.SaveChanges();
    }

    private void PopulateTaxes()
    {
        DbSet<Tax> set = _dbContext.Set<Tax>();

        if (set.Any())
            return;

        set.AddRange(new Tax { Name = "URSSAF", Rate = 0.213 });

        _dbContext.SaveChanges();
    }

    private void PopulateCustomers()
    {
        string filePath = Path.Combine(_directory, "clients.csv");
        DbSet<Customer> set = _dbContext.Set<Customer>();

        if (set.Any()
            || !File.Exists(filePath))
            return;

        using StreamReader reader = new(filePath);
        using CsvReader csv = new(reader, Configuration);

        foreach (IDictionary<string, object> customer in csv.GetRecords<dynamic>())
        {
            set.Add(new Customer
            {
                Name = (string)customer["Nom du client"],
                SIRET = (string)customer["SIRET"],
                VAT = (string)customer["N° TVA"],
                Address = (string)customer["Adresse"],
                Website = (string)customer["Site internet"],
            });
        }

        _dbContext.SaveChanges();
    }

    private void PopulateContacts()
    {
        string filePath = Path.Combine(_directory, "contacts.csv");
        DbSet<Customer> customerSet = _dbContext.Set<Customer>();
        DbSet<Contact> set = _dbContext.Set<Contact>();

        if (set.Any() 
            || !File.Exists(filePath))
            return;

        using StreamReader reader = new(filePath);
        using CsvReader csv = new(reader, Configuration);

        foreach (IDictionary<string, object> contact in csv.GetRecords<dynamic>().Select(v => (IDictionary<string, object>)v))
        {
            Customer customer = customerSet.First(e => e.Name == (string)contact["Nom du client"]);

            set.Add(new Contact
            {
                CustomerId = customer.Id,
                Name = (string)contact["Référent"],
                PhoneNumber = (string)contact["Numéro de téléphone"],
                Email = (string)contact["Email"],
            });
        }

        _dbContext.SaveChanges();
    }

    private void PopulateInvoices()
    {
        DbSet<Customer> customerSet = _dbContext.Set<Customer>();
        DbSet<Invoice> set = _dbContext.Set<Invoice>();

        if (set.Any())
            return;

        foreach(string filePath in Directory.GetFiles(Path.Combine(_directory, "web"), "invoices_*.htm"))
        {
            int year = int.Parse(Path.GetFileNameWithoutExtension(filePath).Replace("invoices_", string.Empty));

            HtmlDocument doc = new();
            doc.Load(filePath, Encoding.UTF8);

            foreach (var container in doc.DocumentNode.SelectNodes("/html/body/div[1]/div[4]/div[2]/div/div/ul/li").Skip(1))
            {
                string[] nodes = container.SelectNodes("div/div/div").Take(6).Select(n => n.InnerText[1..^1]).ToArray();

                set.Add(new Invoice
                {
                    IssueDate = ParseWebDate(year, nodes[0]),
                    ExecutionDate = ParseWebDate(year, nodes[0]),
                    Number = nodes[1],
                    CustomerId = customerSet.First(c => c.Name == nodes[2]).Id,
                    Total = ParseWebMoney(nodes[5]),
                    TotalVAT = ParseWebMoney(nodes[4]),
                    State = InvoiceState.Imported,
                    Attachment = new Attachment
                    {
                        FileName = $"{nodes[1]}.pdf",
                        EntityData = new EntityData
                        {
                            Data = File.ReadAllBytes(Path.Combine(_directory, $"documents\\factures\\{nodes[1]}.pdf")),
                        }
                    }
                });
            }
        }

        _dbContext.SaveChanges();
    }

    private void PopulateRevenues()
    {
        DbSet<RevenueEntry> set = _dbContext.Set<RevenueEntry>();

        if (set.Any())
            return;

        List<BankTransaction> transactions = _dbContext.Set<BankTransaction>().ToList();
        Invoice[] invoices = _dbContext.Set<Invoice>().ToArray();
        int cnt = 0;
        foreach(Invoice invoice in invoices.OrderBy(e => e.IssueDate))
        {
            BankTransaction? transaction = null;

            foreach (BankTransaction t in transactions.OrderBy(e => e.SettledDate))
            {
                if (t.SettledDate < invoice.IssueDate
                    || t.SettledDate > invoice.IssueDate + TimeSpan.FromDays(90)
                    || t.Amount != invoice.Total + invoice.TotalVAT)
                    continue;

                transaction = t;
                break;
            }

            if (transaction is null)
                continue;

            transactions.Remove(transaction);
            set.Add(new RevenueEntry
            {
                Amount = transaction.Amount,
                BankTransactionId = transaction.Id,
                InvoiceId = invoice.Id,
            });
            cnt++;
        }

        _dbContext.SaveChanges();
    }

    private void PopulatePurchases()
    {
        string filePath = Path.Combine(_directory, "achats.csv");
        DbSet<PurchaseEntry> set = _dbContext.Set<PurchaseEntry>();
        DbSet<BankTransaction> tSet = _dbContext.Set<BankTransaction>();

        if (set.Any())
            return;

        using StreamReader reader = new(filePath);
        using CsvReader csv = new(reader, Configuration);

        foreach (IDictionary<string, object> purchase in csv.GetRecords<dynamic>().Select(v => (IDictionary<string, object>)v))
        {
            DateTime date = DateTime.ParseExact((string)purchase["Date"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            decimal totalVAT = decimal.Parse((string)purchase["TVA"]);
            decimal total = - decimal.Parse((string)purchase["Montant TTC"]) - totalVAT;
            //decimal total = decimal.Parse((string)purchase["Montant HT"]);
            int bankTransactionId = tSet.First(e => e.SettledDate.Date == date && e.Amount == -(total + totalVAT)).Id;
            string[] factures = ((string)purchase["Factures"]).Split(" | ");
            string vendor = (string)purchase["Clients"];

            decimal amount = decimal.Round(total / factures.Length, 2);
            decimal vat = decimal.Round(totalVAT / factures.Length, 2);

            for(int i = 0; i < factures.Length; i++)
            {
                set.Add(new PurchaseEntry
                {
                    Amount = i < factures.Length - 1 ? amount : (total - amount * (factures.Length - 1)),
                    VAT = i < factures.Length - 1 ? vat : (totalVAT - vat * (factures.Length - 1)),
                    BankTransactionId = bankTransactionId,
                    Vendor = vendor,
                    Attachment = new Attachment
                    {
                        FileName = $"{factures[i]}.pdf",
                        EntityData = new EntityData
                        {
                            Data = File.ReadAllBytes(Path.Combine(_directory, $"achats\\{factures[i]}.pdf")),
                        }
                    }
                });
            }
        }

        _dbContext.SaveChanges();
    }

    private static DateTime ParseWebDate(int year, string value)
    {
        int[] el = value.Split('.').Select(int.Parse).ToArray();
        return new DateTime(year, el[1], el[0]);
    }

    private static decimal ParseWebMoney(string value)
        => decimal.Parse(value.Remove("€", " "), CultureInfo.InvariantCulture);
}