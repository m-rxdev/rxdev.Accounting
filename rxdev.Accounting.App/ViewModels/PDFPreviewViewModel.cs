using System;

namespace rxdev.Accounting.App.ViewModels;

public class PDFPreviewViewModel
    : ViewModel
{
    private object? _source;

    public PDFPreviewViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    public object? Source { get => _source; set => Set(ref _source, value); }

    public override void Load(params object[] args)
    {
        Source = args.Length > 0 ? args[0] : null;
        base.Load(args);
    }
}