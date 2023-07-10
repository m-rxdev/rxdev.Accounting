using Microsoft.Web.WebView2.Wpf;
using rxdev.Accounting.Model;
using System;
using System.IO;
using System.Windows;

namespace rxdev.Accounting.App.Resources;

public class PDFViewer
    : WebView2
{

    public new static readonly DependencyProperty SourceProperty =
        DependencyProperty.RegisterAttached("Source", typeof(object), typeof(PDFViewer), new PropertyMetadata(null, OnSourceChanged));

    public new object Source { get => (string)GetValue(SourceProperty); set => SetValue(SourceProperty, value); }

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PDFViewer viewer)
            return;

        viewer.OnSourceChanged(e.NewValue);
    }

    static PDFViewer()
    {
        Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", Path.Combine(Path.GetTempPath(), "rxdev.Accounting"));
    }

    private void OnSourceChanged(object value)
    {
        switch (value)
        {
            case string path:
                base.Source = new Uri(path);
                break;

            case Attachment attachment:
                LoadFile(attachment.EntityData!.Data);
                break;

            case EntityData entityData:
                LoadFile(entityData.Data);
                break;

            case byte[] arr:
                LoadFile(arr);
                break;

            default:
                base.Source = new Uri("about:blank");
                break;
        }
    }

    private string? _filePath;

    private void LoadFile(byte[] data)
    {
        if (_filePath is not null)
            File.Delete(_filePath);

        _filePath = Path.GetTempFileName();
        File.WriteAllBytes(_filePath, data);
        base.Source = new Uri(_filePath);
    }
}