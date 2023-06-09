using rxdev.Accounting.Model;
using rxdev.Accounting.Persistence;
using System;
using System.Linq;

namespace rxdev.Accounting.App.ViewModels;

public class MainViewModel
    : ViewModel
{
    public MainViewModel(
        IServiceProvider serviceProvider,
        Repository<CompanyInfo> repoc,
        CompanyInfoEditViewModel companyEditViewModel,
        DashboardViewModel dashboardViewModel)
        : base(serviceProvider)
    {
        NavigationService.SetDefault(dashboardViewModel);

        CompanyInfo? info = repoc.AsQueryable().FirstOrDefault();
        if (info is null)
        {
            companyEditViewModel.Load(new CompanyInfo());
            NavigationService.NavigateTo(companyEditViewModel);
        }
        else
            NavigationService.UpdateYearRange(info.CreationDate);
    }
}