using AutoMapper;

namespace rxdev.Accounting.Banking.Qonto;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Transaction, Model.BankTransaction>()
            .ForMember(t => t.Amount, opt => opt.MapFrom(src => src.LocalAmount * (src.Side == TransactionSide.Credit ? 1 : -1)))
            .ForMember(t => t.SettledDate, opt => opt.MapFrom(src => src.SettledAt))
            ;
    }
}