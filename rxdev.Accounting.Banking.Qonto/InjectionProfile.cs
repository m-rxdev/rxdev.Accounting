using Microsoft.Extensions.DependencyInjection;

namespace rxdev.Accounting.Banking.Qonto;

public class InjectionProfile
{
    public InjectionProfile(IServiceCollection services)
        => services.AddTransient<QontoClient>();
}