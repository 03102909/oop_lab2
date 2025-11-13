using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Lab2.Views;

public partial class ExitDialog : Window
{
    public ExitDialog()
    {
        InitializeComponent();

        YesButton.Click += OnYesClick;
        NoButton.Click  += OnNoClick;
    }

    private void OnYesClick(object? sender, RoutedEventArgs e)
    {
        Close(true);
    }

    private void OnNoClick(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}