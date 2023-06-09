using Microsoft.Extensions.DependencyInjection;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.App.ViewModels;
using rxdev.Accounting.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace rxdev.Accounting.App.ApplicationServices;

public class NavigationService
    : ApplicationService
{
    private readonly List<ViewModel> _stack = new();
    private ViewModel? _defaultViewModel;
    private ViewModel? _viewModel;
    private int _selectedYear = DateTime.Now.Year;
    private ObservableCollection<int> _selectableYears = new(Enumerable.Range(DateTime.Now.Year - 4, 5).Reverse());

    public NavigationService(IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }
        
    public ViewModel? ViewModel { get => _viewModel; private set => Set(ref _viewModel, value); }
    public int SelectedYear { get => _selectedYear; set
        {
            if (!Set(ref _selectedYear, value))
                return;
            ViewModel?.Reload();
        }
    }
    public ObservableCollection<int> SelectableYears { get => _selectableYears; set => Set(ref _selectableYears, value); }

    public void NavigateBack()
    {
        if (ViewModel is not null)
        {
            _stack.Remove(ViewModel);
            ViewModel.Unload();
        }

        ViewModel? viewModel = _stack.LastOrDefault() ?? _defaultViewModel;

        viewModel?.Reload();
        ViewModel = viewModel;
    }

    public void NavigateTo<T>(params object[] args)
        where T : ViewModel
        => NavigateTo(ServiceProvider.GetRequiredService<T>(), args);

    public void NavigateTo(Type? type, params object[] args)
    {
        if (type is null)
            return;

        NavigateTo(ServiceProvider.GetRequiredService(type) as ViewModel, args);
    }

    public void NavigateToEdit<TEntity, TAdapter>(params object[] args)
        where TEntity : Entity, new()
        where TAdapter: EntityAdapter, new()
        => NavigateTo(ServiceProvider.GetRequiredService<EditViewModel<TEntity, TAdapter>>(), args);

    public void NavigateToGrid<TEntity, TAdapter>(params object[] args)
        where TEntity : Entity, new()
        where TAdapter : EntityAdapter, new()
        => NavigateTo(ServiceProvider.GetRequiredService<GridViewModel<TEntity, TAdapter>>(), args);

    public void NavigateTo(ViewModel? viewModel, params object[] args)
    {
        if (viewModel is null
            || viewModel == ViewModel)
            return;

        ViewModel?.Unload();

        _stack.Remove(viewModel);

        if (viewModel != _defaultViewModel)
            _stack.Add(viewModel);

        viewModel.Load(args);
        ViewModel = viewModel;
    }

    public void SetDefault(ViewModel viewModel)
    {
        _defaultViewModel = viewModel;
        viewModel.Load();
        ViewModel = viewModel;
    }

    public void UpdateYearRange(DateTime? creationDate = null)
    {
        int start = creationDate.HasValue ? creationDate.Value.Year : DateTime.Now.Year - 5;
        int end = DateTime.Now.Year;
        SelectableYears = new(Enumerable.Range(start, end - start + 1).Reverse());
    }
}