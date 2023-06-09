using AutoMapper;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;

namespace rxdev.Accounting.App;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Entity, EntityAdapter>()
            .AfterMap((s, d) => d.IsDirty = false)
            .IncludeAllDerived();

        CreateMap<CompanyInfo, CompanyInfoAdapter>().ReverseMap();
        CreateMap<BankAccount, BankAccountAdapter>().ReverseMap();
        CreateMap<BankTransaction, BankTransactionAdapter>().ReverseMap();
        CreateMap<Contact, ContactAdapter>().ReverseMap();
        CreateMap<PurchaseEntry, PurchaseEntryAdapter>().ReverseMap();
        CreateMap<RevenueEntry, RevenueEntryAdapter>().ReverseMap();
        CreateMap<Attachment, AttachmentAdapter>().ReverseMap();
        CreateMap<Customer, CustomerAdapter>().ReverseMap();
        CreateMap<Invoice, InvoiceAdapter>()
            .ForMember(s => s.ExecutionDays, opt => opt.MapFrom((s, d) => (s.ExecutionDate - s.IssueDate).Days))
            .ReverseMap();
        CreateMap<InvoiceItem, InvoiceItemAdapter>().ReverseMap();
        CreateMap<Quotation, QuotationAdapter>()
            .ForMember(s => s.ValidityDays, opt => opt.MapFrom((s, d) => (s.ValidityDate - s.IssueDate).Days))
            .ReverseMap();
        CreateMap<Tax, TaxAdapter>().ReverseMap();
    }
}