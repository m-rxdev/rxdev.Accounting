using Microsoft.Extensions.DependencyInjection;
using rxdev.Accounting.App.ApplicationServices;
using rxdev.Accounting.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rxdev.Accounting.App;

public class InjectionProfile
{
    public InjectionProfile(IServiceCollection services)
    {
        services
            .AddSingleton<NotificationService>()
            .AddSingleton<NavigationService>()

            .AddSingleton<MainViewModel>()
            .AddSingleton<DashboardViewModel>()
            .AddSingleton<PDFPreviewViewModel>()
            ;

        RegisterViewModels(services, typeof(EditViewModel<,>));
        RegisterViewModels(services, typeof(GridViewModel<,>));
    }

    private static void RegisterViewModels(IServiceCollection services, Type baseType)
    {
        foreach (Type type in GetViewModelTypes())
        {
            if (!type.IsGenericAssignable(baseType, out Type[]? args))
                continue;

            services
                .AddSingleton(type)
                .AddSingleton(baseType.MakeGenericType(args), type);
        }
        Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.IsAssignableTo(typeof(EditViewModel<,>).GetGenericTypeDefinition())).ToArray();
    }

    private static IEnumerable<Type> GetViewModelTypes()
        => AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(t => t.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == "rxdev.Accounting.App.ViewModels");
}