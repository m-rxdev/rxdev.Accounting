using Microsoft.Extensions.DependencyInjection;
using ModernWpf.Controls;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.App.Resources.MVVM;
using rxdev.Accounting.Model;
using rxdev.Accounting.Persistence;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace rxdev.Accounting.App.ViewModels;

public abstract class EditViewModel<TEntity, TAdapter>
    : ViewModel
    where TEntity : Entity, new()
    where TAdapter : EntityAdapter, new()
{
    private ICommand? _cancelCommand;
    private TAdapter _item = new();
    private ICommand? _saveCommand;
    private bool _canEdit = true;

    protected EditViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        Commands.ActionBar.HasSave = true;
    }

    public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(OnCancel, CanCancel);
    public TAdapter Item { get => _item; set => Set(ref _item, value); }
    public ICommand SaveCommand => _saveCommand ??= new RelayCommand(OnSave, CanSave);
    public bool CanEdit { get => _canEdit; set => Set(ref _canEdit, value); }

    public override void Load(params object[] args)
    {
        TEntity? entity = null;

        if (args.Length > 0)
            switch (args[0])
            {
                case TEntity e:
                    entity = e;
                    break;

                case int id:
                    entity = GetQuery().FirstOrDefault(e => e.Id == id);
                    break;

            }

        Item = Mapper.Map<TAdapter>(entity ?? CreateEntity());
        Title = Item.Id > 0 ? $"{typeof(TEntity).Name} {{{Item.Id}}} Edition" : $"New {typeof(TEntity).Name}";
    }

    protected virtual bool CanCancel()
        => true;

    protected virtual bool CanSave()
        => Item.IsDirty || Item.Id == 0;

    protected virtual void OnCancel()
    {
        NavigationService.NavigateBack();
    }

    protected virtual void OnSave()
    {
        Save();
        NavigationService.NavigateBack();
    }

    protected virtual void Save()
    {
        using IServiceScope scope = ServiceProvider.CreateScope();
        UnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
        Repository<TEntity> repo = scope.ServiceProvider.GetRequiredService<Repository<TEntity>>();

        TEntity? entity = GetQuery(true).FirstOrDefault(e => e.Id == Item.Id);

        if (entity is null)
        {
            entity = Mapper.Map<TEntity>(Item);
            repo.Add(entity);
        }
        else
        {
            Mapper.Map(Item, entity);
            repo.Update(entity);
        }

        unitOfWork.Save();
        Item.Id = entity.Id;
        Item.IsDirty = false;
    }

    public override bool Unload()
    {
        if (!Item.IsDirty)
            return true;

        switch (NotificationService.Ask("There is unsaved changes. Do you want to save ?", "Unsaved Changes", MessageBoxButton.YesNoCancel))
        {
            case MessageBoxResult.Yes:
                Save();
                return true;
            case MessageBoxResult.No:
                return true;
            case MessageBoxResult.Cancel:
            default:
                return false;
        }
    }

    public override void Reload()
    {
        base.Reload();
        Load(Item.Id);
    }

    protected virtual IQueryable<TEntity> GetQuery(bool tracking = false)
    {
        Repository<TEntity> repo = ServiceProvider.GetRequiredService<Repository<TEntity>>();
        return repo.AsQueryable(tracking);
    }

    protected virtual TEntity CreateEntity()
        => new();
}