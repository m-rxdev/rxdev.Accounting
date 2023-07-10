using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.Rendering;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.Advanced;
using PdfSharpCore.Utils;
using rxdev.Accounting.Model;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace rxdev.Accounting.FileGeneration;
public class FileFontResolver : IFontResolver
{
    private readonly IFontResolver _defaultResolver;
    public string DefaultFontName => "Avenir Next LT Pro";

    public FileFontResolver(IFontResolver defaultResolver)
    {
        _defaultResolver = defaultResolver;
    }

    public byte[] GetFont(string faceName)
    {
        if(faceName.StartsWith("fonts/"))
        {
            using var ms = new MemoryStream();
            using var fs = File.Open(faceName, FileMode.Open);
            fs.CopyTo(ms);
            ms.Position = 0;
            return ms.ToArray();
        }

        return _defaultResolver.GetFont(faceName);
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        => familyName switch
        {
            "Avenir Next LT Pro" when isBold => new FontResolverInfo("fonts/Avenir Next LT Pro Medium.ttf"),
            "Avenir Next LT Pro" => new FontResolverInfo("fonts/Avenir Next LT Pro Regular.ttf"),
            _ => _defaultResolver.ResolveTypeface(familyName, isBold, isItalic)
        };
}

public static class PdfGenerator
{
    private static Unit PageWidth = Unit.FromPoint(595 - 100);
    private static Color LightColor = new(180, 180, 180);
    private static Color GlowingColor = new(22, 198, 12);
    private static Dictionary<InvoiceItemUnit, string> FRUnits = new()
    {
        { InvoiceItemUnit.Day, "jour" },
        { InvoiceItemUnit.Hour, "heure" },
        { InvoiceItemUnit.Unit, "unité" },
        { InvoiceItemUnit.Click, "clic" },
        { InvoiceItemUnit.Page, "page" },
        { InvoiceItemUnit.Line, "ligne" },
        { InvoiceItemUnit.Word, "mot" },
        { InvoiceItemUnit.Character, "caractère" },
    };

    static PdfGenerator()
    {
        GlobalFontSettings.FontResolver = new FileFontResolver(GlobalFontSettings.FontResolver);
    }

    public static void Generate(
        Stream destination,
        CompanyInfo companyInfo,
        Quotation quotation,
        BankAccount bankAccount)
    {
        Document document = new();

        Style style = document.Styles["Normal"];
        style.Font.Name = "Avenir Next LT Pro";
        style.Font.Color = new Color(100, 100, 100);

        Section section = document.AddSection();
        section.PageSetup.LeftMargin = Unit.FromPoint(50);
        section.PageSetup.RightMargin = Unit.FromPoint(50);
        section.PageSetup.TopMargin = Unit.FromPoint(60);
        section.PageSetup.BottomMargin = Unit.FromPoint(80);

        if (quotation.Customer is null)
            throw new ArgumentNullException(nameof(quotation.Customer));

        DrawHeaderFooter(section, companyInfo, $"Devis N°{quotation.Number}");
        DrawCompanyInfo(section, companyInfo);
        DrawDocumentTitle(section, "DEVIS", $"N°{quotation.Number}");
        DrawCustomerInfo(section, quotation.Customer, quotation);
        DrawItems(section, quotation.Items);
        DrawBankingInfo(section, bankAccount);

        DrawFooter(section, companyInfo);
        PdfDocumentRenderer pdfRenderer = new(false)
        {
            Document = document
        };
        pdfRenderer.RenderDocument();
        pdfRenderer.PdfDocument.Save(destination);
    }

    public static void Generate(
        Stream destination,
        CompanyInfo companyInfo,
        Invoice invoice,
        BankAccount bankAccount)
    {
        Document document = new();

        Style style = document.Styles["Normal"];
        style.Font.Name = "Avenir Next LT Pro";
        style.Font.Color = new Color(100, 100, 100);

        Section section = document.AddSection();
        section.PageSetup.LeftMargin = Unit.FromPoint(50);
        section.PageSetup.RightMargin = Unit.FromPoint(50);
        section.PageSetup.TopMargin = Unit.FromPoint(60);
        section.PageSetup.BottomMargin = Unit.FromPoint(80);

        if (invoice.Customer is null)
            throw new ArgumentNullException(nameof(invoice.Customer));

        DrawHeaderFooter(section, companyInfo, $"Facture N°{invoice.Number}");
        DrawCompanyInfo(section, companyInfo);
        DrawDocumentTitle(section, "FACTURE", $"N°{invoice.Number}");
        DrawCustomerInfo(section, invoice.Customer, invoice);
        DrawItems(section, invoice.Items);
        DrawBankingInfo(section, bankAccount);

        DrawFooter(section, companyInfo);
        PdfDocumentRenderer pdfRenderer = new(false)
        {
            Document = document
        };
        pdfRenderer.RenderDocument();
        pdfRenderer.PdfDocument.Save(destination);
    }

    private static void DrawHeaderFooter(Section parent, CompanyInfo companyInfo, string documentName)
    {
        Table table;
        Row row;
        Paragraph p;

        parent.PageSetup.OddAndEvenPagesHeaderFooter = false;

        table = parent.Headers.Primary.AddTable(PageWidth, 1, 1);
        table.Borders.Bottom.Style = BorderStyle.Single;
        row = table.AddRow();
        row.Format.SpaceAfter = 5;
        p = row[1].AddParagraph(documentName);
        p.Format.Alignment = ParagraphAlignment.Right;



        table = parent.Footers.Primary.AddTable(PageWidth, 4, 1);
        table.Borders.Top.Style = BorderStyle.Single;

        row = table.AddRow();
        row.Format.SpaceBefore = 5;
        row[0].AddParagraph($"{companyInfo.Name} {companyInfo.LegalStatus}");
        row[0].AddParagraph($"SITEN {companyInfo.SIREN} - SIRET {companyInfo.SIRET} - TVA {companyInfo.VAT}");

        p = row[1].AddParagraph();
        p.Format.Alignment = ParagraphAlignment.Right;
        p.AddPageField();
        p.AddText(" / ");
        p.AddNumPagesField();
        row[1].VerticalAlignment = VerticalAlignment.Bottom;

    }

    private static void DrawCompanyInfo(Section parent, CompanyInfo companyInfo)
    {
        Paragraph p;
        Table table = parent.AddTable(PageWidth, 1, 1);
        table.Borders.Width = 0;
        table.AddRow();

        p = table.Rows[0].Cells[0].AddParagraph($"{companyInfo.Name} {companyInfo.LegalStatus}");
        p.Format.Font.Size = 12;
        p.Format.Font.Bold = true;
        p = table.Rows[0].Cells[0].AddParagraph(
            $"SIRET : {companyInfo.SIRET}" + Environment.NewLine
            + $"TVA : {companyInfo.VAT}"
            );

        p = table.Rows[0].Cells[1].AddParagraph(
            companyInfo.Mail + Environment.NewLine
            + companyInfo.PhoneNumber + Environment.NewLine
            + companyInfo.Address + Environment.NewLine
            );
        p.Format.Alignment = ParagraphAlignment.Right;
    }

    private static void DrawDocumentTitle(Section parent, string title, string number)
    {
        Paragraph p;
        
        p = parent.AddParagraph(title);
        p.Format.Alignment = ParagraphAlignment.Center;
        p.Format.Font.Size = 14;
        p.Format.Font.Bold = true;
        p.Format.SpaceBefore = 20;
        p = parent.AddParagraph(number);
        p.Format.Alignment = ParagraphAlignment.Center;
        p.Format.Font.Size = 14;
        p.Format.SpaceAfter = 20;
    }

    private static void DrawCustomerInfo(Section parent, Customer customer, Invoice invoice)
    {
        Table table = parent.AddTable(PageWidth, 1, 5);

        table.AddRow().Populate("CLIENT", customer.Name);
        //table.AddRow().Populate("SIRET", customer.SIRET);
        table.AddRow().Populate("SIREN", customer.SIRET?.Substring(0, 9));
        table.AddRow().Populate("N° TVA", customer.VAT);
        table.AddRow().Populate("ADRESSE", customer.Address);
        table.AddRow().Populate("ÉMIS LE", invoice.IssueDate.ToString("dd/MM/yyyy"));
        table.AddRow().Populate("EXÉCUTION", invoice.ExecutionDate.ToString("dd/MM/yyyy"));

        foreach (Row row in table.Rows)
            row[0].Format.Font.Color = LightColor;

        parent.AddParagraph().Format.SpaceAfter = 0;
    }

    private static void DrawCustomerInfo(Section parent, Customer customer, Quotation quotation)
    {
        Table table = parent.AddTable(PageWidth, 1, 5);

        table.AddRow().Populate("CLIENT", customer.Name);
        //table.AddRow().Populate("SIRET", customer.SIRET);
        table.AddRow().Populate("SIREN", customer.SIRET?.Substring(0, 9));
        table.AddRow().Populate("N° TVA", customer.VAT);
        table.AddRow().Populate("ADRESSE", customer.Address);
        table.AddRow().Populate("ÉMIS LE", quotation.IssueDate.ToString("dd/MM/yyyy"));
        table.AddRow().Populate("VALIDITÉ", quotation.ValidityDate.ToString("dd/MM/yyyy"));

        foreach (Row row in table.Rows)
            row[0].Format.Font.Color = LightColor;

        parent.AddParagraph().Format.SpaceAfter = 0;
    }

    private static void DrawItems(Section parent, List<InvoiceItem> items)
    {
        Paragraph p;
        Row row;
        Table table = parent.AddTable(PageWidth, 12, 3, 2, 3, 3, 3);
        Row header = table.AddRow();
        header.Format.Font.Bold = true;
        header.Populate("Prestation", "Quantité", "TVA", "Unité HT", "HT", "TTC");
        header.Shading.Color = new Color(245, 245, 245);
        header.BottomPadding = 5;
        header.TopPadding = 5;
        header.HeadingFormat = true;
        header.Format.Alignment = ParagraphAlignment.Right;
        header[0].Format.Alignment = ParagraphAlignment.Left;

        foreach (InvoiceItem item in items)
        {
            row = table.AddRow();
            row.Populate(
                "", 
                item.Quantity.ToString(item.Quantity % 1 == 0 ? "F0" : "F2") + $" {GetUnit(item.Quantity, item.Unit)}",
                item.VATRate.ToString("P0"),
                FormatMoney(item.Price),
                FormatMoney((decimal)item.Quantity * item.Price),
                FormatMoney((decimal)item.Quantity * item.Price * (1 + (decimal)item.VATRate))
                );
            row.Format.SpaceBefore = 10;
            row.Format.Alignment = ParagraphAlignment.Right;
            row[0].Format.Alignment = ParagraphAlignment.Left;
            p = row[0].AddParagraph(item.Title);
            p.Format.Font.Bold = true;
            if(item.Description != null)
            {
                p = row[0].AddParagraph(item.Description);
                p.Format.SpaceBefore = 0;
                p.Format.LeftIndent = 10;
                p.Format.Font.Size = 9;

            }
        }

        parent.AddParagraph().Format.SpaceAfter = 20;

        table = parent.AddTable(PageWidth, 4, 1, 1);
        row = table.AddRow().Populate(null, "Total HT", FormatMoney(items.Sum(i => (decimal)i.Quantity * i.Price)));
        row.KeepWith = 2;
        table.AddRow().Populate(null, "Total TVA", FormatMoney(items.Sum(i => (decimal)i.Quantity * i.Price * (decimal)i.VATRate))).Format.Font.Size = 9;
        row = table.AddRow().Populate(null, "Total TTC", FormatMoney(items.Sum(i => (decimal)i.Quantity * i.Price * (1 + (decimal)i.VATRate))));
        row.Format.SpaceBefore = 5;
        row.Format.Font.Color = GlowingColor;

        table.Rows[2].Format.Font.Bold = true;

        for(int i = 0; i < table.Rows.Count; i++)
        {
            table.Rows[i][1].Format.Alignment = ParagraphAlignment.Left;
            table.Rows[i][2].Format.Alignment = ParagraphAlignment.Right;
        }
    }

    private static string GetUnit(double quantity, InvoiceItemUnit unit)
        => (FRUnits.TryGetValue(unit, out string? value)
        ? value : unit.ToString())
        + (quantity > 1 ? "s" : string.Empty);

    private static void DrawBankingInfo(Section parent, BankAccount bankAccount)
    {

        Paragraph p;
        p = parent.AddParagraph($"PAIEMENT");
        p.Format.SpaceBefore= 30;
        p.Format.Font.Bold = true;
        p.Format.Font.Size = 12;

        Table table = parent.AddTable(PageWidth, 1, 5);

        FormattedText ftext = new FormattedText();
        ftext.AddText("Banque : ");
        ftext.Color = LightColor;

        p = table.AddRow()[1].AddParagraph();
        p.Add(ftext);
        p.AddText(bankAccount.Bank);

        p = table.AddRow()[1].AddParagraph();
        p.Format.Font.Size = 8;
        ftext = new FormattedText();
        ftext.AddText("IBAN : ");
        ftext.Color = LightColor;
        p.Add(ftext);
        p.AddText(bankAccount.IBAN);
        p.AddSpace(2);

        ftext = new FormattedText();
        ftext.AddText("RIB : ");
        ftext.Color = LightColor;
        p.Add(ftext);
        p.AddText(bankAccount.RIB);
        p.AddSpace(2);

        ftext = new FormattedText();
        ftext.AddText("BIC : ");
        ftext.Color = LightColor;
        p.Add(ftext);
        p.AddText(bankAccount.BIC);

        /*
        table.AddRow().Populate("Banque", bankAccount.Bank);
        table.AddRow().Populate("IBAN", bankAccount.IBAN);
        table.AddRow().Populate("RIB", bankAccount.RIB);
        table.AddRow().Populate("BIC", bankAccount.BIC);

        foreach (Row row in table.Rows)
            row[0].Format.Font.Color = LightColor;*/
    }

    private static void DrawFooter(Section parent, CompanyInfo companyInfo)
    {
        Paragraph p;

        p = parent.AddParagraph($"CONDITIONS");
        p.Format.SpaceBefore = 20;
        p.Format.Font.Bold = true;
        p.Format.Font.Size = 12;
        Table table = parent.AddTable(PageWidth, 1, 5);
        table.AddRow().Populate(null, companyInfo.InvoiceCustomFooter).Format.Font.Size = 8;
    }

    private static string FormatMoney(decimal amount)
        => amount % 1 == 0
        ? amount.ToString("### ##0 €")
        : amount.ToString("### ##0.00 €");
}

public static class SectionExtension
{
    private static Table Init(this Table table, Unit width, params double[] columns)
    {
        double total = columns.Sum();
        foreach (double c in columns)
            table.AddColumn(c * width / total);
        return table;
    }
    public static Table AddTable(this Section section, Unit width, params double[] columns)
        => section.AddTable().Init(width, columns);

    public static Table AddTable(this HeaderFooter section, Unit width, params double[] columns)
        => section.AddTable().Init(width, columns);

    public static Row Populate(this Row row, params string?[] values)
        => Populate(row, VerticalAlignment.Top, values);

    public static Row Populate(this Row row, VerticalAlignment verticalAlignment, params string?[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            if (string.IsNullOrEmpty(values[i]))
                continue;

            row.Cells[i].AddParagraph(values[i]);
            row.Cells[i].VerticalAlignment = verticalAlignment;
        }
        return row;
    }
}