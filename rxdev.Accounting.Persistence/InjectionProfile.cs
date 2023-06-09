using Microsoft.Extensions.DependencyInjection;

namespace rxdev.Accounting.Persistence;

public class InjectionProfile
{
    public InjectionProfile(IServiceCollection services)
    {
        services.AddScoped<UnitOfWork>();

        foreach (Type type in EntityHelper.GetEntityTypes())
            services.AddScoped(typeof(Repository<>).MakeGenericType(type));
    }
}