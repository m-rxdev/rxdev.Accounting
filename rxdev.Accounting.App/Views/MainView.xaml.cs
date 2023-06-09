using Microsoft.Extensions.DependencyInjection;
using ModernWpf.Controls;
using ModernWpf.Navigation;
using rxdev.Accounting.App.ApplicationServices;
using rxdev.Accounting.App.Resources.MVVM;
using rxdev.Accounting.App.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace rxdev.Accounting.App.Views;
/// <summary>
/// Interaction logic for MainView.xaml
/// </summary>
public partial class MainView : Window
{
    private MainViewModel? _viewModel;

    public MainView()
    {
        InitializeComponent();
    }

    private void NavView_PaneOpening(NavigationView sender, object args)
    {
        //UpdateAppTitleMargin(sender);
    }

    private void NavView_PaneClosing(NavigationView sender, NavigationViewPaneClosingEventArgs args)
    {
        //UpdateAppTitleMargin(sender);
    }

    private void NavView_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
    }

    private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        _viewModel?.NavigationService.NavigateTo((args.IsSettingsInvoked ? typeof(CompanyInfoEditViewModel) : args.InvokedItemContainer.Tag) as Type, NavView.SelectedItem);
        //((args.IsSettingsInvoked ? NavView.Tag : args.InvokedItemContainer.Tag) as RelayCommand)?.Execute(null);
    }

    private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        _viewModel?.NavigationService.NavigateBack();
    }

    public MainView Init(IServiceProvider serviceProvider)
    {
        _viewModel = serviceProvider.GetRequiredService<MainViewModel>();
        _viewModel.NavigationService.PropertyChanged += OnPropertyChanged;
        DataContext = _viewModel;
        return this;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(NavigationService.ViewModel))
            Dispatcher.BeginInvoke(UpdateSelectedItem);
    }

    private void UpdateSelectedItem()
    {
        Type? type = _viewModel?.NavigationService.ViewModel?.GetType();
        if (type is null)
            return;

        NavView.SelectedItem = NavView.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(item => item.Tag is Type t && t == type);
    }
}
