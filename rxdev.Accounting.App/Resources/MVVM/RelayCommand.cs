using System;
using System.Windows.Input;

namespace rxdev.Accounting.App.Resources.MVVM;

public class RelayCommand<T> : ICommand
{
    private readonly Func<T, bool>? _canExecute;
    private readonly Action<T> _execute;

    public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
    {
        if (execute is null)
            throw new ArgumentNullException(nameof(execute));

        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? parameter)
        => parameter is T val && (_canExecute?.Invoke(val) ?? true);

    public void Execute(object? parameter) 
        => _execute.Invoke((T)parameter!);
}

public class RelayCommand : ICommand
{
    private readonly Func<bool>? _canExecute;
    private readonly Action _execute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        if (execute is null)
            throw new ArgumentNullException(nameof(execute));

        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? parameter)
        => _canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) 
        => _execute.Invoke();
}