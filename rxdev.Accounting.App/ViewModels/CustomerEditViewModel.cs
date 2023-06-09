using Microsoft.Extensions.DependencyInjection;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.App.Resources.MVVM;
using rxdev.Accounting.Model;
using System;
using System.Windows.Input;

namespace rxdev.Accounting.App.ViewModels;

public class CustomerEditViewModel
    : EditViewModel<Customer, CustomerAdapter>
{
    private ICommand? _addCommand;

    public CustomerEditViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        Commands.ActionBar.HasAdd = true;
        ContactGridViewModel = serviceProvider.GetRequiredService<ContactGridViewModel>();
    }

    public ContactGridViewModel ContactGridViewModel { get; init; }
    public ICommand AddCommand => _addCommand ??= new RelayCommand(OnAdd, CanAdd);

    public override void Load(params object[] args)
    {
        base.Load(args);

        ContactGridViewModel.Load(Item);
    }

    public override void Reload()
    {
        base.Reload();

        ContactGridViewModel.Reload();
    }

    private bool CanAdd()
        => ContactGridViewModel.AddCommand.CanExecute(null);

    private void OnAdd()
    {
        if (Item.Id == 0)
        {
            Save();
            ContactGridViewModel.Load(Item);
        }

        ContactGridViewModel.AddCommand.Execute(null);
    }
}