using Microsoft.Extensions.DependencyInjection;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.App.Resources.MVVM;
using rxdev.Accounting.Model;
using rxdev.Accounting.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace rxdev.Accounting.App.ViewModels;

public abstract class GridViewModel<TEntity, TAdapter>
    : ViewModel
    where TEntity : Entity, new()
    where TAdapter : EntityAdapter, new()
{
    private ObservableCollection<TAdapter> _items = new();
    private ICommand? _addCommand;
    private ICommand? _removeCommand;
    private ICommand? _copyCommand;
    private ICommand? _editCommand;
    private ICommand? _viewCommand;

    protected GridViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        Title = typeof(TEntity).Name;
        Commands.ActionBar.HasAdd = true;
        Commands.ItemActions.HasEdit = true;
        Commands.ItemActions.HasRemove = true;
    }

    public ICommand AddCommand => _addCommand ??= new RelayCommand(OnAdd, CanAdd);
    public ICommand RemoveCommand => _removeCommand ??= new RelayCommand<TAdapter>(OnRemove, CanRemove);
    public ICommand CopyCommand => _copyCommand ??= new RelayCommand<TAdapter>(OnCopy, CanCopy);
    public ICommand EditCommand => _editCommand ??= new RelayCommand<TAdapter>(OnEdit, CanEdit);
    public ICommand ViewCommand => _viewCommand ??= new RelayCommand<TAdapter>(OnView, CanView);
    public ObservableCollection<TAdapter> Items { get => _items; set => Set(ref _items, value); }

    public override void Reload()
    {
        Items.Clear();

        foreach (TAdapter adapter in Mapper.Map<IEnumerable<TAdapter>>(GetQuery()))
            Items.Add(adapter);
        //Items = new ObservableCollection<TAdapter>(Mapper.Map<IEnumerable<TAdapter>>(GetQuery()));
    }

    protected virtual IQueryable<TEntity> GetQuery(bool tracking = false)
    {
        Repository<TEntity> repo = ServiceProvider.GetRequiredService<Repository<TEntity>>();
        return repo.AsQueryable(tracking);
    }

    protected virtual void OnAdd()
    {
        NavigationService.NavigateToEdit<TEntity, TAdapter>(0);
    }

    protected virtual void OnRemove(TAdapter? item) 
    {
        if (item is null)
            return;

        switch (NotificationService.Ask("Sure ?", "Confirm remove", System.Windows.MessageBoxButton.YesNo))
        {
            case System.Windows.MessageBoxResult.Yes:
                {
                    using IServiceScope scope = ServiceProvider.CreateScope();
                    Repository<TEntity> repo = scope.ServiceProvider.GetRequiredService<Repository<TEntity>>();
                    UnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();

                    repo.Remove(item.Id);
                    unitOfWork.Save();
                    Reload();
                }
                break;
        }
    }

    protected virtual void OnCopy(TAdapter? item)
    {
        if (item is null)
            return;

        TEntity entity = Mapper.Map<TEntity>(item);
        NavigationService.NavigateToEdit<TEntity, TAdapter>(entity);
    }

    protected virtual void OnEdit(TAdapter? item)
    {
        if (item is null)
            return;

        NavigationService.NavigateToEdit<TEntity, TAdapter>(item.Id);
    }

    protected virtual void OnView(TAdapter? item)
    { }

    protected virtual bool CanAdd()
        => true;

    protected virtual bool CanRemove(TAdapter? item)
        => true;

    protected virtual bool CanCopy(TAdapter? item)
        => true;

    protected virtual bool CanEdit(TAdapter? item)
        => true;

    protected virtual bool CanView(TAdapter? item)
        => true;
}