using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using rxdev.Accounting.App.Adapters;
using System;

namespace rxdev.Accounting.App.ApplicationServices;

public abstract class ApplicationService : Adapter
{
    protected readonly IMapper Mapper;
    protected readonly IServiceProvider ServiceProvider;

    public ApplicationService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        Mapper = ServiceProvider.GetRequiredService<IMapper>();
    }
}