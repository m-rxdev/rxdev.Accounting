using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace rxdev.Accounting.App.ApplicationServices;

public class NotificationService
    : ApplicationService
{
    public NotificationService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public MessageBoxResult Ask(string message, string caption, MessageBoxButton buttons)
        => System.Windows.MessageBox.Show(message, caption, buttons);

    private void test()
    {



        /*
        var dial = new ContentDialog()
        {
            Title = "test",
            IsPrimaryButtonEnabled = true,
            Content = "unsaved changes",
            CloseButtonText = "test",
        };

        dial.ShowAsync();*/
    }
}
