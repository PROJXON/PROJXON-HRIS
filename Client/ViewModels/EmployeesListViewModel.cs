using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using Client.Models.EmployeeManagement;
using Client.Services;
using Client.Utils.Enums;
using Client.Utils.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.EmployeeManagement.Responses;

namespace Client.ViewModels;

public partial class EmployeesListViewModel(
    IEmployeeRepository employeeRepository,
    INavigationService navigationService)
    : ViewModelBase, INavigationAware
{
    [ObservableProperty]
    private ObservableCollection<EmployeeResponse> _employees = [];
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    public async Task OnNavigatedToAsync()
    {
        if (IsLoading) return;
        await LoadEmployeesCommand.ExecuteAsync(null);
    }

    public Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task LoadEmployeesAsync()
    {
        await ExecuteWithLoading(async () =>
        {
            var result = await employeeRepository.GetAllAsync<EmployeeResponse>();
            if (result.IsSuccess)
            {
                var employees = result.Value?.ToList() ?? [];
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Employees = new ObservableCollection<EmployeeResponse>(employees);
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
        navigationService.NavigateTo(ViewModelType.EmployeeDetails, employeeId);
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