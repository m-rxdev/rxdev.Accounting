using Microsoft.Extensions.DependencyInjection;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.Model;
using rxdev.Accounting.Persistence;
using System;
using System.Linq;

namespace rxdev.Accounting.App.ViewModels;

public class CompanyInfoEditViewModel
    : EditViewModel<CompanyInfo, CompanyInfoAdapter>
{
    public CompanyInfoEditViewModel(IServiceProvider serviceProvider) 
        : base(serviceProvider)
    { }

    protected override void OnSave()
    {
        base.OnSave();
        NavigationService.UpdateYearRange(Item.CreationDate);
    }

    public override void Load(params object[] args)
    {
        Repository<CompanyInfo> repo = ServiceProvider.GetRequiredService<Repository<CompanyInfo>>();
        base.Load(repo.AsQueryable().FirstOrDefault() ?? new CompanyInfo());
    }
}