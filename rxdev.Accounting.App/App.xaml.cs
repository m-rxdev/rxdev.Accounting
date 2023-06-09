using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using rxdev.Accounting.App.Views;
using rxdev.Accounting.Import;
using rxdev.Accounting.Persistence;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace rxdev.Accounting.App;

public partial class App : Application
{
    private IHost? _host;

    protected override void OnStartup(StartupEventArgs e)
    {
        _host = Host.CreateDefaultBuilder().ConfigureServices((_, c) => {
            foreach (Type type in AssemblyExtension.GetLocalTypes("rxdev.*.dll").Where(t => !t.IsInterface && t.Name == "InjectionProfile"))
                Activator.CreateInstance(type, c);

            c.AddSingleton(new MapperConfiguration(c =>
            {
                foreach (Type type in AssemblyExtension.GetLocalTypes("rxdev.*.dll").Where(t => t.IsSubclassOf(typeof(Profile))))
                    c.AddProfile(type);
            }).CreateMapper());

            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            c.AddSingleton<IConfiguration>(configurationRoot);
            c.AddTransient<FreebeDbInitializer>();
            c.AddDbContext<AccountingDbContext>(options => options.UseSqlite("data source=db.sqlite"));
        }).Build();

        IConfiguration configuration = _host.Services.GetRequiredService<IConfiguration>();
        if(configuration.GetValue<bool>("ResetDataBase") == true)
            File.Delete(@"db.sqlite");

        DbContext dbContext = _host.Services.GetRequiredService<AccountingDbContext>();
        dbContext.Database.EnsureCreated();
        //dbContext.Database.Migrate();

        string? importDirectory = configuration.GetValue<string>("ImportDirectory");
        if (importDirectory is not null)
            _host.Services.GetRequiredService<FreebeDbInitializer>().Init(importDirectory);

        Current.MainWindow = new MainView().Init(_host.Services);
        Current.MainWindow.Show();
        Current.MainWindow.Activate();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        _host?.Dispose();
    }
}
