using Microsoft.Extensions.DependencyInjection;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.App.ApplicationServices;
using System;

namespace rxdev.Accounting.App.ViewModels;

public abstract class ViewModel : ApplicationService
{
    public class CommandsGroup
    {
        public class CommandsDefinition
            : Adapter
        {
            private bool _hasAdd;
            private bool _hasSave;
            private bool _hasGenerate;
            private bool _hasSync;
            private bool _hasEdit;
            private bool _hasRemove;
            private bool _hasPreview;
            private bool _hasUpload;

            public bool HasUpload { get => _hasUpload; set => Set(ref _hasUpload, value); }
            public bool HasPreview { get => _hasPreview; set => Set(ref _hasPreview, value); }
            public bool HasRemove { get => _hasRemove; set => Set(ref _hasRemove, value); }
            public bool HasEdit { get => _hasEdit; set => Set(ref _hasEdit, value); }
            public bool HasSync { get => _hasSync; set => Set(ref _hasSync, value); }
            public bool HasGenerate { get => _hasGenerate; set => Set(ref _hasGenerate, value); }
            public bool HasSave { get => _hasSave; set => Set(ref _hasSave, value); }
            public bool HasAdd { get => _hasAdd; set => Set(ref _hasAdd, value); }
        }
        public CommandsDefinition ActionBar { get; set; } = new();
        public CommandsDefinition ItemActions { get; set; } = new();
    }

    private string _title = string.Empty;

    public ViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        NotificationService = serviceProvider.GetRequiredService<NotificationService>();
        NavigationService = serviceProvider.GetRequiredService<NavigationService>();
    }

    public NotificationService NotificationService { get; private set; }
    public NavigationService NavigationService { get; private set; }
    public string Title { get => _title; protected set => Set(ref _title, value); }
    public CommandsGroup Commands { get; set; } = new();

    public virtual void Load(params object[] args)
    {
        Reload();
    }

    public virtual void Reload()
    { }

    public virtual bool Unload()
        => true;
}