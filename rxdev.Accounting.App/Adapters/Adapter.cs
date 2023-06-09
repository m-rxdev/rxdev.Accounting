using System;
using System.Runtime.CompilerServices;
using rxdev.Accounting.App.Resources.MVVM;

namespace rxdev.Accounting.App.Adapters;

public abstract class Adapter : ObservableObject
{
    private bool _isDirty;
    public bool IsDirty { get => _isDirty; set => Set(ref _isDirty, value); }

    public bool Set<T>(ref T obj, T value, [CallerMemberName] string? propertyName = null, bool dirty = false, string[]? raise = null, Action? action = null)
    {
        if (obj != null && obj.Equals(value))
            return false;

        obj = value;
        RaisePropertyChanged(propertyName);
        if(raise is not null)
            foreach(string name in raise)
                RaisePropertyChanged(name);

        action?.Invoke();

        if (dirty)
            IsDirty = true;

        return true;
    }

    public bool SetDirty<T>(ref T obj, T value, [CallerMemberName] string? propertyName = null, string[]? raise = null, Action? action = null)
        => Set(ref obj, value, propertyName, true, raise, action);
}