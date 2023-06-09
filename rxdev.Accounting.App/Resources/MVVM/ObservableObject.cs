using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace rxdev.Accounting.App.Resources.MVVM;

public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public void RaisePropertyChanged(object? sender, PropertyChangedEventArgs e) 
        => PropertyChanged?.Invoke(sender, e);

    public void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}