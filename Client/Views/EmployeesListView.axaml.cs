using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Client.ViewModels;

namespace Client.Views;

public partial class EmployeesListView : UserControl
{
    public EmployeesListView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is EmployeesListViewModel viewModel)
        {
            await viewModel.LoadEmployeesCommand.ExecuteAsync(null);
        }
    }
}