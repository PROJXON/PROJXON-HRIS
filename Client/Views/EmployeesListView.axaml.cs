using Avalonia.Controls;
using Client.ViewModels;

namespace Client.Views;

public partial class EmployeesListView : UserControl
{
    public EmployeesListView()
    {
        Loaded += OnLoaded;
        InitializeComponent();
    }

    private async void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
{
    if (DataContext is EmployeesListViewModel viewModel)
    {
        await viewModel.LoadEmployeesCommand.ExecuteAsync(null);
    }
}
}