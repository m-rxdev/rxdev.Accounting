using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "rxdev.Accounting.App.Resources")]
namespace rxdev.Accounting.App.Resources;

public class ExtendedGrid : Grid
{
    public static readonly DependencyProperty ColumnsDefinitionProperty =
        DependencyProperty.RegisterAttached("ColumnsDefinition", typeof(string), typeof(ExtendedGrid), new PropertyMetadata("*", OnColumnsDefinitionChanged));

    public static readonly DependencyProperty RowsDefinitionProperty =
        DependencyProperty.RegisterAttached("RowsDefinition", typeof(string), typeof(ExtendedGrid), new PropertyMetadata("*", OnRowsDefinitionChanged));

    public static readonly DependencyProperty AutoPlacementProperty =
        DependencyProperty.RegisterAttached("AutoPlacement", typeof(bool), typeof(ExtendedGrid), new PropertyMetadata(false));

    public static readonly DependencyProperty RowAutoGenerationProperty =
        DependencyProperty.RegisterAttached("RowAutoGeneration", typeof(bool), typeof(ExtendedGrid), new PropertyMetadata(false));

    private static readonly GridLengthConverter Converter = new();
    public string ColumnsDefinition { get => (string)GetValue(RowsDefinitionProperty); set => SetValue(RowsDefinitionProperty, value); }
    public string RowsDefinition { get => (string)GetValue(RowsDefinitionProperty); set => SetValue(RowsDefinitionProperty, value); }
    public bool AutoPlacement { get => (bool)GetValue(AutoPlacementProperty); set => SetValue(AutoPlacementProperty, value); }
    public bool RowAutoGeneration { get => (bool)GetValue(RowAutoGenerationProperty); set => SetValue(RowAutoGenerationProperty, value); }

    private static void OnColumnsDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Grid grid
            || e.NewValue is not string structure)
            return;

        grid.ColumnDefinitions.Clear();

        foreach (GridLength length in structure.Split(',').Select(i => (GridLength)Converter.ConvertFromString(i)!))
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = length });
    }

    private static void OnRowsDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Grid grid
            || e.NewValue is not string structure)
            return;

        grid.RowDefinitions.Clear();

        foreach (GridLength length in structure.Split(',').Select(i => (GridLength)Converter.ConvertFromString(i)!))
            grid.RowDefinitions.Add(new RowDefinition { Height = length });
    }

    public override void EndInit()
    {
        if (AutoPlacement)
        {
            int x = 0;
            int y = 0;
            int colCount = ColumnDefinitions.Count;

            foreach (UIElement child in Children)
            {
                Grid.SetColumn(child, x);
                Grid.SetRow(child, y);

                x++;
                if (x >= colCount)
                {
                    x = 0;
                    y++;
                }
            }
        }

        if(RowAutoGeneration)
        {
            int cnt = Children.Cast<UIElement>().Max(Grid.GetRow);
            for (int i = RowDefinitions.Count; i <= cnt; i++)
                RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        base.EndInit();
    }
}