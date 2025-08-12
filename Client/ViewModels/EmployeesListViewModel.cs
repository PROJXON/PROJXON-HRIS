using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using Client.Models.EmployeeManagement;
using Client.Services;
using Client.Utils.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.EmployeeManagement.Responses;

namespace Client.ViewModels;

public partial class EmployeesListViewModel(
    IEmployeeRepository employeeRepository,
    INavigationService navigationService)
    : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<EmployeeResponse> _employees = [];
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [RelayCommand]
    private async Task LoadEmployeesAsync()
    {
        await ExecuteWithLoading(async () =>
        {
            var result = await employeeRepository.GetAllAsync<EmployeeResponse>();
            if (result.IsSuccess)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Employees.Clear();
                    foreach (var employee in result.Value)
                    {
                        Console.WriteLine(employee);
                        Employees.Add(employee);
                    }
                });
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
            }
        });
    }

    [RelayCommand]
    private void OpenEmployeeDetailsPage(int employeeId)
    {
        navigationService.NavigateTo(ViewModelType.EmployeeDetails);
    }

    private async Task ExecuteWithLoading(Func<Task> operation)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            await operation();
        }
        finally
        {
            IsLoading = false;
        }
    }
}